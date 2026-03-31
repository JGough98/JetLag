namespace JetLag.Scripts.Render;

using Community.Blazor.MapLibre;


/// <summary>
/// Interface to render on a map.
/// </summary>
public interface IMapLayerShapeRender
{
    public Task RenderStraightLine(MapLibre map, double latitude, double longitude, double angle);

    public Task RenderCircle(MapLibre map, double latitude, double longitude, double radius);

    public Task RenderInvertCircle(MapLibre map, double latitude, double longitude, double radius);
}
