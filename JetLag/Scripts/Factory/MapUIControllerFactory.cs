using JetLag.Scripts.Factory.Interface;
using JetLag.Scripts.Mechanics;
using JetLag.Scripts.Render;

namespace JetLag.Scripts;

public class MapUIControllerFactory : IFactory<MapUIController>
{
    private MapRender _mapRender;


    public MapUIControllerFactory(MapRender mapRender)
    {
        _mapRender = mapRender;
    }


    public MapUIController Create() =>
        new MapUIController(new List<IMapAction>() { new RadarMapAction(_mapRender) });
}
