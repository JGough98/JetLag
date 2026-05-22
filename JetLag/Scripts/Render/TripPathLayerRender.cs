using Community.Blazor.MapLibre;
using Community.Blazor.MapLibre.Models.Feature;
using Community.Blazor.MapLibre.Models.Layers;
using Community.Blazor.MapLibre.Models.Sources;
using JetLag.Scripts.Transit;

namespace JetLag.Scripts.Render;

public class TripPathLayerRender
{
    private const string SourceId = "trip-path-source";
    private const string LayerId = "trip-path-layer";

    private readonly ITransitService _transitService;
    private MapLibre? _map;
    private bool _layerInitialized;

    public TripPathLayerRender(ITransitService transitService)
    {
        _transitService = transitService;
    }

    public void Initialize(MapLibre map)
    {
        _map = map;
    }

    public async Task DrawTripPath(string tripId)
    {
        if (_map is null) return;

        var stops = await _transitService.GetStopsForTrip(tripId);
        if (stops.Count < 2) return;

        var coordinates = stops.Select(s => new[] { s.Lon, s.Lat }).ToArray();
        var source = BuildSource(coordinates);

        if (_layerInitialized)
        {
            await _map.SetSourceData(SourceId, source);
        }
        else
        {
            await _map.AddSource(SourceId, source);
            await _map.AddLayer(new LineLayer
            {
                Id = LayerId,
                Source = SourceId,
                Paint = new LineLayerPaint
                {
                    LineColor = "#fbbf24",
                    LineWidth = 3,
                    LineOpacity = 0.9
                }
            });
            _layerInitialized = true;
        }
    }

    public async Task Clear()
    {
        if (_map is null || !_layerInitialized) return;

        await _map.RemoveLayer(LayerId);
        await _map.RemoveSource(SourceId);
        _layerInitialized = false;
    }

    private static GeoJsonSource BuildSource(double[][] coordinates) =>
        new GeoJsonSource
        {
            Data = new FeatureCollection
            {
                Features = new List<IFeature>
                {
                    new FeatureFeature
                    {
                        Geometry = new LineGeometry { Coordinates = coordinates }
                    }
                }
            }
        };
}
