using CsvHelper.Configuration;

namespace JetLag.Scripts.Data.Gtfs;

public class GtfsTripMap : ClassMap<GtfsTrip>
{
    public GtfsTripMap()
    {
        Map(m => m.TripId).Name("trip_id");
        Map(m => m.RouteId).Name("route_id");
        Map(m => m.ServiceId).Name("service_id").Optional();
        Map(m => m.TripHeadsign).Name("trip_headsign").Optional();
        Map(m => m.DirectionId).Name("direction_id").Optional();
    }
}
