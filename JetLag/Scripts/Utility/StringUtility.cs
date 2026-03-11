public static class StringUtility
{
    /// <summary>
    /// Sanitises a string by trimming it.
    /// </summary>
    /// <param name="name">The string to sanitise.</param>
    /// <returns>The sanitised string.</returns>
    public static string Sanitise(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

       return name.Trim();
    }
}