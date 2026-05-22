using Google.Protobuf;
using TransitRealtime;

namespace JetLag.Scripts.GtfsRt;

/// <summary>
/// Generates a synthetic GTFS-RT feed.pb for development/testing.
/// Creates a 5-minute delay on trip T1 at stops S4–S6 and cancels trip T2.
/// </summary>
public class GtfsRtTestFeedGenerator
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<GtfsRtTestFeedGenerator> _logger;

    public GtfsRtTestFeedGenerator(IWebHostEnvironment env, ILogger<GtfsRtTestFeedGenerator> logger)
    {
        _env = env;
        _logger = logger;
    }

    public void Generate()
    {
        var outputDir = Path.Combine(_env.ContentRootPath, "Data", "GtfsRt");
        Directory.CreateDirectory(outputDir);
        var outputPath = Path.Combine(outputDir, "feed.pb");

        if (File.Exists(outputPath))
            return;

        var feed = new FeedMessage
        {
            Header = new FeedHeader
            {
                GtfsRealtimeVersion = "2.0",
                Incrementality = FeedHeader.Types.Incrementality.FullDataset,
                Timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            }
        };

        // T1: 5-minute delay (300s) at S4 Jamaica, propagating to S5 and S6
        feed.Entity.Add(new FeedEntity
        {
            Id = "T1-delay",
            TripUpdate = new TripUpdate
            {
                Trip = new TripDescriptor { TripId = "T1" },
                StopTimeUpdate =
                {
                    new TripUpdate.Types.StopTimeUpdate
                    {
                        StopSequence = 4,
                        StopId = "S4",
                        Arrival   = new TripUpdate.Types.StopTimeEvent { Delay = 300 },
                        Departure = new TripUpdate.Types.StopTimeEvent { Delay = 300 }
                    },
                    new TripUpdate.Types.StopTimeUpdate
                    {
                        StopSequence = 5,
                        StopId = "S5",
                        Arrival   = new TripUpdate.Types.StopTimeEvent { Delay = 300 },
                        Departure = new TripUpdate.Types.StopTimeEvent { Delay = 300 }
                    },
                    new TripUpdate.Types.StopTimeUpdate
                    {
                        StopSequence = 6,
                        StopId = "S6",
                        Arrival   = new TripUpdate.Types.StopTimeEvent { Delay = 300 }
                    }
                }
            }
        });

        // T2: cancelled
        feed.Entity.Add(new FeedEntity
        {
            Id = "T2-cancel",
            TripUpdate = new TripUpdate
            {
                Trip = new TripDescriptor
                {
                    TripId = "T2",
                    ScheduleRelationship = TripDescriptor.Types.ScheduleRelationship.Canceled
                }
            }
        });

        using var stream = File.Create(outputPath);
        feed.WriteTo(stream);

        _logger.LogInformation("Generated test GTFS-RT feed at {Path}", outputPath);
    }
}
