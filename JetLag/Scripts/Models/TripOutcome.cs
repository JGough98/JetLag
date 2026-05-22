namespace JetLag.Scripts.Models;

public record TripOutcome(
    string TripId,
    string StopId,
    int ScheduledArrivalSeconds,
    int ActualArrivalSeconds,
    int DelaySeconds,
    bool IsCancelled
);
