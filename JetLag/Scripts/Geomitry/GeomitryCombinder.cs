using NetTopologySuite.Geometries;

namespace JetLag.Scripts.Geomitry;


public class GeomitryCombinder : IGeomitryCombinder
{
    private readonly GeometryFactory _geometryFactory;

    private Geometry? _masterGeometry;


    public GeomitryCombinder()
    {
        _geometryFactory = new GeometryFactory();
    }


    public double[][][] GetGeometryCoordinates()
    {
        if (_masterGeometry is not Polygon poly)
            return Array.Empty<double[][]>();

        var rings = new List<double[][]>
        {
            poly.ExteriorRing.Coordinates.Select(c => new[] { c.X, c.Y }).ToArray()
        };

        for (int i = 0; i < poly.NumInteriorRings; i++)
        {
            rings.Add(
                poly.GetInteriorRingN(i).Coordinates.Select(c => new[] { c.X, c.Y }).ToArray()
            );
        }

        return rings.ToArray();
    }

    public void Add(double[][] coordinates)
    {
        var newPoly = CreatePolygon(coordinates);

        if (_masterGeometry is null)
        {
            _masterGeometry = newPoly;
            return;
        }

        _masterGeometry = _masterGeometry.Union(newPoly);
    }

    public void AddInverted(double[][] coordinates, double[][] worldBounds)
    {
        var holePoly = CreatePolygon(coordinates);

        if (_masterGeometry is null)
        {
            var worldPoly = CreatePolygon(worldBounds);
            _masterGeometry = worldPoly.Difference(holePoly);
            return;
        }

        _masterGeometry = _masterGeometry.Difference(holePoly);
    }


    private Polygon CreatePolygon(double[][] coords)
    {
        var points = coords.Select(c => new Coordinate(c[0], c[1])).ToArray();
        return _geometryFactory.CreatePolygon(points);
    }
}
