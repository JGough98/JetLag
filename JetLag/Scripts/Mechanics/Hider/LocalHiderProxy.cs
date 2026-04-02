namespace JetLag.Scripts.Mechanics.Hider;

public class LocalHiderProxy : IHiderProxy
{
    private readonly double _hiderLatitude;
    private readonly double _hiderLongitude;


    public LocalHiderProxy()
    {
        var random = new Random();
        _hiderLatitude  = random.NextDouble() * 180 - 90;   // -90  to  90
        _hiderLongitude = random.NextDouble() * 360 - 180;  // -180 to 180
    }


    public Task<bool> CircleHitHider(double latitude, double longitude, int radiusMeters)
    {
        var distance = HaversineDistance(latitude, longitude, _hiderLatitude, _hiderLongitude);
        return Task.FromResult(distance <= radiusMeters);
    }


    private static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double EarthRadius = 6371000; // metres — matches PolygonUtility
        
        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLon = (lon2 - lon1) * Math.PI / 180;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
              + Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180)
              * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        
        return EarthRadius * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }
}
