using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace SpaceTrader.Data;
internal abstract class BaseDbController
{
    protected static string _dbPath;
    protected static SQLiteAsyncConnection _connection;

    protected static ApiClient _client;
    protected static ILogger<BaseDbController> _logger;

    protected bool _isInitialized;

    public BaseDbController (string path, ApiClient client, ILogger<BaseDbController> logger)
    {
        _dbPath ??= path;
        _client ??= client;
        _logger ??= logger;
        _connection ??= new(_dbPath);
    }

    protected abstract Task Initialize ();
    public virtual async Task<T> Get<T> (string symbol) where T : new()
    {
        await Initialize();
        return await _connection.GetWithChildrenAsync<T>(symbol, true);
    }

    public virtual async Task<List<T>> GetAll<T> (Expression<Func<T,bool>> filter = null) where T : new()
    {
        await Initialize();
        return await _connection.GetAllWithChildrenAsync<T>(filter, true);
    }

    public virtual async Task Delete<T> (T data)
    {
        await Initialize ();
        await _connection.DeleteAsync(data, true);
    }

    public async Task Insert<T> (params T[] data)
    {
        await Initialize();
        await _connection.InsertOrReplaceAllWithChildrenAsync(data, true);
    }
}
