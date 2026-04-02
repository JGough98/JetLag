namespace JetLag.Scripts.Models;

public record RadarRequest(
    double Latitude,
    double Longitude,
    int RadiusMeters
);
