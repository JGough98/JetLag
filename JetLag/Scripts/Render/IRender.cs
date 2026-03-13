namespace JetLag.Scripts.Render;


/// <summary>
/// Interface to render on a map.
/// </summary>
/// <typeparam name="T">The type of the map event arguments.</typeparam>
/// <typeparam name="U">The type of the render data.</typeparam>
public interface IRender<T, U>
{
    /// <summary>
    /// Renders the data on the map.
    /// </summary>
    /// <param name="args">The map event arguments.</param>
    /// <param name="renderData">The render data.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task Render(T args, U renderData);

    /// <summary>
    /// Updates the data on the map.
    /// </summary>
    /// <param name="args">The map event arguments.</param>
    /// <param name="updateData">The update data.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task Update(T args, U updateData);

    /// <summary>
    /// Removes the data from the map.
    /// </summary>
    /// <param name="args">The map event arguments.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task Remove(T args);
}
