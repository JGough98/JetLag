using Community.Blazor.MapLibre;
using Community.Blazor.MapLibre.Models.Event;
using Microsoft.AspNetCore.Components;


namespace JetLag.Scripts.Input;

public class MapMouseObserver : IMapMouseObserver
{
    private Listener? _onMouseMoveListener;
    private Listener? _onClickListener;


    public EventCallback<MapMouseEvent> OnMouseMove { get; set; }
    public EventCallback<MapMouseEvent> OnClick { get; set; }


    public async Task Subscribe(MapLibre map)
    {
        _onMouseMoveListener = await map.AddAsyncListener<MapMouseEvent>(
            "mousemove",//EventType.MouseMove.ToString(),
            HandleMove
        );
        _onClickListener = await map.AddAsyncListener<MapMouseEvent>(
            "click",//EventType.Click.ToString(),
            HandleClick
        );
    }

    public void Unsubscribe()
    {
        _onMouseMoveListener?.Dispose();
        _onClickListener?.Dispose();
    }

    private async Task HandleMove(MapMouseEvent e)
    {
        if (OnMouseMove.HasDelegate)
            await OnMouseMove.InvokeAsync(e);
    }

    private async Task HandleClick(MapMouseEvent e)
    {
        if (OnClick.HasDelegate)
            await OnClick.InvokeAsync(e);
    }
}