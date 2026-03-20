namespace JetLag.Scripts.Render;

using Community.Blazor.MapLibre;


/// <summary>
/// Interface to render on a map.
/// </summary>
public interface IMapRender
{
    public Task RenderCircle(MapLibre map, double Latitude, double Longitude, double Radius);
}
