namespace JetLag.Scripts.Data.Gtfs;

public record TrainDeparture(
    string TripId,
    string RouteId,
    string? RouteShortName,
    string? TripHeadsign,
    TimeSpan DepartureTime
);
