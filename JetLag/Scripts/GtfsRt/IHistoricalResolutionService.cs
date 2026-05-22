using JetLag.Scripts.Models;

namespace JetLag.Scripts.GtfsRt;

public interface IHistoricalResolutionService
{
    Task<TripOutcome?> ResolveAsync(string tripId, string stopId, int scheduledArrivalSeconds);
}
