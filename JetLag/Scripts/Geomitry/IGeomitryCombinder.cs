namespace JetLag.Scripts.Geomitry;


public interface IGeomitryCombinder
{
    public double[][][] GetGeometryCoordinates();

    public void Add(double[][] coordinates);

    public void AddInverted(double[][] coordinates, double[][] worldBounds);
}