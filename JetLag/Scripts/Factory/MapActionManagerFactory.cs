using JetLag.Scripts.Factory.Interface;
using JetLag.Scripts.Mechanics.Hider;
using JetLag.Scripts.Mechanics.MapAction;
using JetLag.Scripts.Render;


namespace JetLag.Scripts;

public class MapActionManagerFactory : IFactory<IMapActionManager>
{
    private readonly MapRender _mapRender;

    private readonly IHiderProxy _hiderProxy;


    public MapActionManagerFactory(MapRender mapRender, IHiderProxy hiderProxy)
    {
        _mapRender = mapRender;
        _hiderProxy = hiderProxy;
    }


    public IMapActionManager Create() =>
        new MapActionManager(
            new List<IMapAction>()
            {
                new RadarMapAction(_mapRender, _hiderProxy),
                new ThermometerMapAction(_mapRender, _hiderProxy)
            }
        );
}
