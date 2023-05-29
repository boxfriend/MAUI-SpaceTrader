using Microsoft.Extensions.Logging;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace SpaceTrader.Data;

internal class SystemDbController : BaseDbController
{
    public SystemDbController (string path, ApiClient client, ILogger<BaseDbController> logger) : base(path, client, logger) { }

    protected override async Task Initialize ()
    {
        if (_isInitialized)
            return;

        _isInitialized = true;
        await _connection.CreateTableAsync<System>();
        await _connection.CreateTableAsync<SystemWaypoint>();
    }
    public async Task Insert (params System[] systems)
    {
        await Initialize();
        foreach(var data in systems)
        {
            foreach(var waypoint in data.Waypoints)
            {
                waypoint.System = data.Symbol;
            }
        }

        await _connection.InsertAllWithChildrenAsync(systems, true);
    }
}