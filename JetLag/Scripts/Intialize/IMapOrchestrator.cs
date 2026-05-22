using JetLag.Scripts.Models;
using Microsoft.AspNetCore.Components;


namespace JetLag.Scripts.Intialize;

public interface IMapOrchestrator<T>
{
    public IReadOnlyList<QuestionCardModel> Cards { get; }
    public bool IsPlaybackRunning { get; }

    public void Initialize(IHandleEvent uiComponent);
    public Task MapLoaded(T map, IHandleEvent uiComponent, EventArgs args);
    public Task StartPlaybackAsync(string tripId, Func<ExecutionOutcome, Task> onPaused);
    public void StopPlayback();
    public void Stop();
}