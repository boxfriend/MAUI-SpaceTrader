using Microsoft.Extensions.Logging;
using Serilog;
using SQLite;

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
        await _connection.InsertAsync(data);
        return data;
    }

    public async Task Update (Agent agent)
    {
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
        foreach (var contract in contracts)
        {
            var payment = contract.Terms.Payment;
            await _connection.InsertOrReplaceAsync(payment);
            var terms = contract.Terms;
            terms.PaymentID = payment.ID;
            await _connection.InsertOrReplaceAsync(terms);

            foreach(var good in contract.Terms.Goods)
            {
                good.TermsID = terms.ID;
                await _connection.InsertOrReplaceAsync(good);
            }
            contract.TermsID = terms.ID;
            await _connection.InsertOrReplaceAsync(contract);
        }
    }
}
