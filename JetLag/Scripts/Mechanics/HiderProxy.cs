using System.Net.Http.Json;
using JetLag.Scripts.Models;


namespace JetLag.Scripts.Mechanics;

public class HiderProxy : IHiderProxy
{
    private readonly HttpClient _httpClient;


    public HiderProxy(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<bool> HitHider(double latitude, double longitude, int radiusMeters)
    {
        var request = new RadarRequest(latitude, longitude, radiusMeters);
        var response = await _httpClient.PostAsJsonAsync("api/radar/hit", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<bool>();
    }
}
