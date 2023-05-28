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
    }

    public async Task InsertOrUpdate(ShipData shipData)
    {
        await InitializeAsync();
        await _connection.InsertOrReplaceAsync(shipData);
    }

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

    public async Task DeleteAllFromAgent(AgentData data)
    {
        var allShips = await GetAgentShips(data);
        foreach(var ship in allShips)
        {
            await Delete(ship);
        }
    }

    public async Task<List<ShipData>> GetAgentShips (AgentData data) => await GetAgentShips(data.AccountID);
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