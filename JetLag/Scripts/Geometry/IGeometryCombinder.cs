namespace JetLag.Scripts.Geometry;


public interface IGeometryCombinder
{
    public bool InUse { get; }


    public double[][][][] GetGeometryCoordinates();

    public void Add(double[][] coordinates);

    public void AddInverted(double[][] coordinates);

    public void Reset();
}