using Microsoft.Extensions.Logging;
using SQLiteNetExtensionsAsync.Extensions;

namespace SpaceTrader.Data;
internal class AgentDbController : BaseDbController
{
    public AgentDbController(string path, ApiClient client, ILogger<BaseDbController> logger) : base(path, client, logger) { }

    protected override async Task Initialize()
    {
        if(_isInitialized) return;

        _isInitialized = true;
        await _connection.CreateTableAsync<Agent>();
        await _connection.CreateTableAsync<ShipData>();
    }

    public async Task Insert (Agent data)
    {
        await Initialize();

        if(string.IsNullOrWhiteSpace(data.Token))
        {
            var agent = await _connection.GetAsync<Agent>(data.AccountID);
            data.Token = agent.Token;
        }

        await _connection.InsertOrReplaceWithChildrenAsync(data, true);

        if (_client.LoggedInAgent.AccountID == data.AccountID)
        {
            _client.Login(data);
        }
    }
}
