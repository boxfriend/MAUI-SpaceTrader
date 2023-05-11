using System.Text.Json.Serialization;

namespace SpaceTrader.Data;
internal class Response<T> where T : ApiObject
{
    [JsonPropertyName("data")] public T Data { get; set; }
    [JsonPropertyName("meta")] public Meta Meta { get; set; }
}

internal record Meta(int Total, int Page, int Limit);

internal abstract record ApiObject();

internal record RegisterData(string Symbol, string Faction);
internal record Registration(Agent Agent, Contract Contract, Faction Faction, Ship Ship, string Token) : ApiObject;

internal record Agent([property:JsonPropertyName("accountId")]string AccountID, [property: JsonPropertyName("symbol")] string Name, 
    [property: JsonPropertyName("headquarters")] string Headquarters, [property: JsonPropertyName("credits")] int Credits) : ApiObject;

internal record Chart([property:JsonPropertyName("waypointSymbol")]string WayPointSymbol, [property: JsonPropertyName("submittedBy")] string SubmittedBy, 
    [property: JsonPropertyName("submittedOn")] DateTime SubmittedOn) : ApiObject;

internal record ConnectedSystem([property: JsonPropertyName("symbol")] string Name, [property: JsonPropertyName("sectorSymbol")] string SectorName, 
    [property: JsonPropertyName("type")] string Type, [property: JsonPropertyName("factionSymbol")] string FactionName,
    [property: JsonPropertyName("x")] int X, [property: JsonPropertyName("y")] int Y, [property: JsonPropertyName("distance")] int Distance) : ApiObject;

internal record Contract([property: JsonPropertyName("id")] string ID, [property: JsonPropertyName("factionSymbol")] string Faction, 
    [property: JsonPropertyName("type")] string Type, [property: JsonPropertyName("terms")] ContractTerms Terms, [property: JsonPropertyName("accepted")] bool Accepted,
    [property: JsonPropertyName("fulfilled")] bool Fulfilled, [property: JsonPropertyName("expiration")] DateTime Expiration) : ApiObject;
internal record ContractTerms([property: JsonPropertyName("deadline")] DateTime Deadline, [property: JsonPropertyName("payment")] ContractPayment Payment,
    [property: JsonPropertyName("deliver")] ContractDeliverGood[] Deliveries) : ApiObject;
internal record ContractPayment([property: JsonPropertyName("onAccepted")] int OnAccepted, [property: JsonPropertyName("onFulfilled")] int OnFulfilled) : ApiObject;
internal record ContractDeliverGood([property: JsonPropertyName("tradeSymbol")] string Name, [property: JsonPropertyName("destinationSymbol")] string Destination,
    [property: JsonPropertyName("unitsRequired")] int Required, [property: JsonPropertyName("unitsFulfilled")] int Fulfilled) : ApiObject;

internal record Cooldown(string Ship, int TotalLength, int RemainingLength, DateTime Expiration) : ApiObject;
internal record Extraction(string Name, ExtractionYield Yield) : ApiObject;
internal record ExtractionYield(string Name, int Units) : ApiObject;

internal record Faction(string Symbol, string Name, string Description, string Headquarters, FactionTrait[] Traits) : ApiObject;
internal record FactionTrait(string Type, string Name, string Description) : ApiObject;
internal record JumpGate(int JumpRange, string Faction, ConnectedSystem[] ConnectedSystems) : ApiObject;
internal record Market(string Symbol, TradeGood[] Exports, TradeGood[] Imports, TradeGood[] Exchange, MarketTransaction[] Transactions, MarketTradeGood[] TradeGoods) : ApiObject;
internal record MarketTradeGood(string Symbol, int TradeVolume, string Supply, int PurchasePrice, int SellPrice) : ApiObject;
internal record MarketTransaction(string Waypoint, string Ship, string Trade, string Type, int Units, int PricePerUnit, int TotalPrice, DateTime Timestamp) :  ApiObject;
internal record ScannedShip(string Symbol, ShipRegistration Registration, ShipNav Nav, ShipFrame Frame, ShipReactor Reactor, ShipEngine Engine, ShipMount[] Mounts);
internal record ScannedSystem(string Symbol, string SectorSymbol, string Type, int X, int Y, int Distance);
internal record ScannedWaypoint(string Symbol, string Type, string SystemSymbol, int X, int Y, WaypointOrbital[] Orbitals, WaypointFaction Faction, WaypointTrait[] Traits, Chart Chart);
internal record Ship(string Symbol, ShipRegistration Registration, ShipNav Nav, ShipCrew Crew, ShipFrame Frame, ShipReactor Reactor, ShipEngine Engine, ShipModule[] Modules, ShipMount[] Mounts, ShipCargo Cargo, ShipFuel Fuel);
internal record ShipCargo(int Capacity, int Units, ShipCargoItem[] Inventory);
internal record ShipCargoItem(string Symbol, string Name, string Description, int Units);
internal record ShipCrew(int Current, int Required, int Capacity, string Rotation, int Morale, int Wages);
internal record ShipEngine(string Symbol, string Name, string Description, int Condition, float Speed, ShipRequirements Requirements);
internal record ShipFrame(string Symbol, string Name, string Description, int Condition, int ModuleSlots, int MountingPoints, int FuelCapacity, ShipRequirements Requirements);
internal record ShipFuel(int Current, int Capacity, ConsumedFuel Consumed);
internal record ConsumedFuel(int Amount, DateTime Timestamp);
internal record ShipModule(string Symbol, int Capacity, int Range, string Name, string Description, ShipRequirements Requirements);
internal record ShipMount(string Symbol, string Name, string Description, int Strength, string[] Deposits, ShipRequirements Requirements);
internal record ShipNav(string SystemSymbol, string WaypointSymbol, ShipNavRoute Route, string Status, string FlightMode);
internal record ShipNavRoute(ShipNavRouteWaypoint Destination, ShipNavRouteWaypoint Departure, DateTime DepartureTime, DateTime Arrival);
internal record ShipNavRouteWaypoint(string Symbol, string Type, string SystemSymbol, int X, int Y);
internal record ShipReactor(string Symbol, string Name, string Description, int Condition, int PowerOutput, ShipRequirements Requirements);
internal record ShipRegistration(string Name, string FactionSymbol, string Role);
internal record ShipRequirements (int Power, int Crew, int Slots);
internal record ShipType(string Type);
internal record Shipyard(string Symbol, ShipType[] ShipTypes, ShipyardTransaction[] Transactions, ShipyardShip[] Ships);
internal record ShipyardShip(ShipType Type, string Name, string Description, int PurchasePrice, ShipFrame Frame, ShipReactor Reactor, ShipEngine Engine, ShipModule[] Modules, ShipMount[] Mounts);
internal record ShipyardTransaction(string WaypointSymbol, string ShipSymbol, int Price, string AgentSymbol, DateTime Timestamp);
internal record Survey(string Signature, string Symbol, SurveyDeposit[] Deposits, DateTime Expiration, string Size);
internal record SurveyDeposit(string Symbol);
internal record System(string Symbol, string SectorSymbol, string Type, int X, int Y, SystemWaypoint[] Waypoints, SystemFaction[] Factions);
internal record SystemFaction(string Symbol);
internal record SystemType(string Type);
internal record SystemWaypoint(string Symbol, string Type, int X, int Y);
internal record TradeGood(TradeSymbol Symbol, string Name, string Description);
internal record TradeSymbol(string Symbol);
internal record Waypoint(string Symbol, string Type, string SystemSymbol, int X, int Y, WaypointOrbital[] Orbitals, WaypointFaction Faction, WaypointTrait[] Traits, Chart Chart);
internal record WaypointFaction(string Symbol);
internal record WaypointOrbital(string Symbol);
internal record WaypointTrait(string Symbol, string Name, string Description);
internal record WaypointType(string Symbol);