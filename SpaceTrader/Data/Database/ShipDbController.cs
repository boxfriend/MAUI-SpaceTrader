using Microsoft.Extensions.Logging;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace SpaceTrader.Data;

internal class ShipDbController : BaseDbController
{


    public ShipDbController (string path, ApiClient client, ILogger<BaseDbController> logger) : base(path, client, logger) { }
    protected override async Task Initialize ()
    {

        if (_isInitialized)
            return;

        _isInitialized = true;
        await _connection.CreateTableAsync<ShipData>();
        await _connection.CreateTableAsync<CargoItem>();
    }

    
    public async Task Insert(Ship ship, string accountID)
    {
        var shipData = ShipData.FromApiObject(ship);
        shipData.AccountID = accountID;
        await Insert(shipData);
        await DeleteShipCargo(shipData);
        foreach (var item in ship.Cargo.Inventory)
        {
            var cargo = CargoItem.FromApiObject(item);
            cargo.Ship = shipData.Symbol;
            await _connection.InsertOrReplaceAsync(cargo);
        }
    }
    public async Task Insert (IEnumerable<Ship> ships, string accountID)
    {
        foreach(var ship in ships)
        {
            await Insert(ship, accountID);
        }
    }

    private async Task DeleteShipCargo (ShipData ship) => await _connection.Table<CargoItem>().DeleteAsync(x => x.Ship == ship.Symbol);

    public async Task DeleteAllFromAgent(Agent data)
    {
        var allShips = await GetAgentShips(data);
        await _connection.DeleteAllAsync(allShips, true);
    }

    public async Task<List<ShipData>> GetAgentShips (Agent data) => await GetAgentShips(data.AccountID);
    public async Task<List<ShipData>> GetAgentShips (string accountID)
    {
        await Initialize();
        return await _connection.GetAllWithChildrenAsync<ShipData>(ship => ship.AccountID == accountID);
    }
}

public static class SqliteExtensions
{
    public static async Task DeleteAllAsync<T>(this SQLiteAsyncConnection conn, IEnumerable<T> data)
    {
        foreach(var d in data)
        {
            await conn.DeleteAsync(d);
        }
    }
}