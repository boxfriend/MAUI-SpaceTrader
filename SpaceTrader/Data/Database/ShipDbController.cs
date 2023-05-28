using SQLite;

namespace SpaceTrader.Data;

internal class ShipDbController
{
    private readonly string _dbPath;
    private SQLiteAsyncConnection _connection;

    public ShipDbController (string path)
    {
        _dbPath = path;
    }
    public async Task InitializeAsync ()
    {

        if (_connection != null)
            return;

        _connection = new(_dbPath);

        await _connection.CreateTableAsync<ShipData>();
        await _connection.CreateTableAsync<CargoItem>();
    }

    public async Task InsertOrUpdate(ShipData shipData)
    {
        await InitializeAsync();
        await _connection.InsertOrReplaceAsync(shipData);
    }
    public async Task InsertOrUpdate(Ship ship, string accountID)
    {
        var shipData = ShipData.FromApiObject(ship);
        shipData.AccountID = accountID;
        await InsertOrUpdate(shipData);
        await DeleteShipCargo(shipData);
        foreach (var item in ship.Cargo.Inventory)
        {
            var cargo = CargoItem.FromApiObject(item);
            cargo.Ship = shipData.Symbol;
            await _connection.InsertOrReplaceAsync(cargo);
        }
    }
    public async Task InsertOrUpdate (IEnumerable<Ship> ships, string accountID)
    {
        foreach(var ship in ships)
        {
            await InsertOrUpdate(ship, accountID);
        }
    }

    private async Task DeleteShipCargo (ShipData ship) => await _connection.Table<CargoItem>().DeleteAsync(x => x.Ship == ship.Symbol);

    public async Task InsertOrUpdate(IEnumerable<ShipData> shipData)
    {
        foreach(var ship in shipData)
            await InsertOrUpdate(ship);
    }

    public async Task Delete (ShipData data)
    {
        await InitializeAsync();
        await _connection.DeleteAsync(data);
    }

    public async Task DeleteAllFromAgent(Agent data)
    {
        var allShips = await GetAgentShips(data);
        await _connection.DeleteAllAsync(allShips);
    }

    public async Task<List<ShipData>> GetAgentShips (Agent data) => await GetAgentShips(data.AccountID);
    public async Task<List<ShipData>> GetAgentShips (string accountID)
    {
        await InitializeAsync();
        return await _connection.Table<ShipData>().Where(ship => ship.AccountID == accountID).ToListAsync();
    }

    public async Task<ShipData> GetShip(string shipSymbol)
    {
        await InitializeAsync();
        return await _connection.GetAsync<ShipData>(shipSymbol);
    }
    public async Task<ShipData> GetShip(ShipData ship) => await GetShip(ship.Symbol);
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