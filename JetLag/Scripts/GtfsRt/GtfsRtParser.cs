using TransitRealtime;

namespace JetLag.Scripts.GtfsRt;

public class GtfsRtParser
{
    private readonly ILogger<GtfsRtParser> _logger;

    public GtfsRtParser(ILogger<GtfsRtParser> logger) => _logger = logger;

    public FeedMessage? ParseFile(string absolutePath)
    {
        if (!File.Exists(absolutePath))
        {
            _logger.LogWarning("GTFS-RT feed file not found at {Path}", absolutePath);
            return null;
        }

        try
        {
            using var stream = File.OpenRead(absolutePath);
            return FeedMessage.Parser.ParseFrom(stream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse GTFS-RT feed at {Path}", absolutePath);
            return null;
        }
    }
}
