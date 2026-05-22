using JetLag.Scripts.Data.Gtfs;
using JetLag.Scripts.Models;

namespace JetLag.Scripts.Transit;

public interface ITransitService
{
    Task<IReadOnlyList<TrainDeparture>> GetDepartingTrains(string stopId, TimeSpan from, TimeSpan to);
    Task<IReadOnlyList<StopCoordinate>> GetAllStops();
    Task<IReadOnlyList<StopCoordinate>> GetStopsForTrip(string tripId);
    Task<IReadOnlyList<TripStopTime>> GetStopTimesForTrip(string tripId);
}
