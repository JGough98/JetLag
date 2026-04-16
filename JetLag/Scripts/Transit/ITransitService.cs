using JetLag.Scripts.Data.Gtfs;

namespace JetLag.Scripts.Transit;

public interface ITransitService
{
    Task<IReadOnlyList<TrainDeparture>> GetDepartingTrains(string stopId, TimeSpan from, TimeSpan to);
}
