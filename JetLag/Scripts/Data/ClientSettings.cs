using JetLag.Scripts.Data.Gtfs;
using JetLag.Scripts.Utility;


namespace JetLag.Scripts.Data;

/// <summary>
/// Manages the state of the player.
/// </summary>
public class ClientSettings
{
    private Player? _player;


    public Player Player
    {
        get => _player;
    }

    public string PlayerName
    {
        get => _player.Name;
        set => _player = new Player(StringUtility.Sanitise(value), true);
    }

    public GtfsStop? CurrentStation { get; private set; }
    public string? SelectedTripId { get; private set; }
    public TimeSpan? PlannedArrivalTime { get; private set; }

    public void SetPlan(GtfsStop station, string tripId, TimeSpan arrivalTime)
    {
        CurrentStation = station;
        SelectedTripId = tripId;
        PlannedArrivalTime = arrivalTime;
    }

    public void ClearPlan()
    {
        CurrentStation = null;
        SelectedTripId = null;
        PlannedArrivalTime = null;
    }
}