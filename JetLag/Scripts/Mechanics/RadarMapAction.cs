using Community.Blazor.MapLibre;
using Community.Blazor.MapLibre.Models.Event;
using JetLag.Scripts.Models;
using JetLag.Scripts.Render;


namespace JetLag.Scripts.Mechanics;

public class RadarMapAction : IMapAction
{
    private MapRender _mapRender;

    private MapLibre _map;


    public string Name => "RADAR";


    public RadarMapAction(MapRender mapRender)
    {
        _mapRender = mapRender;
    }


    public async Task HandleClick(
        MapMouseEvent mapMouseEvent,
        QuestionButtonEventArgs questionButtonEventArgs
    )
    {
        await _mapRender.OutOfBounds.RenderCircle(
            _map,
            mapMouseEvent.LngLat.Latitude,
            mapMouseEvent.LngLat.Longitude,
            questionButtonEventArgs.Size
        );
        await _mapRender.Overlay.Clear(_map);
    }

    public async Task HandleMove(
        MapMouseEvent mapMouseEvent,
        QuestionButtonEventArgs questionButtonEventArgs
    )
    {
        await _mapRender.Overlay.ReplaceCircle(
            _map,
            mapMouseEvent.LngLat.Latitude,
            mapMouseEvent.LngLat.Longitude,
            questionButtonEventArgs.Size
        );
    }

    public void Intialize(MapLibre map)
    {
        _map = map;
    }
}
