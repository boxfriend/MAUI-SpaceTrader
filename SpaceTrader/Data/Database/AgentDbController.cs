using Microsoft.Extensions.Logging;
using Serilog;
using SQLite;
using SQLiteNetExtensions.Extensions;
using SQLiteNetExtensionsAsync.Extensions;

namespace SpaceTrader.Data;
internal class AgentDbController
{
    private readonly string _dbPath;
    private SQLiteAsyncConnection _connection;

    private readonly ApiClient _client;
    private readonly ILogger<AgentDbController> _logger;

    public AgentDbController(string path, ApiClient client, ILogger<AgentDbController> logger)
    {
        _dbPath = path;
        _client = client;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        
        if (_connection != null)
            return;

        _connection = new(_dbPath);

        await _connection.CreateTableAsync<Agent>();
        await _connection.CreateTableAsync<Contract>();
        await _connection.CreateTableAsync<ContractTerms>();
        await _connection.CreateTableAsync<ContractPayment>();
        await _connection.CreateTableAsync<ContractDeliverGood>();

    }

    public async Task<List<Agent>> GetAll ()
    {
        await InitializeAsync();
        return await _connection.Table<Agent>().ToListAsync();
    }

    public async Task<Agent> Get(string accountID)
    {
        await InitializeAsync();
        return await _connection.FindAsync<Agent>(accountID);
    }
    public async Task<Agent> Create (Agent data)
    {
        await InitializeAsync();
        await _connection.InsertAsync(data);
        return data;
    }

    public async Task Update (Agent agent)
    {
        await InitializeAsync();
        var agentData = await Get(agent.AccountID);
        await _connection.UpdateAsync(agentData);

        if(_client.LoggedInAgent.AccountID == agentData.AccountID)
        {
            _client.Login(agentData);
        }
    }

    public async Task Delete (Agent data) => await _connection.DeleteAsync(data);

    public async Task InsertContracts(params Contract[] contracts)
    {
        await InitializeAsync();
        foreach (var contract in contracts)
        {
            contract.Terms.ID = contract.ID;
            contract.Terms.Payment.ID = contract.ID;

            foreach (var good in contract.Terms.Deliveries)
            {
                good.TermsID = contract.ID;
            }
        }
        await _connection.InsertOrReplaceAllWithChildrenAsync(contracts, true);
    }

    public async Task<Contract> GetContract(string contractID)
    {
        await InitializeAsync();
        try
        {
            return await _connection.GetWithChildrenAsync<Contract>(contractID,true);
        }
        catch(Exception ex)
        {
            _logger.LogCritical(ex.Message);
            throw;
        }
    }
}