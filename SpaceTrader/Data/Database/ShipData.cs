using SQLite;
using SQLiteNetExtensions.Attributes;

namespace SpaceTrader.Data;

public class ShipData
{
    [PrimaryKey,Unique] public string Symbol { get; set; }
    [ForeignKey(typeof(AgentData))] public string AccountID { get; set; }
    public Ship Ship { get; set; }
}