using Community.Blazor.MapLibre.Models.Event;
using JetLag.Scripts.Models;


namespace JetLag.Scripts.Mechanics.MapAction;

public interface IMapActionManager
{
    public  Task HandleQuestionButton(QuestionButtonEventArgs questionButtonEventArgs);

    public Task HandleClick(MapMouseEvent mapMouseEvent);

    public Task HandleMove(MapMouseEvent mapMouseEvent);

    public  Task HandleMapLoaded(EventArgs args);
}
