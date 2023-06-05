using Microsoft.Extensions.Logging;
using SQLiteNetExtensionsAsync.Extensions;

namespace SpaceTrader.Data;

internal class SystemDbController : BaseDbController
{
    public SystemDbController (string path, ApiClient client, ILogger<BaseDbController> logger) : base(path, client, logger) { }

    protected override async Task Initialize ()
    {
        await _connection.CreateTableAsync<System>();
        await _connection.CreateTableAsync<SystemWaypoint>();
        await _connection.CreateTableAsync<SystemFaction>();
    }
    public async Task Insert (params System[] systems)
    {
        foreach(var data in systems)
        {
            data.Waypoints.ForEach(x => x.System = data.Symbol);
            data.Factions.ForEach(x => x.System = data.Symbol);
        }
        await _connection.InsertAllWithChildrenAsync(systems, true);
    }
}