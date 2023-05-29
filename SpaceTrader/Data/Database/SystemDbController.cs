using Microsoft.Extensions.Logging;
using SQLite;

namespace SpaceTrader.Data;

internal class SystemDbController
{
    private readonly string _dbPath;
    private SQLiteAsyncConnection _connection;

    private readonly ApiClient _client;
    private readonly ILogger<SystemDbController> _logger;

    public SystemDbController (string path, ApiClient client, ILogger<SystemDbController> logger)
    {
        _dbPath = path;
        _client = client;
        _logger = logger;
    }

    public async Task InitializeAsync ()
    {

        if (_connection != null)
            return;

        _connection = new(_dbPath);

        await _connection.CreateTableAsync<System>();
        await _connection.CreateTableAsync<SystemWaypoint>();
    }

    public async Task<List<System>> GetAll ()
    {
        await InitializeAsync();
        return await _connection.Table<System>().ToListAsync();
    }

    public async Task<System> Get (string symbol)
    {
        await InitializeAsync();
        return await _connection.FindAsync<System>(symbol);
    }
    public async Task InsertOrReplace (params System[] systems)
    {
        await InitializeAsync();
        foreach(var data in systems)
        {
            await _connection.InsertOrReplaceAsync(data);
            foreach(var waypoint in data.Waypoints)
            {
                waypoint.System = data.Symbol;
                await _connection.InsertOrReplaceAsync(waypoint);
            }
        }
    }


    public async Task Delete (System data) => await _connection.DeleteAsync(data);

}