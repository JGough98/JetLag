namespace JetLag.Scripts.Render;

using Community.Blazor.MapLibre;


public interface IMapLayerRender
{
    public Task Add(double[][] newCoordinates, MapLibre map);

    public Task AddInverted(double[][] newCoordinates, MapLibre map);

    public Task Clear(MapLibre map);
}
