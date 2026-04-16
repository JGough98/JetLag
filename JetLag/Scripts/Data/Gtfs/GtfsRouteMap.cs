using CsvHelper.Configuration;

namespace JetLag.Scripts.Data.Gtfs;

public class GtfsRouteMap : ClassMap<GtfsRoute>
{
    public GtfsRouteMap()
    {
        Map(m => m.RouteId).Name("route_id");
        Map(m => m.RouteShortName).Name("route_short_name").Optional();
        Map(m => m.RouteLongName).Name("route_long_name").Optional();
        Map(m => m.RouteType).Name("route_type");
    }
}
