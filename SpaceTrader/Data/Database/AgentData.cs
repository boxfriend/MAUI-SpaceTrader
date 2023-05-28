using SQLite;
using SQLiteNetExtensions.Attributes;

namespace SpaceTrader.Data;

public class AgentData
{
    [PrimaryKey,Unique] public string AccountID { get; set; }
    [Unique] public string Name { get; set; }
    [Unique] public string Token { get; set; }
    public string Headquarters { get; set; }
    public int Credits { get; set; }

    [OneToMany]
    public List<ShipData> Ships { get; set; }

    internal static AgentData FromAPIAgent (Agent data, string token) => new AgentData()
    {
        AccountID = data.AccountID,
        Name = data.Name,
        Token = token,
        Credits = data.Credits,
        Headquarters = data.Headquarters,
    };
}
