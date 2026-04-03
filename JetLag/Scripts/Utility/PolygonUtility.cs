namespace JetLag.Scripts.Utility;


/// <summary>
/// Utility class for generating polygons.
/// </summary>
public static class PolygonUtility
{
    /// <summary>
    /// Generates a circle polygon.
    /// </summary>
    /// <param name="centerLat">The latitude of the center of the circle.</param>
    /// <param name="centerLng">The longitude of the center of the circle.</param>
    /// <param name="radiusMeters">The radius of the circle in meters.</param>
    /// <param name="points">The number of points to generate.</param>
    /// <returns>A list of double arrays representing the polygon.</returns>
    public static List<double[]> GenerateCirclePolygon(double centerLat, double centerLng, double radiusMeters, int points = 64)
    {
        const double earthRadius = 6371000;
        var polygon = new List<double[]>();
        for (int i = 0; i <= points; i++)
        {
            double angle = 2 * Math.PI * i / points;
            double latOffset = radiusMeters / earthRadius * (180.0 / Math.PI);
            double lngOffset = radiusMeters / earthRadius * (180.0 / Math.PI) / Math.Cos(centerLat * Math.PI / 180.0);
            polygon.Add(new double[]
            {
                centerLat + latOffset * Math.Sin(angle),
                centerLng + lngOffset * Math.Cos(angle)
            });
        }
        return polygon;
    }


    public static List<double[]> GenerateBandPolygon(double latitude, double longitude, double angle)
    {
        const double earthRadius = 6371000.0;
        const double halfRadius  = earthRadius / 2.0;
        const double northLat    =  85.051129;
        const double southLat    = -85.051129;
        const double eps         = 1e-9;
        const int    segments    = 64;

        double latRange = northLat - southLat;
        double angleRad = angle * Math.PI / 180.0;
        double sinA     = Math.Sin(angleRad);
        double cosA     = Math.Cos(angleRad);

        // Convert half-radius to degree offsets at the origin latitude.
        // Perpendicular 90° CW from line direction in (Δlng, Δlat) space:
        //   line direction = (sinA,  cosA)
        //   perp CW       = (cosA, -sinA)
        double latOffDeg = halfRadius / earthRadius * (180.0 / Math.PI);
        double lngOffDeg = halfRadius / (earthRadius * Math.Cos(latitude * Math.PI / 180.0)) * (180.0 / Math.PI);

        // Line-1 origin: shifted +½R perp  →  (lng + cosA·lngOff,  lat − sinA·latOff)
        double lat1 = latitude  - sinA * latOffDeg;
        double lng1 = longitude + cosA * lngOffDeg;
        // Line-2 origin: shifted −½R perp  →  (lng − cosA·lngOff,  lat + sinA·latOff)
        double lat2 = latitude  + sinA * latOffDeg;
        double lng2 = longitude - cosA * lngOffDeg;

        // Normalize longitudes to [-180, 180]: the perpendicular offset can push them
        // outside the valid range (e.g. -186°), which causes GetTRange to find the wrong
        // boundary intersections and collapses the band into a triangle.
        while (lng1 >  180) lng1 -= 360;
        while (lng1 < -180) lng1 += 360;
        while (lng2 >  180) lng2 -= 360;
        while (lng2 < -180) lng2 += 360;

        // Find where line (latO + t·cosA, lngO + t·sinA) exits the map boundary.
        (double tLo, double tHi) GetTRange(double latO, double lngO)
        {
            var tVals = new List<double>();
            if (Math.Abs(cosA) > eps)
            {
                double t = (northLat - latO) / cosA;
                if (Math.Abs(lngO + t * sinA) <= 180 + eps) tVals.Add(t);
                t = (southLat - latO) / cosA;
                if (Math.Abs(lngO + t * sinA) <= 180 + eps) tVals.Add(t);
            }
            if (Math.Abs(sinA) > eps)
            {
                double t = ( 180.0 - lngO) / sinA;
                double y = latO + t * cosA;
                if (y >= southLat - eps && y <= northLat + eps) tVals.Add(t);
                t = (-180.0 - lngO) / sinA;
                y = latO + t * cosA;
                if (y >= southLat - eps && y <= northLat + eps) tVals.Add(t);
            }
            tVals.Sort();
            return tVals.Count >= 2 ? (tVals[0], tVals[^1]) : (0.0, 0.0);
        }

        var (tLo1, tHi1) = GetTRange(lat1, lng1);
        var (tLo2, tHi2) = GetTRange(lat2, lng2);

        double[] Pt(double lng, double lat) =>
            [Math.Clamp(lng, -180, 180), Math.Clamp(lat, southLat, northLat)];

        double[] pLo1 = Pt(lng1 + tLo1 * sinA, lat1 + tLo1 * cosA);
        double[] pHi1 = Pt(lng1 + tHi1 * sinA, lat1 + tHi1 * cosA);
        double[] pLo2 = Pt(lng2 + tLo2 * sinA, lat2 + tLo2 * cosA);
        double[] pHi2 = Pt(lng2 + tHi2 * sinA, lat2 + tHi2 * cosA);

        double BoundaryS(double[] p) =>
            Math.Abs(p[1] - northLat) < eps ? (p[0] + 180.0) / 360.0 :
            Math.Abs(p[0] - 180.0)    < eps ? 1.0 + (northLat - p[1]) / latRange :
            Math.Abs(p[1] - southLat) < eps ? 2.0 + (180.0 - p[0]) / 360.0 :
                                              3.0 + (p[1] - southLat) / latRange;

        var corners = new (double s, double[] coord)[]
        {
            (0.0, [-180.0, northLat]),
            (1.0, [ 180.0, northLat]),
            (2.0, [ 180.0, southLat]),
            (3.0, [-180.0, southLat])
        };

        // Walk the SHORT way (CW or CCW, whichever arc is smaller) from 'from' to 'to',
        // inserting any map corners that lie on that arc.
        void AddBoundaryShort(List<double[]> poly, double[] from, double[] to)
        {
            double sFrom   = BoundaryS(from);
            double sTo     = BoundaryS(to);
            double spanCW  = (sTo   - sFrom + 4.0) % 4.0;   // clockwise  (increasing s)
            double spanCCW = (sFrom - sTo   + 4.0) % 4.0;   // counter-CW (decreasing s)

            if (spanCW <= spanCCW)
            {
                // Short way is clockwise (increasing s)
                foreach (var (s, coord) in corners.OrderBy(c => (c.s - sFrom + 4.0) % 4.0))
                {
                    double dist = (s - sFrom + 4.0) % 4.0;
                    if (dist > eps && dist < spanCW - eps)
                        poly.Add(coord);
                }
            }
            else
            {
                // Short way is counter-clockwise (decreasing s)
                foreach (var (s, coord) in corners.OrderBy(c => (sFrom - c.s + 4.0) % 4.0))
                {
                    double dist = (sFrom - s + 4.0) % 4.0;
                    if (dist > eps && dist < spanCCW - eps)
                        poly.Add(coord);
                }
            }
            poly.Add(to);
        }

        var polygon = new List<double[]>(segments * 2 + 16);

        // Edge 1 – line-1 from pLo1 → pHi1 (subdivided for Mercator accuracy)
        for (int i = 0; i <= segments; i++)
        {
            double t = tLo1 + (tHi1 - tLo1) * i / segments;
            polygon.Add(Pt(lng1 + t * sinA, lat1 + t * cosA));
        }

        // Edge 2 – boundary (short way) from pHi1 → pHi2
        AddBoundaryShort(polygon, pHi1, pHi2);

        // Edge 3 – line-2 from pHi2 → pLo2 (reverse direction)
        for (int i = segments; i >= 0; i--)
        {
            double t = tLo2 + (tHi2 - tLo2) * i / segments;
            polygon.Add(Pt(lng2 + t * sinA, lat2 + t * cosA));
        }

        // Edge 4 – boundary (short way) from pLo2 → pLo1
        AddBoundaryShort(polygon, pLo2, pLo1);

        polygon.Add(polygon[0]); // close
        return polygon;
    }


    public static List<double[]> GenerateRectanclePolygon(double latitude, double longitude, double angle)
    {
        double angleRad = angle * Math.PI / 180.0;
        double sinA = Math.Sin(angleRad);
        double cosA = Math.Cos(angleRad);
        const double eps      = 1e-9;
        const double northLat =  85.051129; // Web Mercator north limit
        const double southLat = -85.051129; // Web Mercator south limit
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

        return polygon;
    }
}