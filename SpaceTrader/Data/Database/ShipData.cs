using SQLite;
using SQLiteNetExtensions.Attributes;

namespace SpaceTrader.Data;

public class ShipData
{
    #region Registration
    [PrimaryKey,Unique] public string Symbol { get; set; }
    [ForeignKey(typeof(Agent)), Indexed] public string AccountID { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
    #endregion
    #region Nav
    public string SystemSymbol { get; set; }
    public string WaypointSymbol { get; set; }
    public ShipStatus Status { get; set; }
    public int Fuel { get; set; }
    #endregion
    #region Cargo
    [OneToMany(CascadeOperations = CascadeOperation.All)] public List<CargoItem> Inventory { get; set; }
    public int Capacity { get; set; }
    public int Contained { get; set; }
    #endregion


    public static ShipData FromApiObject (Ship ship) => new()
    {
        Symbol = ship.Symbol,
        Name = ship.Registration.Name,
        Role = ship.Registration.Role,
        SystemSymbol = ship.Nav.SystemSymbol,
        WaypointSymbol = ship.Nav.WaypointSymbol,
        Status = StatusFromString(ship.Nav.Status),
        Fuel = ship.Fuel.Current,
        Capacity = ship.Cargo.Capacity,
        Contained = ship.Cargo.Units,

    };

    public static ShipStatus StatusFromString (string str) => str switch
    {
        "DOCKED" => ShipStatus.Docked,
        "IN_TRANSIT" => ShipStatus.InTransit,
        "IN_ORBIT" => ShipStatus.InOrbit,
        _ => throw new NotImplementedException()
    };
}
public enum ShipStatus
{
    InTransit,InOrbit,Docked
}
public class CargoItem
{
    [ForeignKey(typeof(ShipData)), Indexed] public string Ship { get; set; }
    public string Symbol { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Units { get; set; }

    public static CargoItem FromApiObject (ShipCargoItem shipCargoItem) => new()
    {
        Symbol = shipCargoItem.Symbol,
        Name = shipCargoItem.Name,
        Description = shipCargoItem.Description,
        Units = shipCargoItem.Units,
    };
}