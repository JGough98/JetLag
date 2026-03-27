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

    private double[][] RectanclePolygon(double latitude, double longitude, double angle)
    {
        double angleRad = angle * Math.PI / 180.0;
        double sinA = Math.Sin(angleRad);
        double cosA = Math.Cos(angleRad);
        const double eps      = 1e-9;
        const double northLat =  85.051129; // Web Mercator north limit
        const double southLat = -75.0;      // Southern limit — cuts off Antarctica
        double latRange = northLat - southLat;

        // Find where the line (x = longitude + t*sinA, y = latitude + t*cosA)
        // exits the map boundary [-180,180] x [southLat, northLat].
        var tValues = new List<double>();

        if (Math.Abs(cosA) > eps)
        {
            double t = (northLat - latitude) / cosA;
            if (Math.Abs(longitude + t * sinA) <= 180 + eps) tValues.Add(t);

            t = (southLat - latitude) / cosA;
            if (Math.Abs(longitude + t * sinA) <= 180 + eps) tValues.Add(t);
        }
        if (Math.Abs(sinA) > eps)
        {
            double t = ( 180.0 - longitude) / sinA;
            double y = latitude + t * cosA;
            if (y >= southLat - eps && y <= northLat + eps) tValues.Add(t);

            t = (-180.0 - longitude) / sinA;
            y = latitude + t * cosA;
            if (y >= southLat - eps && y <= northLat + eps) tValues.Add(t);
        }

        tValues.Sort();
        double tLo = tValues[0], tHi = tValues[^1];

        double[] pLo = [
            Math.Clamp(longitude + tLo * sinA, -180, 180),
            Math.Clamp(latitude  + tLo * cosA, southLat, northLat)
        ];
        double[] pHi = [
            Math.Clamp(longitude + tHi * sinA, -180, 180),
            Math.Clamp(latitude  + tHi * cosA, southLat, northLat)
        ];

        // Clockwise boundary parameter s in [0,4): Top=0..1, Right=1..2, Bottom=2..3, Left=3..4
        double BoundaryS(double[] p) =>
            Math.Abs(p[1] - northLat) < eps ? (p[0] + 180.0) / 360.0 :
            Math.Abs(p[0] - 180.0)    < eps ? 1.0 + (northLat - p[1]) / latRange :
            Math.Abs(p[1] - southLat) < eps ? 2.0 + (180.0 - p[0]) / 360.0 :
                                              3.0 + (p[1] - southLat) / latRange;

        double sHi  = BoundaryS(pHi);
        double sLo  = BoundaryS(pLo);
        double span = (sLo - sHi + 4.0) % 4.0;

        var corners = new (double s, double[] coord)[]
        {
            (0.0, [-180.0, northLat]),
            (1.0, [ 180.0, northLat]),
            (2.0, [ 180.0, southLat]),
            (3.0, [-180.0, southLat])
        };

        // Build the polygon:
        // 1. Diagonal edge from pLo to pHi with 64 intermediate points so that
        //    Mercator's non-linear y-scale doesn't shift the rendered line away from the origin.
        // 2. Clockwise boundary walk from pHi back to pLo through the right-side corners.
        const int segments = 64;
        var polygon = new List<double[]>(segments + 8);

        for (int i = 0; i <= segments; i++)
        {
            double t = tLo + (tHi - tLo) * i / segments;
            polygon.Add([
                Math.Clamp(longitude + t * sinA, -180, 180),
                Math.Clamp(latitude  + t * cosA, southLat, northLat)
            ]);
        }

        foreach (var (s, coord) in corners.OrderBy(c => (c.s - sHi + 4.0) % 4.0))
        {
            double dist = (s - sHi + 4.0) % 4.0;
            if (dist > eps && dist < span - eps)
                polygon.Add(coord);
        }

        polygon.Add(polygon[0]); // close

        return polygon.ToArray();
    }
}
