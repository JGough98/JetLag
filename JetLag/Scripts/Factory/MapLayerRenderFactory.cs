using JetLag.Scripts.Render;

namespace JetLag.Scripts.Factory;


public class MapLayerRenderFactory : IFactory<IMapLayerRender>
{
    public IMapLayerRender Create()
    {
        var sessionID = Guid.NewGuid();

        return new MapLayerRender(
            $"source-id-{sessionID}",
            $"layer-id-{sessionID}",
            opacity: 0.2f,
            color: "#FF0000",
            worldBounds: new double[][]
            {
                new double[] { -180, -90 },
                new double[] { 180, -90 },
                new double[] { 180, 90 },
                new double[] { -180, 90 },
                new double[] { -180, -90 }
            }
        );
    }
}
