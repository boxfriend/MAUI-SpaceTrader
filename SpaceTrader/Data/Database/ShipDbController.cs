using Microsoft.Extensions.Logging;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace SpaceTrader.Data;

internal class ShipDbController : BaseDbController
{
    public ShipDbController (string path, ApiClient client, ILogger<BaseDbController> logger) : base(path, client, logger) { }
    protected override async Task Initialize ()
    {
        await _connection.CreateTableAsync<Ship>();
        await _connection.CreateTableAsync<ShipRegistration>();
        await _connection.CreateTableAsync<ShipNav>();
        await _connection.CreateTableAsync<ShipCrew>();
        await _connection.CreateTableAsync<ShipFrame>();
        await _connection.CreateTableAsync<ShipReactor>();
        await _connection.CreateTableAsync<ShipEngine>();
        await _connection.CreateTableAsync<ShipModule>();
        await _connection.CreateTableAsync<ShipMount>();
        await _connection.CreateTableAsync<ShipCargo>();
        await _connection.CreateTableAsync<ShipCargoItem>();
        await _connection.CreateTableAsync<ShipFuel>();
    }

    public async Task Insert (string accountID, params Ship[] ships)
    {
        foreach(var ship in ships)
        {
            ship.AccountID = accountID;
            ship.Registration.ShipID = ship.Symbol;
            ship.Nav.ShipID = ship.Symbol;
            ship.Crew.ShipID = ship.Symbol;
            ship.Frame.ShipID = ship.Symbol;
            ship.Reactor.ShipID = ship.Symbol;
            ship.Engine.ShipID = ship.Symbol;
            foreach (var mod in ship.Modules) mod.ShipID = ship.Symbol;
            foreach(var mount in ship.Mounts) mount.ShipID = ship.Symbol;
            ship.Cargo.ShipID = ship.Symbol;
            foreach(var cargo in ship.Cargo.Inventory) cargo.CargoID = ship.Symbol;
            ship.Fuel.ShipID = ship.Symbol;
        }
        await _connection.InsertOrReplaceAllWithChildrenAsync(ships, true);
    }

    private async Task DeleteShipCargo (Ship ship)
    {
        await _connection.GetChildAsync(ship, "Cargo", true);
        await _connection.DeleteAsync(ship.Cargo, true);
    }

    public async Task DeleteAllFromAgent(Agent data)
    {
        var allShips = await GetAgentShips(data);
        await _connection.DeleteAllAsync(allShips, true);
    }

    public async Task<List<Ship>> GetAgentShips (Agent data) => await GetAgentShips(data.AccountID);
    public async Task<List<Ship>> GetAgentShips (string accountID) => await _connection.GetAllWithChildrenAsync<Ship>(ship => ship.AccountID == accountID, true);
}