using Community.Blazor.MapLibre;
using Community.Blazor.MapLibre.Models.Feature;
using Community.Blazor.MapLibre.Models.Layers;
using Community.Blazor.MapLibre.Models.Sources;
using JetLag.Scripts.Models;
using JetLag.Scripts.Transit;

namespace JetLag.Scripts.Render;

public class StationLayerRender
{
    private const string SourceId = "stations-source";
    private const string LayerId = "stations-layer";
    private const string HighlightSourceId = "station-highlight-source";
    private const string HighlightLayerId = "station-highlight-layer";
    private const double ClickRadiusMetres = 500.0;
    private const double EarthRadius = 6371000.0;

    private readonly ITransitService _transitService;
    private IReadOnlyList<StopCoordinate> _stops = Array.Empty<StopCoordinate>();
    private MapLibre? _map;

    public StationLayerRender(ITransitService transitService)
    {
        _transitService = transitService;
    }

    public IReadOnlyList<StopCoordinate> Stops => _stops;

    public async Task InitializeAsync(MapLibre map)
    {
        _map = map;
        _stops = await _transitService.GetAllStops();

        var features = _stops
            .Select(s => (IFeature) new FeatureFeature
            {
                Geometry = new PointGeometry { Coordinates = new[] { s.Lon, s.Lat } },
                Properties = new Dictionary<string, object>
                {
                    ["stop_id"] = s.StopId,
                    ["stop_name"] = s.StopName
                }
            })
            .ToList();

        await map.AddSource(SourceId, new GeoJsonSource
        {
            Data = new FeatureCollection { Features = features }
        });

        await map.AddLayer(new CircleLayer
        {
            Id = LayerId,
            Source = SourceId,
            Paint = new CircleLayerPaint
            {
                CircleRadius = 5,
                CircleColor = "#ffffff",
                CircleStrokeWidth = 1.5,
                CircleStrokeColor = "#ca0000"
            }
        });

        await map.AddSource(HighlightSourceId, new GeoJsonSource
        {
            Data = new FeatureCollection { Features = new List<IFeature>() }
        });

        await map.AddLayer(new CircleLayer
        {
            Id = HighlightLayerId,
            Source = HighlightSourceId,
            Paint = new CircleLayerPaint
            {
                CircleRadius = 9,
                CircleColor = "#fbbf24",
                CircleStrokeWidth = 2,
                CircleStrokeColor = "#ffffff"
            }
        });
    }

    public async Task HighlightStop(StopCoordinate stop)
    {
        if (_map is null) return;

        await _map.SetSourceData(HighlightSourceId, new GeoJsonSource
        {
            Data = new FeatureCollection
            {
                Features = new List<IFeature>
                {
                    new FeatureFeature
                    {
                        Geometry = new PointGeometry { Coordinates = new[] { stop.Lon, stop.Lat } }
                    }
                }
            }
        });
    }

    public async Task ClearHighlight()
    {
        if (_map is null) return;

        await _map.SetSourceData(HighlightSourceId, new GeoJsonSource
        {
            Data = new FeatureCollection { Features = new List<IFeature>() }
        });
    }

    public StopCoordinate? FindNearestStopWithinRadius(double lat, double lon)
    {
        StopCoordinate? nearest = null;
        double nearestDist = double.MaxValue;

        foreach (var stop in _stops)
        {
            var dist = HaversineMetres(lat, lon, stop.Lat, stop.Lon);
            if (dist < ClickRadiusMetres && dist < nearestDist)
            {
                nearest = stop;
                nearestDist = dist;
            }
        }

        return nearest;
    }

    private static double HaversineMetres(double lat1, double lon1, double lat2, double lon2)
    {
        var dLat = (lat2 - lat1) * Math.PI / 180.0;
        var dLon = (lon2 - lon1) * Math.PI / 180.0;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
              + Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0)
              * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return EarthRadius * 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));
    }
}
