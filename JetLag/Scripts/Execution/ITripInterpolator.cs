using JetLag.Scripts.Models;

namespace JetLag.Scripts.Execution;

public interface ITripInterpolator
{
    (double Lat, double Lon) Interpolate(IReadOnlyList<TripStopTime> stops, TimeSpan currentTime);
}
