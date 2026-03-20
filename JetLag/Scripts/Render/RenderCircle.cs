namespace JetLag.Scripts.Render;

using JetLag.Scripts.Data;
using JetLag.Scripts.Utility;
using Community.Blazor.MapLibre;
using Community.Blazor.MapLibre.Models.Sources;
using Community.Blazor.MapLibre.Models.Layers;
using Community.Blazor.MapLibre.Models.Feature;


/// <summary>
/// Renders a circle on the map as a GeoJSON polygon.
/// </summary>
public class CircleRender : IRender<MapLibre, CircleRenderData>
{
    /// <summary>
    /// Renders a geographic circle on the map by adding a GeoJSON polygon source and fill layer.
    /// </summary>
    public async Task Render(MapLibre map, CircleRenderData data)
    {
        var circlePoints = PolygonUtility.GenerateCirclePolygon(
            data.Latitude,
            data.Longitude,
            data.Radius,
            points: 64);

        // GeoJSON coordinates are [lng, lat]; PolygonUtility returns [lat, lng].
        // The ring is already closed by GenerateCirclePolygon (i=0 to i=points).
        var coordinates = circlePoints
            .Select(p => new double[] { p[1], p[0] })
            .ToArray();

        var id = Guid.NewGuid().ToString("N");
        var sourceId = $"circle-source-{id}";
        var layerId = $"circle-layer-{id}";

        var source = new GeoJsonSource
        {
            Data = new FeatureCollection
            {
                Features = new List<IFeature>
                {
                    new FeatureFeature
                    {
                        Geometry = new PolygonGeometry
                        {
                            Coordinates = new double[][][] { coordinates }
                        }
                    }
                }
            }
        };

        await map.AddSource(sourceId, source);
        await map.AddLayer(new FillLayer
        {
            Id = layerId,
            Source = sourceId,
            Paint = new FillLayerPaint
            {
                FillColor = data.FillColor,
                FillOpacity = data.FillOpacity,
                FillOutlineColor = data.Color
            }
        });
    }

    public Task Update(MapLibre map, CircleRenderData updateOptions)
    {
        throw new NotImplementedException();
    }

    public Task Remove(MapLibre map)
    {
        throw new NotImplementedException();
    }
}
