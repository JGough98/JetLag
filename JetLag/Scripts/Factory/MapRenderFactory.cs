using JetLag.Scripts.Factory.Interface;
using JetLag.Scripts.Geomitry;
using JetLag.Scripts.Render;


namespace JetLag.Scripts.Factory;

public class MapRenderFactory : IFactory<MapRender>
{
    private readonly IFactory<IGeomitryCombinder> _geomitryCombinderFactory;


    public MapRenderFactory(IFactory<IGeomitryCombinder> geomitryCombinderFactory)
    {
        _geomitryCombinderFactory = geomitryCombinderFactory;
    }


    public MapRender Create()
    {
        var sessionID = Guid.NewGuid();
        var sourceId = "-sourceID-";
        var layerId = "-layerID-";
        var opacity = 0.3;

        return new MapRender()
        {
            OutOfBounds = CreateMapRender(
                $"{nameof(MapRender.OutOfBounds)}{sourceId}{sessionID}",
                $"{nameof(MapRender.OutOfBounds)}{layerId}{sessionID}",
                "#FF0000",
                opacity
            ),
            Overlay = CreateMapRender(
                $"{nameof(MapRender.Overlay)}{sourceId}{sessionID}",
                $"{nameof(MapRender.Overlay)}{layerId}{sessionID}",
                "#00e5ff",
                opacity
            ),
        };
    }


    private IMapLayerShapeRender CreateMapRender(
        string sourceId,
        string layerId,
        string color,
        double opacity
    ) =>
        new MapLayerShapeRender(
            new MapLayerRender(
                _geomitryCombinderFactory.Create(),
                sourceId,
                layerId,
                color,
                opacity
            )
        );
}
