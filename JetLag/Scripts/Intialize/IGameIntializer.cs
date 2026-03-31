using JetLag.Scripts.Models;
using Microsoft.AspNetCore.Components;


namespace JetLag.Scripts.Intialize;

public interface IGameIntializer<T>
{
    public IReadOnlyList<QuestionCardModel> Cards { get; }


    public void Awake(IHandleEvent uiComponent);

    public Task Intialize(T map, IHandleEvent uiComponent);

    public Task HandleMapLoaded(EventArgs args);

    public void Stop();
}