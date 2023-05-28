using SQLite;

namespace SpaceTrader.Data;
internal class AgentDbController
{
    private readonly string _dbPath;
    private SQLiteAsyncConnection _connection;

    private readonly ApiClient _client;

    public AgentDbController(string path, ApiClient client)
    {
        _dbPath = path;
        _client = client;
    }

    public async Task InitializeAsync()
    {
        
        if (_connection != null)
            return;

        _connection = new(_dbPath);

        await _connection.CreateTableAsync<Agent>();
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
}
