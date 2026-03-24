namespace JetLag.Scripts.Render;

using Community.Blazor.MapLibre;
using Community.Blazor.MapLibre.Models.Sources;
using Community.Blazor.MapLibre.Models.Layers;
using Community.Blazor.MapLibre.Models.Feature;


public class MapLayerRender : IMapLayerRender
{
    private List<IFeature> _mapFeatures = new List<IFeature>();

    private readonly string _sourceId;
    private readonly string _layerId;
    private readonly string _color;

    private readonly double _opacity;

    private readonly double[][] _worldBounds;

    public MapLayerRender(
        string sourceId,
        string layerId,
        string color,
        double opacity,
        double[][] worldBounds
    )
    {
        _sourceId = sourceId;
        _layerId = layerId;
        _color = color;
        _opacity = opacity;
        _worldBounds = worldBounds;
    }

    public async Task Clear(MapLibre map)
    {
        await map.RemoveLayer(_layerId);
        await map.RemoveSource(_sourceId);

        _mapFeatures = new List<IFeature>();
    }

    public async Task Add(double[][] newCoordinates, MapLibre map) =>
        await AddFeature(
            new FeatureFeature
            {
                Geometry = new PolygonGeometry { Coordinates = new double[][][] { newCoordinates } }
            },
            map
        );

    public async Task AddInverted(double[][] newCoordinates, MapLibre map) =>
        await AddFeature(
            new FeatureFeature
            {
                Geometry = new PolygonGeometry
                {
                    Coordinates = new double[][][] { _worldBounds, newCoordinates }
                }
            },
            map
        );

    private async Task AddFeature(FeatureFeature newFeature, MapLibre map)
    {
        _mapFeatures.Add(newFeature);

        if (_mapFeatures.Count == 1)
            await InitializeMapLayer(map);
        else
            await RefreshMapData(map);
    }

    private async Task InitializeMapLayer(MapLibre map)
    {
        await map.AddSource(
            _sourceId,
            new GeoJsonSource { Data = new FeatureCollection { Features = _mapFeatures } }
        );

        await map.AddLayer(
            new FillLayer
            {
                Id = _layerId,
                Source = _sourceId,
                Paint = new FillLayerPaint { FillColor = _color, FillOpacity = _opacity }
            }
        );
    }

    private async Task RefreshMapData(MapLibre map)
    {
        await map.SetSourceData(
            _sourceId,
            new GeoJsonSource { Data = new FeatureCollection { Features = _mapFeatures } }
        );
    }
}
