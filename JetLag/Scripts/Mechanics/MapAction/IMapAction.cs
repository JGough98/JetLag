using Community.Blazor.MapLibre.Models.Event;
using JetLag.Scripts.Models;

namespace JetLag.Scripts.Mechanics.MapAction;

public interface IMapAction
{
    public string Name { get; }


    public Task HandleLeave(
        MapMouseEvent mapMouseEvent,
        QuestionButtonEventArgs questionButtonEventArgs
    );

    public Task HandleClick(
        MapMouseEvent mapMouseEvent,
        QuestionButtonEventArgs questionButtonEventArgs
    );

    public Task HandleMove(
        MapMouseEvent mapMouseEvent,
        QuestionButtonEventArgs questionButtonEventArgs
    );
}
