using CsvHelper;
using CsvHelper.Configuration;
using JetLag.Scripts.Data.Gtfs;
using Microsoft.EntityFrameworkCore;
using System.Globalization;


namespace JetLag.Scripts.Data;

public class GtfsSeeder
{
    private const int MAX_ENTITY_ADD_COUNT = 5000;
    private readonly GtfsDbContext _db;
    private readonly string _gtfsFolder;


    private static readonly CsvConfiguration CsvConfig =
        new(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            BadDataFound = null
        };


    public GtfsSeeder(GtfsDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _gtfsFolder = Path.Combine(env.ContentRootPath, "Data", "Gtfs");
    }


    public async Task SeedAsync()
    {
        await _db.Database.EnsureCreatedAsync();

        if (await _db.Stops.AnyAsync())
            return;

        _db.ChangeTracker.AutoDetectChangesEnabled = false;

        await Seed<GtfsStopMap, GtfsStop>("stops.txt", _db.Stops);
        await Seed<GtfsRouteMap, GtfsRoute>("routes.txt", _db.Routes);
        await Seed<GtfsTripMap, GtfsTrip>("trips.txt", _db.Trips);
        await Seed<GtfsStopTimeMap, GtfsStopTime>("stop_times.txt", _db.StopTimes);

        _db.ChangeTracker.AutoDetectChangesEnabled = true;
    }

    private async Task Seed<T, U>(string fileNameAndType, DbSet<U> dbSet)
        where T : ClassMap<U>
        where U : class
    {
        var path = Path.Combine(_gtfsFolder, fileNameAndType);
        if (!File.Exists(path))
            return;

        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CsvConfig);
        csv.Context.RegisterClassMap<T>();

        var batch = new List<U>(MAX_ENTITY_ADD_COUNT);
        await foreach (var record in csv.GetRecordsAsync<U>())
        {
            batch.Add(record);

            if (batch.Count >= MAX_ENTITY_ADD_COUNT)
            {
                await dbSet.AddRangeAsync(batch);
                await _db.SaveChangesAsync();
                batch.Clear();
            }
        }
        if (batch.Count > 0)
        {
            await dbSet.AddRangeAsync(batch);
            await _db.SaveChangesAsync();
        }
    }
}