using System.Text.Json.Serialization;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace SpaceTrader.Data;
internal class Response<T>
{
    [JsonPropertyName("data")] public T Data { get; set; }
    [JsonPropertyName("meta")] public Meta Meta { get; set; }
}

public record Meta(int Total, int Page, int Limit);
public record Cooldown (string Ship, int TotalLength, int RemainingLength, DateTime Expiration);

#region Agent And Faction
public record RegisterData(string Symbol, string Faction);
public record Registration(Agent Agent, Contract Contract, Faction Faction, Ship Ship, string Token);
public record Agent
{
    [PrimaryKey, Unique, JsonPropertyName("accountId")] public string AccountID { get; set; }
    [Unique, JsonPropertyName("symbol")] public string Name { get; set; }
    [Unique] public string Token { get; set; }
    public string Headquarters { get; set; }
    public int Credits { get; set; }
    public string StartingFaction { get; set; }
    [OneToMany(CascadeOperations = CascadeOperation.All)] public List<ShipData> Ships { get; set; }
    [OneToMany(CascadeOperations = CascadeOperation.All)] public List<Contract> Contracts { get; set; }
}
public record Faction (string Symbol, string Name, string Description, string Headquarters, FactionTrait[] Traits);
public record FactionTrait (string Type, string Name, string Description);
#endregion

#region Contracts
public record ContractAccept(Agent Agent, Contract Contract);
public record Contract
{
    [ForeignKey(typeof(Agent))] public string AccountID { get; set; }
    [PrimaryKey,Unique,JsonPropertyName("id")] public string ID { get; set; }
    public string FactionSymbol { get; set; }
    public string Type { get; set; }
    [OneToOne(CascadeOperations = CascadeOperation.All)] public ContractTerms Terms { get; set; }
    public bool Accepted { get; set; }
    public bool Fulfilled { get; set; }
    public DateTime Expiration { get; set; }
}

public record ContractTerms
{
    [ForeignKey(typeof(Contract)),PrimaryKey] public string ID { get; set; }
    public DateTime Deadline { get; set; }
    [OneToOne(CascadeOperations = CascadeOperation.All)] public ContractPayment Payment { get; set; }
    [OneToMany(CascadeOperations = CascadeOperation.All), JsonPropertyName("deliver")] public List<ContractDeliverGood> Deliveries { get; set; }
}

public record ContractPayment
{
    [ForeignKey(typeof(ContractTerms)), PrimaryKey] public string ID { get; set; }
    public int OnAccepted { get; set; }
    public int OnFulfilled { get; set; }
}

public record ContractDeliverGood
{
    [ForeignKey(typeof(ContractTerms)),Indexed(Unique = true)] public string TermsID { get; set; }
    [PrimaryKey] public string ID { get => $"{TermsID}_{TradeSymbol}"; set { return; } }
    public string TradeSymbol { get; set; }
    public string DestinationSymbol { get; set; }
    public int UnitsRequired { get; set; }
    public int Fulfilled { get; set; }
}
#endregion

#region Markets
public record Market(string Symbol, TradeGood[] Exports, TradeGood[] Imports, TradeGood[] Exchange, MarketTransaction[] Transactions, MarketTradeGood[] TradeGoods);
public record MarketTradeGood(string Symbol, int TradeVolume, string Supply, int PurchasePrice, int SellPrice);
public record MarketTransaction(string WaypointSymbol, string ShipSymbol, string TradeSymbol, string Type, int Units, int PricePerUnit, int TotalPrice, DateTime Timestamp);
public record TradeGood (string Symbol, string Name, string Description);

public record TransactionResponse(Agent Agent, ShipCargo Cargo, MarketTransaction Transaction);
#endregion

#region Ships
public record ScannedShip(string Symbol, ShipRegistration Registration, ShipNav Nav, ShipFrame Frame, ShipReactor Reactor, ShipEngine Engine, ShipMount[] Mounts);
public record Ship(string Symbol, ShipRegistration Registration, ShipNav Nav, ShipCrew Crew, ShipFrame Frame, ShipReactor Reactor, ShipEngine Engine, ShipModule[] Modules, ShipMount[] Mounts, ShipCargo Cargo, ShipFuel Fuel);
public record ShipCargo(int Capacity, int Units, ShipCargoItem[] Inventory);
public record ShipCargoItem(string Symbol, string Name, string Description, int Units);
public record ShipCrew(int Current, int Required, int Capacity, string Rotation, int Morale, int Wages);
public record ShipEngine(string Symbol, string Name, string Description, int Condition, float Speed, ShipRequirements Requirements);
public record ShipFrame(string Symbol, string Name, string Description, int Condition, int ModuleSlots, int MountingPoints, int FuelCapacity, ShipRequirements Requirements);
public record ShipFuel(int Current, int Capacity, ConsumedFuel Consumed);
public record ConsumedFuel(int Amount, DateTime Timestamp);
public record ShipModule(string Symbol, int Capacity, int Range, string Name, string Description, ShipRequirements Requirements);
public record ShipMount(string Symbol, string Name, string Description, int Strength, string[] Deposits, ShipRequirements Requirements);
public record ShipNav(string SystemSymbol, string WaypointSymbol, ShipNavRoute Route, string Status, string FlightMode);
public record ShipNavRoute(ShipNavRouteWaypoint Destination, ShipNavRouteWaypoint Departure, DateTime DepartureTime, DateTime Arrival);
public record ShipNavRouteWaypoint(string Symbol, string Type, string SystemSymbol, int X, int Y);
public record ShipReactor(string Symbol, string Name, string Description, int Condition, int PowerOutput, ShipRequirements Requirements);
public record ShipRegistration(string Name, string FactionSymbol, string Role);
public record ShipRequirements (int Power, int Crew, int Slots);
public record ShipType(string Type);
public record ToggleDock(ShipNav Nav);
public record NavResponse(ShipFuel Fuel, ShipNav Nav);
public record RefuelResponse(Agent Agent, ShipFuel Fuel, MarketTransaction Transaction);
#endregion

#region Shipyard
public record Shipyard(string Symbol, ShipType[] ShipTypes, ShipyardTransaction[] Transactions, ShipyardShip[] Ships);
public record ShipyardShip(ShipType Type, string Name, string Description, int PurchasePrice, ShipFrame Frame, ShipReactor Reactor, ShipEngine Engine, ShipModule[] Modules, ShipMount[] Mounts);
public record ShipyardTransaction(string WaypointSymbol, string ShipSymbol, int Price, string AgentSymbol, DateTime Timestamp);
#endregion

#region Survey And Extraction
public record Survey(string Signature, string Symbol, SurveyDeposit[] Deposits, DateTime Expiration, string Size);
public record SurveyDeposit(string Symbol);
public record Extraction (string Name, ExtractionYield Yield);
public record ExtractionYield (string Name, int Units);
#endregion

#region Systems
public record ScannedSystem (string Symbol, string SectorSymbol, string Type, int X, int Y, int Distance);

public record System
{
    [PrimaryKey] public string Symbol { get; set; }
    [Indexed] public string SectorSymbol { get; set; }
    public string Type { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    [OneToMany(CascadeOperations = CascadeOperation.All)] public List<SystemWaypoint> Waypoints { get; set; }
    [Ignore] public string[] Factions { get; set; }
}
public record SystemFaction
{
    public string Symbol { get; set; }
    public string System { get; set; }
}
public record SystemType(string Type);
public record SystemWaypoint
{
    [ForeignKey(typeof(System))] public string System { get; set; }
    [PrimaryKey] public string Symbol { get; set; }
    public string Type { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}

public record ConnectedSystem ([property: JsonPropertyName("symbol")] string Name, [property: JsonPropertyName("sectorSymbol")] string SectorName,
    [property: JsonPropertyName("type")] string Type, [property: JsonPropertyName("factionSymbol")] string FactionName,
    [property: JsonPropertyName("x")] int X, [property: JsonPropertyName("y")] int Y, [property: JsonPropertyName("distance")] int Distance);
#endregion

#region Waypoints
public record Chart ([property: JsonPropertyName("waypointSymbol")] string WayPointSymbol, [property: JsonPropertyName("submittedBy")] string SubmittedBy,
    [property: JsonPropertyName("submittedOn")] DateTime SubmittedOn);
public record JumpGate (int JumpRange, string Faction, ConnectedSystem[] ConnectedSystems);
public record ScannedWaypoint (string Symbol, string Type, string SystemSymbol, int X, int Y, WaypointOrbital[] Orbitals, WaypointFaction Faction, WaypointTrait[] Traits, Chart Chart);
public record Waypoint(string Symbol, string Type, string SystemSymbol, int X, int Y, WaypointOrbital[] Orbitals, WaypointFaction Faction, WaypointTrait[] Traits, Chart Chart);
public record WaypointFaction(string Symbol);
public record WaypointOrbital(string Symbol);
public record WaypointTrait(string Symbol, string Name, string Description);
public record WaypointType(string Symbol);
#endregion

#region System Status
public record Link(string Name, string Url);
public record Announcement(string Title, string Body);
public record ServerReset(DateTime Next, string Frequency);
public record CreditsLeaderboard(string AgentSymbol, int Credits);
public record ChartsLeaderboard(string AgentSymbol, int ChartCount);
public record Leaderboards (CreditsLeaderboard[] MostCredits, ChartsLeaderboard[] MostSubmittedCharts);
public record Stats(int Agents, int Ships, int Systems, int Waypoints);
public record GameStatus(string Status, string Version, DateTime ResetDate, string Description, Stats Stats, Leaderboards Leaderboards, ServerReset ServerResets, Announcement[] Announcements, Link[] Links);

#endregion