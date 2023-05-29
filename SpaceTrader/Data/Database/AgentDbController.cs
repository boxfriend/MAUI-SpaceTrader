using Microsoft.Extensions.Logging;
using Serilog;
using SQLite;
using SQLiteNetExtensions.Extensions;
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
    }

    public async Task Insert (Agent data)
    {
        await Initialize();
        await _connection.InsertOrReplaceWithChildrenAsync(data, true);

        if (_client.LoggedInAgent.AccountID == data.AccountID)
        {
            _client.Login(data);
        }
    }
}
