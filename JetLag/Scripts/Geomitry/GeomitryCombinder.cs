namespace JetLag.Scripts.Geomitry;

using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Union;
using System.Collections.Generic;
using System.Linq;


public class GeomitryCombinder : IGeomitryCombinder
{
    private readonly GeometryFactory _geometryFactory;
    private readonly List<Geometry> _positiveShapes = new();
    private readonly List<Geometry> _negativeShapes = new();

    private double[][] _worldBounds;


    public GeomitryCombinder()
    {
        _geometryFactory = new GeometryFactory(new PrecisionModel(1000));
    }


    public void Add(double[][] coordinates)
    {
        _positiveShapes.Add(CreatePolygon(coordinates));
    }

    public void AddInverted(double[][] coordinates, double[][] worldBounds)
    {
        _worldBounds = worldBounds;
        _negativeShapes.Add(CreatePolygon(coordinates));
    }

    public double[][][] GetGeometryCoordinates()
    {
        // 1. Combine all positive shapes (OR logic: "Player is NOT in A or B or C")
        var totalPositive = UnaryUnionOp.Union(_positiveShapes);

        // 2. Intersect all negative shapes (AND logic: "Player IS in A and B and C")
        Geometry totalNegative = null;

        if (_negativeShapes.Count > 0)
        {
            // Start with the first negative shape
            totalNegative = _negativeShapes[0];

            // Intersect it with every subsequent negative shape
            for (int i = 1; i < _negativeShapes.Count; i++)
            {
                totalNegative = totalNegative.Intersection(_negativeShapes[i]);
                
                // Optimization: If they don't overlap at all, the player 
                // logically cannot exist (or the clues are contradictory).
                if (totalNegative.IsEmpty) break;
            }
        }

        // 3. Final Calculation
        Geometry finalResult;

        if (totalNegative == null || totalNegative.IsEmpty)
        {
            // If no negative clues, just show the positives
            finalResult = totalPositive;
        }
        else
        {
            // The game logic: 
            // "Show the whole world MINUS the area we know the player IS in"
            // Then ADD the areas we know the player IS NOT in.
            var worldPoly = CreatePolygon(_worldBounds);
            var playerPossibleArea = worldPoly.Difference(totalNegative);
            
            finalResult = playerPossibleArea.Union(totalPositive);
        }

        return GetGeometryCoordinates(finalResult.Normalized());
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

    private double[][][] GetGeometryCoordinates(Geometry? geometry)
    {
        if (geometry == null || geometry.IsEmpty) return Array.Empty<double[][]>();

        var allRings = new List<double[][]>();

        for (int i = 0; i < geometry.NumGeometries; i++)
        {
            if (geometry.GetGeometryN(i) is Polygon poly)
            {
                // First ring: Exterior (The solid part of this polygon)
                allRings.Add(poly.ExteriorRing.Coordinates.Select(c => new[] { c.X, c.Y }).ToArray());

                // Subsequent rings: Interior (The holes inside this polygon)
                for (int j = 0; j < poly.NumInteriorRings; j++)
                {
                    allRings.Add(poly.GetInteriorRingN(j).Coordinates.Select(c => new[] { c.X, c.Y }).ToArray());
                }
            }
        }
        return allRings.ToArray();
    }
}