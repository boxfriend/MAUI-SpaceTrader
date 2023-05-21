﻿using SQLite;

namespace SpaceTrader.Data;
internal class AgentDbController
{
    private readonly string _dbPath;
    private SQLiteAsyncConnection _connection;

    public AgentDbController(string path)
    {
        _dbPath = path;
    }

    public async Task InitializeAsync()
    {
        
        if (_connection != null)
            return;

        _connection = new(_dbPath);

        await _connection.CreateTableAsync<AgentData>();
    }

    public async Task<List<AgentData>> GetAll ()
    {
        await InitializeAsync();
        return await _connection.Table<AgentData>().ToListAsync();
    }

    public async Task<AgentData> Get(string accountID)
    {
        await InitializeAsync();
        return await _connection.FindAsync<AgentData>(accountID);
    }
    public async Task<AgentData> Create (AgentData data)
    {
        await _connection.InsertAsync(data);
        return data;
    }

    public async Task Update (Agent agent)
    {
        var agentData = await Get(agent.AccountID);
        agentData = AgentData.FromAPIAgent(agent, agentData.Token);
        await _connection.UpdateAsync(agentData);
    }

    public async Task Delete (AgentData data) => await _connection.DeleteAsync(data);
}
