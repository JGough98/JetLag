namespace JetLag.Scripts.Data;


/// <summary>
/// Represents the data for a circle render.
/// </summary>
public class CircleRenderData
{
    public double Latitude { get; set; } = 0;
    public double Longitude { get; set; } = 0;
    public double Radius { get; set; } = 500;
    public string Color { get; set; } = "blue";
    public string FillColor { get; set; } = "blue";
    public int Weight { get; set; } = 2;
    public double Opacity { get; set; } = 0.8;
    public double FillOpacity { get; set; } = 0.2;
}
