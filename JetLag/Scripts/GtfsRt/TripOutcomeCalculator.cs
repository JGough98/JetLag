using JetLag.Scripts.Models;
using TransitRealtime;

namespace JetLag.Scripts.GtfsRt;

public class TripOutcomeCalculator
{
    public TripOutcome Calculate(TripUpdate tripUpdate, string targetStopId, int scheduledArrivalSeconds) =>
        Calculate(tripUpdate, targetStopId, scheduledArrivalSeconds, int.MaxValue);

    public TripOutcome Calculate(TripUpdate tripUpdate, string targetStopId, int scheduledArrivalSeconds, int targetStopSequence)
    {
        var tripId = tripUpdate.Trip.TripId;

        if (tripUpdate.Trip.ScheduleRelationship == TripDescriptor.Types.ScheduleRelationship.Canceled)
            return new TripOutcome(tripId, targetStopId, scheduledArrivalSeconds, scheduledArrivalSeconds, 0, true);

        var updates = tripUpdate.StopTimeUpdate;

        var exactMatch = updates.FirstOrDefault(stu => stu.StopId == targetStopId);
        if (exactMatch is not null)
        {
            var delay = ExtractDelay(exactMatch);
            return new TripOutcome(tripId, targetStopId, scheduledArrivalSeconds, scheduledArrivalSeconds + delay, delay, false);
        }

        if (updates.Count > 0)
        {
            var sorted = updates.OrderBy(stu => stu.StopSequence).ToList();
            var propagated = sorted
                .Where(stu => stu.StopSequence <= targetStopSequence)
                .LastOrDefault()
                ?? sorted.Last();

            var delay = ExtractDelay(propagated);
            return new TripOutcome(tripId, targetStopId, scheduledArrivalSeconds, scheduledArrivalSeconds + delay, delay, false);
        }

        return new TripOutcome(tripId, targetStopId, scheduledArrivalSeconds, scheduledArrivalSeconds, 0, false);
    }

    private static int ExtractDelay(TripUpdate.Types.StopTimeUpdate stu)
    {
        if (stu.Arrival != null && stu.Arrival.HasDelay)
            return stu.Arrival.Delay;
        if (stu.Departure != null && stu.Departure.HasDelay)
            return stu.Departure.Delay;
        return 0;
    }
}
