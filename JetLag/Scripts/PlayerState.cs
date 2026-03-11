namespace JetLag.Scripts;


/// <summary>
/// Manages the state of the player.
/// </summary>
public class PlayerState
{
    private string _playerName = string.Empty;


    public string PlayerName
    {
        get => _playerName;
        set => _playerName = StringUtility.Sanitise(value);
    }
}