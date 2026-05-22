using JetLag.Scripts.Models;

namespace JetLag.Scripts.Execution;

public class TripInterpolator : ITripInterpolator
{
    public (double Lat, double Lon) Interpolate(IReadOnlyList<TripStopTime> stops, TimeSpan currentTime)
    {
        if (stops.Count == 0) return (0, 0);
        if (stops.Count == 1) return (stops[0].Lat, stops[0].Lon);

        int currentSeconds = (int)currentTime.TotalSeconds;

        if (currentSeconds <= stops[0].ArrivalSeconds) return (stops[0].Lat, stops[0].Lon);
        if (currentSeconds >= stops[^1].ArrivalSeconds) return (stops[^1].Lat, stops[^1].Lon);

        int i = 0;
        for (; i < stops.Count - 2; i++)
            if (stops[i + 1].ArrivalSeconds > currentSeconds) break;

        double segmentDuration = stops[i + 1].ArrivalSeconds - stops[i].ArrivalSeconds;
        double elapsed = currentSeconds - stops[i].ArrivalSeconds;
        double t = segmentDuration == 0 ? 0.0 : Math.Clamp(elapsed / segmentDuration, 0.0, 1.0);

        return (
            stops[i].Lat + t * (stops[i + 1].Lat - stops[i].Lat),
            stops[i].Lon + t * (stops[i + 1].Lon - stops[i].Lon)
        );
    }
}
