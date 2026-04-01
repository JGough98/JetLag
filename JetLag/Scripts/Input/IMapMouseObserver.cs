using Community.Blazor.MapLibre;
using Community.Blazor.MapLibre.Models.Event;
using Microsoft.AspNetCore.Components;


namespace JetLag.Scripts.Input;

public interface IMapMouseObserver
{
    public EventCallback<MapMouseEvent> OnMouseMove { get; set; }
    public EventCallback<MapMouseEvent> OnClick { get; set; }

    public Task Subscribe(MapLibre map);

    public void Unsubscribe();
}
