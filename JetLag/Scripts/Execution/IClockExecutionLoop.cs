using Community.Blazor.MapLibre;
using JetLag.Scripts.Models;

namespace JetLag.Scripts.Execution;

public interface IClockExecutionLoop
{
    bool IsRunning { get; }

    Task StartAsync(
        MapLibre map,
        IReadOnlyList<TripStopTime> stops,
        string tripId,
        string destinationStopId,
        int scheduledArrivalSeconds,
        Func<ExecutionOutcome, Task> onPaused);

    void Pause();
    Task ResumeAsync();
    void Stop();
}
