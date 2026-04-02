namespace JetLag.Scripts.Mechanics.Hider;

public interface IHiderProxy
{
    public Task<bool> CircleHitHider(double latitude, double longitude, int radiusMeters);
}
