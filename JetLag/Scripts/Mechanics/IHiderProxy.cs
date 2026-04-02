namespace JetLag.Scripts.Mechanics;

public interface IHiderProxy
{
    public Task<bool> HitHider(double latitude, double longitude, int radiusMeters);
}
