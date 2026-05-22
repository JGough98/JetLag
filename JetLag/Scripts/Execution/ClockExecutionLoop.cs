using Community.Blazor.MapLibre;
using Community.Blazor.MapLibre.Models;
using JetLag.Scripts.Data;
using JetLag.Scripts.GtfsRt;
using JetLag.Scripts.Models;
using JetLag.Scripts.Render;

namespace JetLag.Scripts.Execution;

public class ClockExecutionLoop : IClockExecutionLoop
{
    private const int WallClockIntervalMs = 500;

    private readonly GameClock _clock;
    private readonly ITripInterpolator _interpolator;
    private readonly PlayerMarkerRender _playerMarkerRender;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ClockExecutionLoop> _logger;

    private CancellationTokenSource? _cts;
    private MapLibre? _map;
    private IReadOnlyList<TripStopTime>? _stops;
    private string? _tripId;
    private string? _destinationStopId;
    private int _scheduledArrivalSeconds;
    private Func<ExecutionOutcome, Task>? _onPaused;

    public bool IsRunning => _cts is not null && !_cts.IsCancellationRequested;

    public ClockExecutionLoop(
        GameClock clock,
        ITripInterpolator interpolator,
        PlayerMarkerRender playerMarkerRender,
        IServiceScopeFactory scopeFactory,
        ILogger<ClockExecutionLoop> logger)
    {
        _clock = clock;
        _interpolator = interpolator;
        _playerMarkerRender = playerMarkerRender;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public Task StartAsync(
        MapLibre map,
        IReadOnlyList<TripStopTime> stops,
        string tripId,
        string destinationStopId,
        int scheduledArrivalSeconds,
        Func<ExecutionOutcome, Task> onPaused)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        _map = map;
        _stops = stops;
        _tripId = tripId;
        _destinationStopId = destinationStopId;
        _scheduledArrivalSeconds = scheduledArrivalSeconds;
        _onPaused = onPaused;

        _ = Task.Run(() => RunLoopAsync(_cts.Token));
        return Task.CompletedTask;
    }

    public void Pause()
    {
        _cts?.Cancel();
        _cts = null;
    }

    public Task ResumeAsync()
    {
        if (IsRunning || _stops is null || _map is null || _onPaused is null)
            return Task.CompletedTask;

        _cts = new CancellationTokenSource();
        _ = Task.Run(() => RunLoopAsync(_cts.Token));
        return Task.CompletedTask;
    }

    public void Stop()
    {
        _cts?.Cancel();
        _cts = null;
        _stops = null;
        _map = null;
        _tripId = null;
        _destinationStopId = null;
        _onPaused = null;
    }

    private async Task RunLoopAsync(CancellationToken ct)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(WallClockIntervalMs));

        try
        {
            while (!ct.IsCancellationRequested && await timer.WaitForNextTickAsync(ct))
            {
                _clock.Tick();

                var (lat, lon) = _interpolator.Interpolate(_stops!, _clock.CurrentTime);

                if (_playerMarkerRender.MarkerId is { } markerId)
                    await _map!.MoveMarker(markerId, new LngLat(lon, lat));

                if (_clock.CurrentTime.TotalSeconds >= _scheduledArrivalSeconds)
                {
                    var outcome = await ResolveOutcomeAsync();
                    _cts?.Cancel();
                    await _onPaused!(outcome);
                    return;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Normal pause/stop — not an error
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Clock execution loop error");
        }
    }

    private async Task<ExecutionOutcome> ResolveOutcomeAsync()
    {
        var stopName = _stops!.FirstOrDefault(s => s.StopId == _destinationStopId)?.StopName
                       ?? _destinationStopId!;
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var resolver = scope.ServiceProvider.GetRequiredService<IHistoricalResolutionService>();
            var tripOutcome = await resolver.ResolveAsync(_tripId!, _destinationStopId!, _scheduledArrivalSeconds);

            if (tripOutcome is null)
                return new ExecutionOutcome(ExecutionPauseReason.Arrived, stopName, 0, false);

            if (tripOutcome.IsCancelled)
                return new ExecutionOutcome(ExecutionPauseReason.Cancelled, stopName, 0, true);

            return tripOutcome.DelaySeconds > 60
                ? new ExecutionOutcome(ExecutionPauseReason.Delayed, stopName, tripOutcome.DelaySeconds, false)
                : new ExecutionOutcome(ExecutionPauseReason.Arrived, stopName, tripOutcome.DelaySeconds, false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving trip outcome for trip {TripId}", _tripId);
            return new ExecutionOutcome(ExecutionPauseReason.Arrived, stopName, 0, false);
        }
    }
}
