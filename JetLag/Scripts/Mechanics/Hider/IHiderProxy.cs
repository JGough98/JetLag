namespace JetLag.Scripts.Mechanics.Hider;

public interface IHiderProxy
{
    public Task<bool> CircleHitHider(double latitude, double longitude, int radiusMeters);

    /// <summary>
    /// Returns true if the hider is on the "drawn" side of the line through
    /// (latitude, longitude) at the given bearing angle (degrees clockwise from north).
    /// </summary>
    public Task<bool> LineHitHider(double latitude, double longitude, double angle);
}
