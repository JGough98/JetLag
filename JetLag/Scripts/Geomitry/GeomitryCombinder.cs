namespace JetLag.Scripts.Geomitry;

using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Union;
using System.Collections.Generic;
using System.Linq;


public class GeomitryCombinder : IGeomitryCombinder
{
    private readonly GeometryFactory _geometryFactory;

    private readonly List<Geometry> _positiveShapes;
    private readonly List<Geometry> _negativeShapes;

    private readonly double[][] _worldBounds;


    public bool InUse => _negativeShapes.Count > 0 || _positiveShapes.Count > 0;


    public GeomitryCombinder(double[][] worldBounds, double precisionScale)
    {
        _worldBounds = worldBounds;
        _geometryFactory = new GeometryFactory(new PrecisionModel(precisionScale));
        _positiveShapes = new();
        _negativeShapes = new();
    }


    public void Add(double[][] coordinates)
    {
        _positiveShapes.Add(CreatePolygon(coordinates));
    }

    public void AddInverted(double[][] coordinates)
    {
        _negativeShapes.Add(CreatePolygon(coordinates));
    }

    public double[][][][] GetGeometryCoordinates()
    {
        if (!InUse)
            return Array.Empty<double[][][]>();

        return GetGeometryCoordinates(BuildFinalGeometry());
    }

    public void Reset()
    {
        _positiveShapes.Clear();
        _negativeShapes.Clear();
    }


    private double[][][][] GetGeometryCoordinates(Geometry? geometry)
    {
        if (geometry == null || geometry.IsEmpty)
            return Array.Empty<double[][][]>();

        var polygons = new List<double[][][]>();

        for (int i = 0; i < geometry.NumGeometries; i++)
        {
            if (geometry.GetGeometryN(i) is Polygon poly)
            {
                var rings = new List<double[][]>
                {
                    poly.ExteriorRing.Coordinates.Select(c => new[] { c.X, c.Y }).ToArray()
                };

                for (int j = 0; j < poly.NumInteriorRings; j++)
                    rings.Add(poly.GetInteriorRingN(j).Coordinates.Select(c => new[] { c.X, c.Y }).ToArray());

                polygons.Add(rings.ToArray());
            }
        }

        return polygons.ToArray();
    }

    private Geometry BuildFinalGeometry()
    {
        var totalPositive = UnaryUnionOp.Union(_positiveShapes);

        Geometry? totalNegative = null;

        if (_negativeShapes.Count > 0)
        {
            totalNegative = _negativeShapes[0];

            for (int i = 1; i < _negativeShapes.Count; i++)
            {
                totalNegative = totalNegative.Intersection(_negativeShapes[i]);

                if (totalNegative.IsEmpty)
                    break;
            }
        }

        if (totalNegative == null || totalNegative.IsEmpty)
            return totalPositive;

        var worldPoly = CreatePolygon(_worldBounds);
        var playerPossibleArea = worldPoly.Difference(totalNegative);

        return playerPossibleArea.Union(totalPositive);
    }

    private Polygon CreatePolygon(double[][] coords)
    {
        var points = coords.Select(c => new Coordinate(c[0], c[1])).ToArray();

        if (!points[0].Equals2D(points[^1]))
        {
            var closedPoints = new Coordinate[points.Length + 1];
            System.Array.Copy(points, closedPoints, points.Length);
            closedPoints[^1] = points[0];
            points = closedPoints;
        }

        return _geometryFactory.CreatePolygon(points);
    }
}