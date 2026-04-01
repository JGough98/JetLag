using Community.Blazor.MapLibre;


namespace JetLag.Scripts.Render;

public interface IMapLayerRender
{
    public Task Draw(double[][] newCoordinates, MapLibre map);

    public Task InvertDraw(double[][] newCoordinates, MapLibre map);

    public Task Replace(double[][] newCoordinates, MapLibre map);

    public Task Clear(MapLibre map);
}
