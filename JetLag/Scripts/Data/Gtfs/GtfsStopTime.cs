namespace JetLag.Scripts.Data.Gtfs;

public class GtfsStopTime
{
    public int Id { get; set; }
    public string TripId { get; set; } = string.Empty;
    public string StopId { get; set; } = string.Empty;
    public int StopSequence { get; set; }
    public int ArrivalSeconds { get; set; }
    public int DepartureSeconds { get; set; }

    public GtfsTrip Trip { get; set; } = null!;
    public GtfsStop Stop { get; set; } = null!;
}
