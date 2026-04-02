using JetLag.Scripts.Models;


namespace JetLag.Scripts.Mechanics.Hider;

public class HiderProxy : IHiderProxy
{
    private readonly HttpClient _httpClient;


    public HiderProxy(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<bool> CircleHitHider(double latitude, double longitude, int radiusMeters)
    {
        var request = new RadarRequest(latitude, longitude, radiusMeters);
        var response = await _httpClient.PostAsJsonAsync("api/radar/hit", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<bool>();
    }

    public async Task<bool> LineHitHider(double latitude, double longitude, double angle)
    {
        var request = new ThermometerRequest(latitude, longitude, angle);
        var response = await _httpClient.PostAsJsonAsync("api/thermometer/hit", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<bool>();
    }
}
