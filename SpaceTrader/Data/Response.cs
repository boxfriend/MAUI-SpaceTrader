using System.Text.Json.Serialization;

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
public record Agent([property:JsonPropertyName("accountId")]string AccountID, [property: JsonPropertyName("symbol")] string Name, 
    [property: JsonPropertyName("headquarters")] string Headquarters, [property: JsonPropertyName("credits")] int Credits);
public record Faction (string Symbol, string Name, string Description, string Headquarters, FactionTrait[] Traits);
public record FactionTrait (string Type, string Name, string Description);
#endregion

#region Contracts
public record ContractAccept(Agent Agent, Contract Contract);
public record Contract([property: JsonPropertyName("id")] string ID, [property: JsonPropertyName("factionSymbol")] string Faction, 
    [property: JsonPropertyName("type")] string Type, [property: JsonPropertyName("terms")] ContractTerms Terms, [property: JsonPropertyName("accepted")] bool Accepted,
    [property: JsonPropertyName("fulfilled")] bool Fulfilled, [property: JsonPropertyName("deadlineToAccept")] DateTime Expiration);
public record ContractTerms([property: JsonPropertyName("deadline")] DateTime Deadline, [property: JsonPropertyName("payment")] ContractPayment Payment,
    [property: JsonPropertyName("deliver")] ContractDeliverGood[] Deliveries);
public record ContractPayment([property: JsonPropertyName("onAccepted")] int OnAccepted, [property: JsonPropertyName("onFulfilled")] int OnFulfilled);
public record ContractDeliverGood([property: JsonPropertyName("tradeSymbol")] string Name, [property: JsonPropertyName("destinationSymbol")] string Destination,
    [property: JsonPropertyName("unitsRequired")] int Required, [property: JsonPropertyName("unitsFulfilled")] int Fulfilled);
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
public record System(string Symbol, string SectorSymbol, string Type, int X, int Y, SystemWaypoint[] Waypoints, SystemFaction[] Factions);
public record SystemFaction(string Symbol);
public record SystemType(string Type);
public record SystemWaypoint(string Symbol, string Type, int X, int Y);

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