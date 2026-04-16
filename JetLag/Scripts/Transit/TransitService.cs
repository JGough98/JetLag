using JetLag.Scripts.Data;
using JetLag.Scripts.Data.Gtfs;
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
}
