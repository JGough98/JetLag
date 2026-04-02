using Community.Blazor.MapLibre;
using Community.Blazor.MapLibre.Models.Event;
using JetLag.Scripts.Utility.Reflection;
using Microsoft.AspNetCore.Components;

namespace JetLag.Scripts.Input;

public class MapMouseObserver : IMapMouseObserver
{
    private Listener? _onMouseMoveListener;
    private Listener? _onMouseLeaveListener;
    private Listener? _onClickListener;


    public EventCallback<MapMouseEvent> OnMouseMove { get; set; }
    public EventCallback<MapMouseEvent> OnMouseLeave { get; set; }
    public EventCallback<MapMouseEvent> OnClick { get; set; }


    public async Task Subscribe(MapLibre map)
    {
        Task<Listener> Sub(EventType type, Func<MapMouseEvent, Task> handler) =>
            map.AddAsyncListener(ReflectionUtility.GetEnumJsonName(type), handler);

        _onMouseMoveListener = await Sub(EventType.MouseMove, HandleMouseMove);
        _onMouseLeaveListener = await Sub(EventType.MouseOut, HandleMouseLeave);
        _onClickListener = await Sub(EventType.Click, HandleClick);
    }

    public void Unsubscribe()
    {
        _onMouseMoveListener?.Dispose();
        _onMouseLeaveListener?.Dispose();
        _onClickListener?.Dispose();
    }


    private Task HandleMouseMove(MapMouseEvent e) => Handle(e, OnMouseMove);

    private Task HandleMouseLeave(MapMouseEvent e) => Handle(e, OnMouseLeave);

    private Task HandleClick(MapMouseEvent e) => Handle(e, OnClick);

    private Task Handle(MapMouseEvent e, EventCallback<MapMouseEvent> handler)
    {
        if (handler.HasDelegate)
            return handler.InvokeAsync(e);

        return Task.CompletedTask;
    }
}