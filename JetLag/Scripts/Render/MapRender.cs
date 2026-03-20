namespace JetLag.Scripts.Render;

using JetLag.Scripts.Data;
using Community.Blazor.MapLibre;


public class MapRender : IMapRender
{
    private readonly IRender<MapLibre, CircleRenderData> _circleRender;

    private readonly CircleRenderData _circleData;


    public MapRender(IRender<MapLibre, CircleRenderData> circleRender)
    {
        _circleRender = circleRender;

        _circleData = new CircleRenderData()
        {
            Color = "blue",
            FillColor = "blue",
            Weight = 2,
            Opacity = 0.8,
            FillOpacity = 0.2
        };
    }



    public async Task RenderCircle(
        MapLibre map,
        double Latitude,
        double Longitude,
        double Radius)
    {
        _circleData.Latitude = Latitude;
        _circleData.Longitude = Longitude;
        _circleData.Radius = Radius;

        await _circleRender.Render(map, _circleData);
    }
}
