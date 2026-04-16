using CsvHelper;
using CsvHelper.Configuration;
using JetLag.Scripts.Data.Gtfs;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace JetLag.Scripts.Data;

public class GtfsSeeder
{
    // Limit batch size to prevent excessive memory consumption and SQL timeout errors
    private const int MAX_ENTITY_ADD_COUNT = 5000;
    private readonly GtfsDbContext _db;
    private readonly string _gtfsFolder;

    // Standard CSV configuration for GTFS data
    private static readonly CsvConfiguration CsvConfig =
        new(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            // Permissive settings: GTFS feeds often contain inconsistent columns or trailing commas
            MissingFieldFound = null,
            BadDataFound = null
        };


    public GtfsSeeder(GtfsDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        // Resolve the physical path to the GTFS files within the project structure
        _gtfsFolder = Path.Combine(env.ContentRootPath, "Data", "Gtfs");
    }


    public async Task SeedAsync()
    {
        // Ensure the schema exists before seeding
        await _db.Database.EnsureCreatedAsync();

        // Guard clause: Avoid duplicate seeding if data already exists
        if (await _db.Stops.AnyAsync())
            return;

        // PERFORMANCE: Disable AutoDetectChanges for bulk inserts. 
        // This prevents EF from scanning the context for changes after every single record added.
        _db.ChangeTracker.AutoDetectChangesEnabled = false;

        // Seed core GTFS tables in order of dependency
        await Seed<GtfsStopMap, GtfsStop>("stops.txt", _db.Stops);
        await Seed<GtfsRouteMap, GtfsRoute>("routes.txt", _db.Routes);
        await Seed<GtfsTripMap, GtfsTrip>("trips.txt", _db.Trips);
        await Seed<GtfsStopTimeMap, GtfsStopTime>("stop_times.txt", _db.StopTimes);

        // Re-enable tracking after the bulk operation is finished
        _db.ChangeTracker.AutoDetectChangesEnabled = true;
    }


    /// <summary>
    /// Generic helper to stream records from CSV to Database using batching.
    /// </summary>
    /// <typeparam name="T">The CsvHelper ClassMap</typeparam>
    /// <typeparam name="U">The Database Entity Type</typeparam>
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
        
        // GetRecordsAsync streams data, which is vital for stop_times.txt (often millions of rows)
        await foreach (var record in csv.GetRecordsAsync<U>())
        {
            batch.Add(record);

            // Execute a batch save when the threshold is met
            if (batch.Count >= MAX_ENTITY_ADD_COUNT)
            {
                await dbSet.AddRangeAsync(batch);
                await _db.SaveChangesAsync();
                batch.Clear(); // Free memory for the next batch
            }
        }

        if (batch.Count > 0)
        {
            await dbSet.AddRangeAsync(batch);
            await _db.SaveChangesAsync();
        }
    }
}