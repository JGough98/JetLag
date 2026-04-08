namespace JetLag.Scripts.Data.Gtfs;

public class GtfsStop
{
    public string StopId { get; set; } = string.Empty;
    public string StopName { get; set; } = string.Empty;
    public double StopLat { get; set; }
    public double StopLon { get; set; }
    public string? LocationType { get; set; }
    public string? ParentStation { get; set; }

    public ICollection<GtfsStopTime> StopTimes { get; set; } = new List<GtfsStopTime>();
}
