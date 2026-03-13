namespace JetLag.Scripts.Render;

using JetLag.Scripts.Data;
using LeafletForBlazor;


public class MapRender : IMapRender
{
    private readonly IRender<RealTimeMap.MapEventArgs, CircleRenderData> _circleRender;

    private readonly CircleRenderData _circleData = new();


    public MapRender(IRender<RealTimeMap.MapEventArgs, CircleRenderData> circleRender)
    {
        _circleRender = circleRender;

        _circleData = new CircleRenderData()
        {
            Latitude = 0,
            Longitude = 0,
            Radius = 500,
            Color = "blue",
            FillColor = "blue",
            Weight = 2,
            Opacity = 0.8,
            FillOpacity = 0.2
        };
    }



    public async Task RenderCircle(
        RealTimeMap.MapEventArgs args,
        double Latitude,
        double Longitude,
        double Radius)
    {
        _circleData.Latitude = Latitude;
        _circleData.Longitude = Longitude;
        _circleData.Radius = Radius;

        await _circleRender.Render(args, _circleData);
    }
}