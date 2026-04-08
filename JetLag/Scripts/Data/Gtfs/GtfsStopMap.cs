using CsvHelper.Configuration;

namespace JetLag.Scripts.Data.Gtfs;

public class GtfsStopMap : ClassMap<GtfsStop>
{
    public GtfsStopMap()
    {
        Map(m => m.StopId).Name("stop_id");
        Map(m => m.StopName).Name("stop_name");
        Map(m => m.StopLat).Name("stop_lat");
        Map(m => m.StopLon).Name("stop_lon");
        Map(m => m.LocationType).Name("location_type").Optional();
        Map(m => m.ParentStation).Name("parent_station").Optional();
    }
}
