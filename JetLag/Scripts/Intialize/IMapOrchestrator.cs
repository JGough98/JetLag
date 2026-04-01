using JetLag.Scripts.Models;
using Microsoft.AspNetCore.Components;


namespace JetLag.Scripts.Intialize;

public interface IMapOrchestrator<T>
{
    public IReadOnlyList<QuestionCardModel> Cards { get; }


    public void Initialize(IHandleEvent uiComponent);

    public Task MapLoaded(T map, IHandleEvent uiComponent, EventArgs args);

    public void Stop();
}