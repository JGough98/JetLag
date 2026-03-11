namespace JetLag.Scripts.Utility;


/// <summary>
/// Utility class for throwing exceptions.
/// </summary>
public static class ErrorUtility
{
    /// <summary>
    /// Throws an exception if the type is not contained in the specified namespace.
    /// </summary>
    /// <typeparam name="T">The type to check.</typeparam>
    public static void ThrowIfNotContainedInNamespace<T>(string nameSpace)
    {
        if (typeof(T).Namespace is not null && !typeof(T).Namespace.Contains(nameSpace))
            throw new InvalidOperationException(
                $"Type {typeof(T).FullName} is not within the {nameSpace} namespace.");
    }
}