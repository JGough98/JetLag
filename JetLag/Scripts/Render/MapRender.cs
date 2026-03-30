namespace JetLag.Scripts.Render;


/// <summary>
/// Class used call rendering between the <OutOfBounds> and <Overlay> layers.
/// </summary>
public class MapRender
{
    /// <summary>
    /// Field used to render the bounds out of bounds area on the map.
    /// </summary>
    /// <value></value>
    public required IMapLayerShapeRender OutOfBounds { get; init; }
    /// <summary>
    /// Field used to render the potential new state of a map update.
    /// </summary>
    /// <value></value>
    public required IMapLayerShapeRender Overlay { get; init; }
}
