using Community.Blazor.MapLibre;


namespace JetLag.Scripts.Render;

/// <summary>
/// Interface to render on a map.
/// </summary>
public interface IMapLayerShapeRender
{
    public void Intialize(MapLibre map);

    public Task RenderStraightLine(double latitude, double longitude, double angle);

    public Task RenderCircle(double latitude, double longitude, double radius);

    public Task RenderInvertCircle(double latitude, double longitude, double radius);

    public Task ReplaceCircle(double latitude, double longitude, double radius);

    public Task Clear();
}
