using SQLite;
using SQLiteNetExtensions.Attributes;

namespace SpaceTrader.Data;

public class ShipData
{
    [PrimaryKey,Unique] public string Symbol { get; set; }
    [ForeignKey(typeof(AgentData))] public string AccountID { get; set; }
    
    public string Role { get; set; }
    public string SystemSymbol { get; set; }
    public string WaypointSymbol { get; set; }

    public static ShipData FromApiObject (Ship ship) => new()
    {
        Symbol = ship.Symbol,
        Role = ship.Registration.Role,
        SystemSymbol = ship.Nav.SystemSymbol,
        WaypointSymbol = ship.Nav.WaypointSymbol,
    };
}