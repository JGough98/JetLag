namespace JetLag.Scripts.Render;

using Community.Blazor.MapLibre;


/// <summary>
/// Interface to render on a map.
/// </summary>
public interface IMapRender
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public Task RenderStraightLine(MapLibre map, double latitude, double longitude, double angle);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public Task RenderCircle(MapLibre map, double latitude, double longitude, double radius);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public Task RenderInvertCircle(MapLibre map, double latitude, double longitude, double radius);
}
