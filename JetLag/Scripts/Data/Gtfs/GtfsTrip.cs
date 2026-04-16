namespace JetLag.Scripts.Data.Gtfs;

public class GtfsTrip
{
    public string TripId { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public string? ServiceId { get; set; }
    public string? TripHeadsign { get; set; }
    public string? DirectionId { get; set; }

    public GtfsRoute Route { get; set; } = null!;
    public ICollection<GtfsStopTime> StopTimes { get; set; } = new List<GtfsStopTime>();
}
