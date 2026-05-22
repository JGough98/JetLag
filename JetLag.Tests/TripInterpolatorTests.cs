using JetLag.Scripts.Execution;
using JetLag.Scripts.Models;

namespace JetLag.Tests;

public class TripInterpolatorTests
{
    private static readonly TripInterpolator _sut = new();

    private static IReadOnlyList<TripStopTime> MakeStops() =>
    [
        new("s1", "Alpha",   51.0, -0.1,    0),
        new("s2", "Beta",    51.5, -0.15, 600),
        new("s3", "Gamma",   52.0, -0.2, 1200),
    ];

    [Fact]
    public void Returns_first_stop_when_time_is_before_first_arrival()
    {
        var stops = MakeStops();
        var (lat, lon) = _sut.Interpolate(stops, TimeSpan.FromSeconds(-10));
        Assert.Equal(stops[0].Lat, lat);
        Assert.Equal(stops[0].Lon, lon);
    }

    [Fact]
    public void Returns_first_stop_at_exactly_first_arrival()
    {
        var stops = MakeStops();
        var (lat, lon) = _sut.Interpolate(stops, TimeSpan.FromSeconds(0));
        Assert.Equal(stops[0].Lat, lat);
        Assert.Equal(stops[0].Lon, lon);
    }

    [Fact]
    public void Returns_last_stop_when_time_is_after_last_arrival()
    {
        var stops = MakeStops();
        var (lat, lon) = _sut.Interpolate(stops, TimeSpan.FromSeconds(1500));
        Assert.Equal(stops[^1].Lat, lat);
        Assert.Equal(stops[^1].Lon, lon);
    }

    [Fact]
    public void Returns_last_stop_at_exactly_last_arrival()
    {
        var stops = MakeStops();
        var (lat, lon) = _sut.Interpolate(stops, TimeSpan.FromSeconds(1200));
        Assert.Equal(stops[^1].Lat, lat);
        Assert.Equal(stops[^1].Lon, lon);
    }

    [Fact]
    public void Returns_midpoint_at_halfway_between_first_two_stops()
    {
        var stops = MakeStops();
        var (lat, lon) = _sut.Interpolate(stops, TimeSpan.FromSeconds(300));
        Assert.Equal((stops[0].Lat + stops[1].Lat) / 2, lat, precision: 10);
        Assert.Equal((stops[0].Lon + stops[1].Lon) / 2, lon, precision: 10);
    }

    [Fact]
    public void Returns_midpoint_at_halfway_between_second_and_third_stops()
    {
        var stops = MakeStops();
        var (lat, lon) = _sut.Interpolate(stops, TimeSpan.FromSeconds(900));
        Assert.Equal((stops[1].Lat + stops[2].Lat) / 2, lat, precision: 10);
        Assert.Equal((stops[1].Lon + stops[2].Lon) / 2, lon, precision: 10);
    }

    [Fact]
    public void Returns_exact_stop_coords_at_intermediate_arrival_time()
    {
        var stops = MakeStops();
        var (lat, lon) = _sut.Interpolate(stops, TimeSpan.FromSeconds(600));
        Assert.Equal(stops[1].Lat, lat, precision: 10);
        Assert.Equal(stops[1].Lon, lon, precision: 10);
    }

    [Fact]
    public void Returns_zero_zero_for_empty_list()
    {
        var (lat, lon) = _sut.Interpolate(Array.Empty<TripStopTime>(), TimeSpan.FromSeconds(100));
        Assert.Equal(0, lat);
        Assert.Equal(0, lon);
    }

    [Fact]
    public void Returns_only_stop_for_single_stop_list()
    {
        var stops = new[] { new TripStopTime("s1", "Solo", 51.0, -0.1, 300) };
        var (lat, lon) = _sut.Interpolate(stops, TimeSpan.FromSeconds(300));
        Assert.Equal(51.0, lat);
        Assert.Equal(-0.1, lon);
    }
}
