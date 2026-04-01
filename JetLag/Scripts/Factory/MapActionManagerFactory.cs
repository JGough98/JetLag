using JetLag.Scripts.Factory.Interface;
using JetLag.Scripts.Mechanics;
using JetLag.Scripts.Render;


namespace JetLag.Scripts;

public class MapActionManagerFactory : IFactory<IMapActionManager>
{
    private MapRender _mapRender;


    public MapActionManagerFactory(MapRender mapRender)
    {
        _mapRender = mapRender;
    }


    public IMapActionManager Create() =>
        new MapActionManager(new List<IMapAction>() { new RadarMapAction(_mapRender) });
}
