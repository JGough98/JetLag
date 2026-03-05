namespace JetLag.Scripts.Utility;

public static class ReadUtility
{
    /// <summary>
    /// Reads a text file from the resources.
    /// </summary>
    /// <param name="resourceName">The name of the resource to read.</param>
    /// <returns>A list of strings read from the file.</returns>
    /// <summary>
    public static List<string> ReadRecourseTextFile(string resourceName)
    {
        var set = new List<string>();
        var assembly = typeof(ReadUtility).Assembly;
        using var stream = assembly.GetManifestResourceStream("JetLag.Resources." + resourceName + ".txt");

        if (stream is null)
            return set;

        using var reader = new System.IO.StreamReader(stream);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();

            if (!string.IsNullOrWhiteSpace(line))
                set.Add(line.Trim());
        }

        return set;
    }
}