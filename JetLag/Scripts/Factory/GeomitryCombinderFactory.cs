using JetLag.Scripts.Factory.Interface;
using JetLag.Scripts.Geometry;


namespace JetLag.Scripts.Factory;

public class GeomitryCombinderFactory : IFactory<IGeometryCombinder>
{
    public IGeometryCombinder Create()
    {
        return new GeometryCombinder(
            worldBounds:
            [
                [-180, -90],
                [180, -90],
                [180, 90],
                [-180, 90],
                [-180, -90]
            ],
            precisionScale : 1000
        );
    }
}