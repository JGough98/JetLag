using Community.Blazor.MapLibre;
using Community.Blazor.MapLibre.Models;
using Community.Blazor.MapLibre.Models.Camera;
using Community.Blazor.MapLibre.Models.Event;
using Microsoft.AspNetCore.Components;

using JetLag.Scripts.Data;
using JetLag.Scripts.Execution;
using JetLag.Scripts.Factory.Interface;
using JetLag.Scripts.Input;
using JetLag.Scripts.Models;
using JetLag.Scripts.Render;
using JetLag.Scripts.Transit;
using JetLag.Scripts.Mechanics.MapAction;


namespace JetLag.Scripts.Intialize;

public class MapLibreOrchestrator : IMapOrchestrator<MapLibre>
{
    private readonly IMapMouseObserver _mapMouseObserver;
    private readonly IFactory<IReadOnlyList<QuestionCardModel>, QuestionCardFactoryInput> _cardFactory;
    private readonly IMapActionManager _mapActionManager;
    private readonly MapRender _mapRender;
    private readonly RailwayLayerRender _railwayLayerRender;
    private readonly StationLayerRender _stationLayerRender;
    private readonly TripPathLayerRender _tripPathLayerRender;
    private readonly PlayerMarkerRender _playerMarkerRender;
    private readonly IClockExecutionLoop _executionLoop;
    private readonly GameClock _clock;
    private readonly PlanningState _planningState;
    private readonly ITransitService _transitService;

    private MapLibre? _map;
    private bool _isPlaybackRunning;
    private IReadOnlyList<TripStopTime>? _currentTripStops;
    private string? _destinationStopId;

    public IReadOnlyList<QuestionCardModel> Cards { get; private set; } = Array.Empty<QuestionCardModel>();
    public bool IsPlaybackRunning => _isPlaybackRunning;


    public MapLibreOrchestrator(
        IMapMouseObserver mapMouseObserver,
        IFactory<IReadOnlyList<QuestionCardModel>, QuestionCardFactoryInput> cardFactory,
        IMapActionManager mapActionManager,
        MapRender mapRender,
        RailwayLayerRender railwayLayerRender,
        StationLayerRender stationLayerRender,
        TripPathLayerRender tripPathLayerRender,
        PlayerMarkerRender playerMarkerRender,
        IClockExecutionLoop executionLoop,
        GameClock clock,
        PlanningState planningState,
        ITransitService transitService
    )
    {
        _mapMouseObserver = mapMouseObserver;
        _cardFactory = cardFactory;
        _mapActionManager = mapActionManager;
        _mapRender = mapRender;
        _railwayLayerRender = railwayLayerRender;
        _stationLayerRender = stationLayerRender;
        _tripPathLayerRender = tripPathLayerRender;
        _playerMarkerRender = playerMarkerRender;
        _executionLoop = executionLoop;
        _clock = clock;
        _planningState = planningState;
        _transitService = transitService;
    }


    public void Initialize(IHandleEvent uiComponent)
    {
        Cards = _cardFactory.Create(
            new QuestionCardFactoryInput(uiComponent, _mapActionManager.HandleQuestionButton)
        );
    }

    public async Task MapLoaded(MapLibre map, IHandleEvent uiComponent, EventArgs args)
    {
        _map = map;

        await _mapMouseObserver.Subscribe(map);

        await _railwayLayerRender.InitializeAsync(map);
        await _stationLayerRender.InitializeAsync(map);
        _tripPathLayerRender.Initialize(map);

        _mapRender.Intialize(map);

        _planningState.OnChange += () =>
        {
            _ = _planningState.SelectedTripId is not null
                ? _tripPathLayerRender.DrawTripPath(_planningState.SelectedTripId)
                : _tripPathLayerRender.Clear();
        };

        _mapMouseObserver.OnMouseMove = EventCallback.Factory.Create<MapMouseEvent>(
            uiComponent,
            _mapActionManager.HandleMove
        );
        _mapMouseObserver.OnMouseLeave = EventCallback.Factory.Create<MapMouseEvent>(
            uiComponent,
            _mapActionManager.HandleLeave
        );
        _mapMouseObserver.OnClick = EventCallback.Factory.Create<MapMouseEvent>(
            uiComponent,
            (MapMouseEvent e) => HandleClickWithStationDetection(e)
        );

        await _mapActionManager.HandleMapLoaded(args);
    }

    public async Task StartPlaybackAsync(string tripId, Func<ExecutionOutcome, Task> onPaused)
    {
        if (_map is null) return;

        var stops = await _transitService.GetStopTimesForTrip(tripId);
        if (stops.Count < 2) return;

        _currentTripStops = stops;
        var destination = stops[^1];
        _destinationStopId = destination.StopId;

        await _playerMarkerRender.PlaceAsync(_map, stops[0].Lat, stops[0].Lon);
        _clock.Reset(TimeSpan.FromSeconds(stops[0].ArrivalSeconds));

        _isPlaybackRunning = true;

        await _executionLoop.StartAsync(
            _map,
            stops,
            tripId,
            destination.StopId,
            destination.ArrivalSeconds,
            outcome => HandleLoopPaused(outcome, onPaused));
    }

    public void StopPlayback()
    {
        _executionLoop.Stop();
        _isPlaybackRunning = false;

        if (_map is not null)
            _ = _playerMarkerRender.RemoveAsync(_map);

        _currentTripStops = null;
        _destinationStopId = null;
    }

    public void Stop()
    {
        StopPlayback();
        _mapMouseObserver.Unsubscribe();
    }


    private async Task HandleLoopPaused(ExecutionOutcome outcome, Func<ExecutionOutcome, Task> onPaused)
    {
        _isPlaybackRunning = false;

        if (outcome.Reason != ExecutionPauseReason.Arrived && _currentTripStops is not null && _map is not null)
        {
            var dest = _currentTripStops.FirstOrDefault(s => s.StopId == _destinationStopId);
            if (dest is not null)
                await _map.FlyTo(new FlyToOptions
                {
                    Center = new LngLat(dest.Lon, dest.Lat),
                    Zoom = 14,
                    Speed = 1.0
                });
        }

        await onPaused(outcome);
    }

    private async Task HandleClickWithStationDetection(MapMouseEvent e)
    {
        var nearest = _stationLayerRender.FindNearestStopWithinRadius(
            e.LngLat.Latitude,
            e.LngLat.Longitude
        );

        if (nearest is not null)
        {
            await _stationLayerRender.HighlightStop(nearest);
            var now = DateTime.Now.TimeOfDay;
            var departures = await _transitService.GetDepartingTrains(
                nearest.StopId,
                now,
                now.Add(TimeSpan.FromHours(2))
            );
            _planningState.SelectStop(nearest, departures);
            return;
        }

        await _mapActionManager.HandleClick(e);
    }
}
