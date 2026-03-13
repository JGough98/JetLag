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
}