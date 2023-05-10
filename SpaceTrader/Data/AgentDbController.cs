﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace SpaceTrader.Data;
internal class AgentDbController
{
    private readonly string _dbPath;
    private SQLite.SQLiteAsyncConnection _connection;

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
    public async Task<AgentData> Create (AgentData data)
    {
        await _connection.InsertAsync(data);
        return data;
    }

    public async Task Delete (AgentData data) => await _connection.DeleteAsync(data);
}

public class AgentData
{
    [PrimaryKey,AutoIncrement] public int ID { get; set; }
    [Unique] public string Name { get; set; }
    [Unique] public string Token { get; set; }
}