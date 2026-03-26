namespace JetLag.Scripts.Geomitry;


public interface IGeomitryCombinder
{
    public bool InUse { get; }


    public double[][][][] GetGeometryCoordinates();

    public void Add(double[][] coordinates);

    public void AddInverted(double[][] coordinates);

    public void Reset();
}