using Community.Blazor.MapLibre;
using Community.Blazor.MapLibre.Models.Sources;
using Community.Blazor.MapLibre.Models.Layers;
using Community.Blazor.MapLibre.Models.Feature;
using JetLag.Scripts.Geometry;


namespace JetLag.Scripts.Render;

public class MapLayerRender : IMapLayerRender
{
    private readonly IGeometryCombinder _geomitryCombinder;

    private readonly Action<double[][]> _addCoordinatesDelagate;
    private readonly Action<double[][]> _addInvertedCoordinatesDelagate;

    private readonly string _sourceId;
    private readonly string _layerId;
    private readonly string _color;

    private readonly double _opacity;

    private bool _layerInitialized = false;

    private readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);


    public MapLayerRender(
        IGeometryCombinder geomitryCombinder,
        string sourceId,
        string layerId,
        string color,
        double opacity
    )
    {
        _geomitryCombinder = geomitryCombinder;
        _addCoordinatesDelagate = x => _geomitryCombinder.Add(x);
        _addInvertedCoordinatesDelagate = x => _geomitryCombinder.AddInverted(x);
        _sourceId = sourceId;
        _layerId = layerId;
        _color = color;
        _opacity = opacity;
    }


    public async Task Draw(double[][] newCoordinates, MapLibre map) =>
        await AddFeature(newCoordinates, map, _addCoordinatesDelagate);

    public async Task InvertDraw(double[][] newCoordinates, MapLibre map) =>
        await AddFeature(newCoordinates, map, _addInvertedCoordinatesDelagate);

    public async Task Replace(double[][] newCoordinates, MapLibre map)
    {
        await _initLock.WaitAsync();
        try
        {
            _geomitryCombinder.Reset();
            _addCoordinatesDelagate(newCoordinates);
            if (_layerInitialized)
                await RefreshMapData(GetGeoJsonSource(), map);
            else
                await InitializeMapLayer(GetGeoJsonSource(), map);
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task Clear(MapLibre map)
    {
        await _initLock.WaitAsync();
        try
        {
            await map.RemoveLayer(_layerId);
            await map.RemoveSource(_sourceId);
            _geomitryCombinder.Reset();
            _layerInitialized = false;
        }
        finally
        {
            _initLock.Release();
        }
    }


    private async Task AddFeature(
        double[][] newCoordinates,
        MapLibre map,
        Action<double[][]> addDelgate
    )
    {
        await _initLock.WaitAsync();
        try
        {
            var inUse = _geomitryCombinder.InUse;
            addDelgate(newCoordinates);
            if (inUse)
                await RefreshMapData(GetGeoJsonSource(), map);
            else
                await InitializeMapLayer(GetGeoJsonSource(), map);
        }
        finally
        {
            _initLock.Release();
        }
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

        _layerInitialized = true;
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
