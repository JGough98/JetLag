using Community.Blazor.MapLibre;
using Community.Blazor.MapLibre.Models;
using Community.Blazor.MapLibre.Models.Marker;

namespace JetLag.Scripts.Render;

public class PlayerMarkerRender
{
    public Guid? MarkerId { get; private set; }

    public async Task<Guid> PlaceAsync(MapLibre map, double lat, double lon)
    {
        if (MarkerId is { } existing)
            await map.RemoveMarker(existing);

        var id = await map.AddMarker(
            new MarkerOptions { Color = "#22d3ee", Scale = 0.8f },
            new LngLat(lon, lat));

        MarkerId = id;
        return id;
    }

    public async Task MoveAsync(MapLibre map, double lat, double lon)
    {
        if (MarkerId is not { } id) return;
        await map.MoveMarker(id, new LngLat(lon, lat));
    }

    public async Task RemoveAsync(MapLibre map)
    {
        if (MarkerId is not { } id) return;
        await map.RemoveMarker(id);
        MarkerId = null;
    }
}
