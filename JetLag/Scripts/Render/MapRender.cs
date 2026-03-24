namespace JetLag.Scripts.Render;

using Community.Blazor.MapLibre;
using JetLag.Scripts.Utility;


public class MapRender : IMapRender
{
    private IMapLayerRender _mapLayerRender;


    public MapRender(IMapLayerRender mapLayerRender)
    {
        _mapLayerRender = mapLayerRender;
    }


    public async Task RenderCircle(MapLibre map, double latitude, double longitude, double radius)
        => await _mapLayerRender.Add(CirclePolygon(latitude, longitude, radius), map);

    public async Task RenderInvertCircle(MapLibre map, double latitude, double longitude, double radius)
        => await _mapLayerRender.AddInverted(CirclePolygon(latitude, longitude, radius), map);

    public async Task RenderStraightLine(MapLibre map, double latitude, double longitude, double angle)
        => await _mapLayerRender.Add(RectanclePolygon(latitude, longitude, angle), map);


    // TODO - Maybe should consider a function that increases the number of "points" the larger the radius is?
    private double[][] CirclePolygon(double latitude, double longitude, double radius)
        => PolygonUtility
            .GenerateCirclePolygon(latitude, longitude, radius, points: 64)
            .Select(p => new double[] { p[1], p[0] })
            .ToArray();

    // TODO - Alter the rectangle drawn to account for angle.
    private double[][] RectanclePolygon(double latitude, double longitude, double angle)
    {
        var rightSideCoords = new double[][]
        {
            [longitude, -90],
            [longitude,  90],
            [180,        90],
            [180,       -90],
            [longitude, -90]
        };

        return rightSideCoords;
    }
}
