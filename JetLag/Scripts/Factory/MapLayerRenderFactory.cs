using JetLag.Scripts.Factory.Interface;
using JetLag.Scripts.Geomitry;
using JetLag.Scripts.Render;

namespace JetLag.Scripts.Factory;


public class MapLayerRenderFactory : IFactory<IMapLayerRender>
{
    private readonly IGeomitryCombinder _geomitryCombinder;


    public MapLayerRenderFactory(IGeomitryCombinder geomitryCombinder)
    {
        _geomitryCombinder = geomitryCombinder;
    }


    public IMapLayerRender Create()
    {
        var sessionID = Guid.NewGuid();

        return new MapLayerRender(
            geomitryCombinder : _geomitryCombinder,
            sourceId : $"source-id-{sessionID}",
            layerId : $"layer-id-{sessionID}",
            color: "#FF0000",
            opacity: 0.2f
        );
    }
}