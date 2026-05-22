using JetLag.Scripts.Models;
using JetLag.Scripts.Transit;

namespace JetLag.Scripts.GtfsRt;

public class HistoricalResolutionService : IHistoricalResolutionService
{
    private readonly GtfsRtParser _parser;
    private readonly TripOutcomeCalculator _calculator;
    private readonly ITransitService _transitService;
    private readonly ILogger<HistoricalResolutionService> _logger;
    private readonly string _feedAbsolutePath;

    public HistoricalResolutionService(
        GtfsRtParser parser,
        TripOutcomeCalculator calculator,
        ITransitService transitService,
        IConfiguration configuration,
        IWebHostEnvironment env,
        ILogger<HistoricalResolutionService> logger)
    {
        _parser = parser;
        _calculator = calculator;
        _transitService = transitService;
        _logger = logger;

        var relativePath = configuration["GtfsRt:FeedPath"] ?? "Data/GtfsRt/feed.pb";
        _feedAbsolutePath = Path.Combine(env.ContentRootPath, relativePath);
    }

    public async Task<TripOutcome?> ResolveAsync(string tripId, string stopId, int scheduledArrivalSeconds)
    {
        var feed = _parser.ParseFile(_feedAbsolutePath);
        if (feed is null)
        {
            _logger.LogWarning("GTFS-RT feed unavailable; cannot resolve trip {TripId}", tripId);
            return null;
        }

        var tripUpdate = feed.Entity
            .Where(e => e.TripUpdate != null && e.TripUpdate.Trip.TripId == tripId)
            .Select(e => e.TripUpdate)
            .FirstOrDefault();

        if (tripUpdate is null)
        {
            _logger.LogInformation("Trip {TripId} not in GTFS-RT feed; assuming on-time", tripId);
            return null;
        }

        var stops = await _transitService.GetStopsForTrip(tripId);
        var targetStopSequence = stops
            .Select((s, i) => (s.StopId, Index: i))
            .Where(x => x.StopId == stopId)
            .Select(x => x.Index)
            .DefaultIfEmpty(int.MaxValue)
            .First();

        return _calculator.Calculate(tripUpdate, stopId, scheduledArrivalSeconds, targetStopSequence);
    }
}
