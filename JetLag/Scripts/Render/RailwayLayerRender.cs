using Community.Blazor.MapLibre;
using Community.Blazor.MapLibre.Models.Feature;
using Community.Blazor.MapLibre.Models.Layers;
using Community.Blazor.MapLibre.Models.Sources;
using System.Text.Json;

namespace JetLag.Scripts.Render;

public class RailwayLayerRender
{
    private const string SourceId = "railway-tracks-source";
    private const string LayerId = "railway-tracks-layer";

    private readonly string _geojsonPath;


    public RailwayLayerRender(IWebHostEnvironment env)
    {
        _geojsonPath = Path.Combine(env.WebRootPath, "data", "railway-tracks.geojson");
    }


    public async Task InitializeAsync(MapLibre map)
    {
        if (!File.Exists(_geojsonPath))
            return;

        var json = await File.ReadAllTextAsync(_geojsonPath);
        var featureCollection = JsonSerializer.Deserialize<FeatureCollection>(json);
        if (featureCollection is null)
            return;

        await map.AddSource(SourceId, new GeoJsonSource { Data = featureCollection });

        await map.AddLayer(new LineLayer
        {
            Id = LayerId,
            Source = SourceId,
            Paint = new LineLayerPaint
            {
                LineColor = "#ca0000",
                LineWidth = 2,
                LineOpacity = 0.8
            }
        });
    }
}
