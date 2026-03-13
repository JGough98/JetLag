namespace JetLag.Scripts.Render;

using LeafletForBlazor;


/// <summary>
/// Interface to render on a map.
/// </summary>
public interface IMapRender
{
    public Task RenderCircle(RealTimeMap.MapEventArgs args, double Latitude, double Longitude, double Radius);
}
