using JetLag.Scripts.Data.Gtfs;
using JetLag.Scripts.Models;

namespace JetLag.Scripts.Data;

public class PlanningState
{
    public StopCoordinate? SelectedStop { get; private set; }
    public IReadOnlyList<TrainDeparture> Departures { get; private set; } = Array.Empty<TrainDeparture>();
    public string? SelectedTripId { get; private set; }
    public bool IsDeparturesPanelVisible => SelectedStop is not null;

    public event Action? OnChange;

    public void SelectStop(StopCoordinate stop, IReadOnlyList<TrainDeparture> departures)
    {
        SelectedStop = stop;
        Departures = departures;
        SelectedTripId = null;
        NotifyStateChanged();
    }

    public void SelectTrip(string tripId)
    {
        SelectedTripId = tripId;
        NotifyStateChanged();
    }

    public void Dismiss()
    {
        SelectedStop = null;
        Departures = Array.Empty<TrainDeparture>();
        SelectedTripId = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
