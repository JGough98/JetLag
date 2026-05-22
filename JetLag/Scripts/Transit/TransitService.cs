using JetLag.Scripts.Data;
using JetLag.Scripts.Data.Gtfs;
using JetLag.Scripts.Models;
using Microsoft.EntityFrameworkCore;

namespace JetLag.Scripts.Transit;

public class TransitService : ITransitService
{
    private readonly GtfsDbContext _db;

    public TransitService(GtfsDbContext db) => _db = db;

    public async Task<IReadOnlyList<TrainDeparture>> GetDepartingTrains(
        string stopId,
        TimeSpan from,
        TimeSpan to
    )
    {
        var fromSec = (int)from.TotalSeconds;
        var toSec = (int)to.TotalSeconds;

        return await _db.StopTimes
            .Where(st =>
                st.StopId == stopId &&
                st.DepartureSeconds >= fromSec &&
                st.DepartureSeconds <= toSec)
            .Include(st => st.Trip)
                .ThenInclude(t => t.Route)
            .OrderBy(st => st.DepartureSeconds)
            .Select(st => new TrainDeparture(
                st.TripId,
                st.Trip.RouteId,
                st.Trip.Route.RouteShortName,
                st.Trip.TripHeadsign,
                TimeSpan.FromSeconds(st.DepartureSeconds)
            ))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<StopCoordinate>> GetAllStops() =>
        await _db.Stops
            .Select(s => new StopCoordinate(s.StopId, s.StopName, s.StopLat, s.StopLon))
            .ToListAsync();

    public async Task<IReadOnlyList<StopCoordinate>> GetStopsForTrip(string tripId) =>
        await _db.StopTimes
            .Where(st => st.TripId == tripId)
            .Include(st => st.Stop)
            .OrderBy(st => st.StopSequence)
            .Select(st => new StopCoordinate(st.Stop.StopId, st.Stop.StopName, st.Stop.StopLat, st.Stop.StopLon))
            .ToListAsync();

    public async Task<IReadOnlyList<TripStopTime>> GetStopTimesForTrip(string tripId) =>
        await _db.StopTimes
            .Where(st => st.TripId == tripId)
            .Include(st => st.Stop)
            .OrderBy(st => st.StopSequence)
            .Select(st => new TripStopTime(st.Stop.StopId, st.Stop.StopName, st.Stop.StopLat, st.Stop.StopLon, st.ArrivalSeconds))
            .ToListAsync();
}
