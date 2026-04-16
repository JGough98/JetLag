namespace JetLag.Scripts.Data.Gtfs;

public class GtfsRoute
{
    public string RouteId { get; set; } = string.Empty;
    public string? RouteShortName { get; set; }
    public string? RouteLongName { get; set; }
    public int RouteType { get; set; }

    public ICollection<GtfsTrip> Trips { get; set; } = new List<GtfsTrip>();
}
