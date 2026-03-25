namespace JetLag.Scripts.Render;

using Community.Blazor.MapLibre;
using Community.Blazor.MapLibre.Models.Sources;
using Community.Blazor.MapLibre.Models.Layers;
using Community.Blazor.MapLibre.Models.Feature;
using JetLag.Scripts.Geomitry;


public class MapLayerRender : IMapLayerRender
{
    private IGeomitryCombinder _geomitryCombinder;

    private readonly string _sourceId;
    private readonly string _layerId;
    private readonly string _color;

    private readonly double _opacity;

    private readonly double[][] _worldBounds;

    private bool _inUse;


    public MapLayerRender(
        IGeomitryCombinder geomitryCombinder,
        string sourceId,
        string layerId,
        string color,
        double opacity,
        double[][] worldBounds
    )
    {
        _geomitryCombinder = geomitryCombinder;
        _sourceId = sourceId;
        _layerId = layerId;
        _color = color;
        _opacity = opacity;
        _worldBounds = worldBounds;
        _inUse = false;
    }


    public async Task Add(double[][] newCoordinates, MapLibre map)
    {
        _geomitryCombinder.Add(newCoordinates);
        await AddFeature(map);
    }

    public async Task AddInverted(double[][] newCoordinates, MapLibre map)
    {
        _geomitryCombinder.AddInverted(newCoordinates, _worldBounds);
        await AddFeature(map);
    }

    public async Task Clear(MapLibre map)
    {
        _inUse = false;

        await map.RemoveLayer(_layerId);
        await map.RemoveSource(_sourceId);
    }


    private async Task AddFeature(MapLibre map)
    {
        if (_inUse)
            await RefreshMapData(GetGeoJsonSource(), map);
        else
            await InitializeMapLayer(GetGeoJsonSource(), map);

        _inUse = true;
    }

    private async Task InitializeMapLayer(GeoJsonSource newFeature, MapLibre map)
    {
        await map.AddSource(_sourceId, newFeature);

        await map.AddLayer(
            new FillLayer
            {
                Id = _layerId,
                Source = _sourceId,
                Paint = new FillLayerPaint { FillColor = _color, FillOpacity = _opacity }
            }
        );
    }

    private async Task RefreshMapData(GeoJsonSource newFeature, MapLibre map)
    {
        await map.SetSourceData(_sourceId, newFeature);
    }

    private GeoJsonSource GetGeoJsonSource() =>
        new GeoJsonSource
        {
            Data = new FeatureCollection
            {
                Features = new List<IFeature>()
                {
                    new FeatureFeature
                    {
                        Geometry = new MultiPolygonGeometry
                        {
                            Coordinates = _geomitryCombinder.GetGeometryCoordinates()
                        }
                    }
                }
            }
        };
}
