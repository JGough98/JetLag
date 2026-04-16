using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace JetLag.Scripts.Data.Gtfs;

public class GtfsStopTimeMap : ClassMap<GtfsStopTime>
{
    public GtfsStopTimeMap()
    {
        Map(m => m.TripId).Name("trip_id");
        Map(m => m.StopId).Name("stop_id");
        Map(m => m.StopSequence).Name("stop_sequence");
        Map(m => m.ArrivalSeconds).Name("arrival_time").TypeConverter<GtfsTimeConverter>();
        Map(m => m.DepartureSeconds).Name("departure_time").TypeConverter<GtfsTimeConverter>();
    }
}

public class GtfsTimeConverter : DefaultTypeConverter
{
    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        var parts = text.Split(':');
        if (parts.Length != 3)
            return 0;

        return int.Parse(parts[0]) * 3600
             + int.Parse(parts[1]) * 60
             + int.Parse(parts[2]);
    }
}
