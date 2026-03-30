namespace JetLag.Scripts.Render;

using Community.Blazor.MapLibre;
using JetLag.Scripts.Utility;


public class MapLayerShapeRender : IMapLayerShapeRender
{
    private readonly IMapLayerRender _mapLayerRender;


    public MapLayerShapeRender(IMapLayerRender mapLayerRender)
    {
        _mapLayerRender = mapLayerRender;
    }


    public async Task RenderCircle(MapLibre map, double latitude, double longitude, double radius)
        => await _mapLayerRender.Add(CirclePolygon(latitude, longitude, radius), map);

    public async Task RenderInvertCircle(MapLibre map, double latitude, double longitude, double radius)
        => await _mapLayerRender.AddInverted(CirclePolygon(latitude, longitude, radius), map);

    public async Task RenderStraightLine(MapLibre map, double latitude, double longitude, double angle)
        => await _mapLayerRender.Add(RectanclePolygon(latitude, longitude, angle), map);


    private double[][] CirclePolygon(double latitude, double longitude, double radius)
        => PolygonUtility
            .GenerateCirclePolygon(latitude, longitude, radius, points: 64)
            .Select(p => new double[] { p[1], p[0] })
            .ToArray();

    private double[][] RectanclePolygon(double latitude, double longitude, double angle)
        => PolygonUtility
            .GenerateRectanclePolygon(latitude, longitude, angle)
            .ToArray();
}
