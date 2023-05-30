using Microsoft.Extensions.Logging;
using SQLiteNetExtensionsAsync.Extensions;

namespace SpaceTrader.Data;
internal class AgentDbController : BaseDbController
{
    public AgentDbController(string path, ApiClient client, ILogger<BaseDbController> logger) : base(path, client, logger) { }

    protected override async Task Initialize()
    {
        await _connection.CreateTableAsync<Agent>();
    }

    public async Task Insert (Agent data, bool recursive = true)
    {
        if(string.IsNullOrWhiteSpace(data.Token))
        {
            var agent = await _connection.GetAsync<Agent>(data.AccountID);
            data.Token = agent.Token;
        }

        if(recursive)
            await _connection.InsertOrReplaceWithChildrenAsync(data, recursive);
        else
            await _connection.InsertOrReplaceAsync(data);

        if (_client.LoggedInAgent.AccountID == data.AccountID)
        {
            _client.Login(data);
        }
    }
}
