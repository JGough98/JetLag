using JetLag.Scripts.Data;

namespace JetLag.Scripts;


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
}