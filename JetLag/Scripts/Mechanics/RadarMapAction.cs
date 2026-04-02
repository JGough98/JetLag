using Community.Blazor.MapLibre.Models.Event;
using JetLag.Scripts.Models;
using JetLag.Scripts.Render;


namespace JetLag.Scripts.Mechanics;

public class RadarMapAction : IMapAction
{
    private readonly MapRender _mapRender;

    private readonly IHiderProxy _hiderProxy;


    public string Name => "RADAR";


    public RadarMapAction(MapRender mapRender, IHiderProxy hiderProxy)
    {
        _mapRender = mapRender;
        _hiderProxy = hiderProxy;
    }


    public async Task HandleClick(
        MapMouseEvent mapMouseEvent,
        QuestionButtonEventArgs questionButtonEventArgs
    )
    {
        var hitHider = await _hiderProxy.HitHider(
            mapMouseEvent.LngLat.Latitude,
            mapMouseEvent.LngLat.Longitude,
            questionButtonEventArgs.Size
        );

        if (hitHider)
        {
            await _mapRender.OutOfBounds.RenderInvertCircle(
                mapMouseEvent.LngLat.Latitude,
                mapMouseEvent.LngLat.Longitude,
                questionButtonEventArgs.Size
            );
        }
        else
        {
            await _mapRender.OutOfBounds.RenderCircle(
                mapMouseEvent.LngLat.Latitude,
                mapMouseEvent.LngLat.Longitude,
                questionButtonEventArgs.Size
            );
        }

        await _mapRender.Overlay.Clear();
    }

    public async Task HandleMove(
        MapMouseEvent mapMouseEvent,
        QuestionButtonEventArgs questionButtonEventArgs
    )
    {
        await _mapRender.Overlay.ReplaceCircle(
            mapMouseEvent.LngLat.Latitude,
            mapMouseEvent.LngLat.Longitude,
            questionButtonEventArgs.Size
        );
    }
}
