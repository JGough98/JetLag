using JetLag.Scripts.Factory.Interface;
using JetLag.Scripts.Geomitry;


namespace JetLag.Scripts.Factory;

public class GeomitryCombinderFactory : IFactory<IGeomitryCombinder>
{
    public IGeomitryCombinder Create()
    {
        return new GeomitryCombinder(
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