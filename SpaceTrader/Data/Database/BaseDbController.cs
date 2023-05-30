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

    public BaseDbController (string path, ApiClient client, ILogger<BaseDbController> logger)
    {
        _dbPath ??= path;
        _client ??= client;
        _logger ??= logger;
        _connection ??= new(_dbPath);

        Task.Run(Initialize);
    }

    protected abstract Task Initialize ();
    public virtual async Task<T> Get<T> (string symbol) where T : new()
    {
        return await _connection.GetWithChildrenAsync<T>(symbol, true);
    }

    public virtual async Task<List<T>> GetAll<T> (Expression<Func<T,bool>> filter = null) where T : new()
    {
        return await _connection.GetAllWithChildrenAsync<T>(filter, true);
    }

    public virtual async Task Delete<T> (T data)
    {
        await _connection.DeleteAsync(data, true);
    }

    public async Task Insert<T> (params T[] data)
    {
        await _connection.InsertOrReplaceAllWithChildrenAsync(data, true);
    }
}
