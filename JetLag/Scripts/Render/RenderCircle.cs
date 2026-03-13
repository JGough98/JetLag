namespace JetLag.Scripts.Render;

using JetLag.Scripts.Data;
using JetLag.Scripts.Utility;
using LeafletForBlazor;


/// <summary>
/// Renders a circle on the map.
/// </summary>
public class RenderCircle : IMapRender<RealTimeMap.MapEventArgs, CircleRenderData>
{
    /// <summary>
    /// The intial render of the circle on the map.
    /// </summary>
    /// <param name="args">The map event arguments.</param>
    /// <param name="data">The circle render data.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Render(RealTimeMap.MapEventArgs args, CircleRenderData data)
    {
        var circlePoints = PolygonUtility.GenerateCirclePolygon(
            data.Latitude,
            data.Longitude,
            data.Radius,
            points : 64);
        
        await args.sender.Geometric.DisplayPolygonsFromArray.add(
            circlePoints,
            new RealTimeMap.PolygonSymbol()
            {
                color = data.Color,
                fillColor = data.FillColor,
                weight = data.Weight,
                opacity = data.Opacity,
                fillOpacity = data.FillOpacity
            });
    }

    public Task Update(RealTimeMap.MapEventArgs args, CircleRenderData updateOptions)
    {
        throw new NotImplementedException();
    }

    public Task Remove(RealTimeMap.MapEventArgs args)
    {
        throw new NotImplementedException();
    }
}