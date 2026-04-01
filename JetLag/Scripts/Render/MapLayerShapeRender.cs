using Community.Blazor.MapLibre;
using JetLag.Scripts.Utility;


namespace JetLag.Scripts.Render;

public class MapLayerShapeRender : IMapLayerShapeRender
{
    private readonly IMapLayerRender _mapLayerRender;

    private MapLibre? _map;


    public MapLayerShapeRender(IMapLayerRender mapLayerRender)
    {
        _mapLayerRender = mapLayerRender;
    }


    public void Intialize(MapLibre map)
    {
        _map = map;
    }

    public async Task RenderCircle(double latitude, double longitude, double radius) =>
        await _mapLayerRender.Draw(CirclePolygon(latitude, longitude, radius), _map!);

    public async Task RenderInvertCircle(double latitude, double longitude, double radius) =>
        await _mapLayerRender.InvertDraw(CirclePolygon(latitude, longitude, radius), _map!);

    public async Task RenderStraightLine(double latitude, double longitude, double angle) =>
        await _mapLayerRender.Draw(RectanclePolygon(latitude, longitude, angle), _map!);

    public async Task ReplaceCircle(double latitude, double longitude, double radius) =>
        await _mapLayerRender.Replace(CirclePolygon(latitude, longitude, radius), _map!);

    public async Task Clear() => await _mapLayerRender.Clear(_map!);


    private double[][] CirclePolygon(double latitude, double longitude, double radius) =>
        PolygonUtility
            .GenerateCirclePolygon(latitude, longitude, radius, points: 64)
            .Select(p => new double[] { p[1], p[0] })
            .ToArray();

    private double[][] RectanclePolygon(double latitude, double longitude, double angle) =>
        PolygonUtility.GenerateRectanclePolygon(latitude, longitude, angle).ToArray();
}
