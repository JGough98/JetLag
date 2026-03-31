namespace JetLag.Scripts.Render;

using Community.Blazor.MapLibre;


public interface IMapLayerRender
{
    public Task Draw(double[][] newCoordinates, MapLibre map);

    public Task InvertDraw(double[][] newCoordinates, MapLibre map);

    public Task Clear(MapLibre map);
}
