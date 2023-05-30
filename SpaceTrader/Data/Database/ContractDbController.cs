using Microsoft.Extensions.Logging;
using SQLiteNetExtensionsAsync.Extensions;

namespace SpaceTrader.Data;

internal class ContractDbController : BaseDbController
{
    public ContractDbController (string path, ApiClient client, ILogger<BaseDbController> logger) : base(path, client, logger) { }

    protected override async Task Initialize ()
    {
        await _connection.CreateTableAsync<Contract>();
        await _connection.CreateTableAsync<ContractTerms>();
        await _connection.CreateTableAsync<ContractPayment>();
        await _connection.CreateTableAsync<ContractDeliverGood>();
    }

    public async Task<List<Contract>> GetAllFromAgent(Agent agent) => await GetAllFromAgent(agent.AccountID);
    public async Task<List<Contract>> GetAllFromAgent (string agentID) => await _connection.GetAllWithChildrenAsync<Contract>(c => c.AccountID == agentID, true);

    public async Task Insert (params Contract[] contracts)
    {
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

}