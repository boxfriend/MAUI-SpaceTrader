using System.Text.Json;
using Microsoft.Extensions.Logging;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.Json;

namespace SpaceTrader.Data;
internal class ApiClient
{
    private readonly RestClient _client;
    private readonly ILogger<ApiClient> _logger;

    public Agent LoggedInAgent
    {
        get => _loggedInAgent; 
        private set
        {
            _loggedInAgent = value;
            LoggedInAgentCompleted?.Invoke(value);
        }
    }
    private JwtAuthenticator _authenticator = null;
    private Agent _loggedInAgent;

    public event Action<Agent> LoggedInAgentCompleted;

    public ApiClient(ILogger<ApiClient> logger)
    {
        var clientOptions = new RestClientOptions(@"https://api.spacetraders.io/v2");
        var serializeOptions = new JsonSerializerOptions(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        _client = new RestClient(clientOptions, configureSerialization: s => s.UseSystemTextJson(serializeOptions));
        _logger = logger;
    }

    public string DisplayName => LoggedInAgent?.Name ?? "SpaceTraders";

    public bool IsLoggedInAgent(Agent agent) => LoggedInAgent is not null && LoggedInAgent.AccountID == agent.AccountID;
    public async Task<Agent> RetrieveAgent (string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("Unable to retrieve agent with an empty token.");

        var request = new RestRequest(@"my/agent", Method.Get)
        {
            Authenticator = new JwtAuthenticator(token)
        };
        var response = await GetRequest<Agent>(request);
        if (response is null)
            return null;

        var agent = response.Data;
        agent.Token = token;
        return agent;
    }

    public async Task<Response<T>> GetRequest<T>(RestRequest request, bool authenticate = false)
    {
        if (authenticate)
            request.Authenticator = _authenticator;

        try
        {
            var response = await _client.ExecuteGetAsync<Response<T>>(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(response.StatusDescription);
            }

            return response.IsSuccessful ? response.Data : null;

        } catch (Exception ex)
        {
            _logger.LogCritical(ex.Message);
            return null;
        }
    }

    public async Task<Response<T>> PostRequest<T> (RestRequest request, bool authenticate = false)
    {
        if(authenticate)
            request.Authenticator = _authenticator;

        try
        {
            var response = await _client.ExecutePostAsync<Response<T>>(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(response.StatusDescription);
            }

            return response.IsSuccessful ? response.Data : null;

        } catch (Exception ex)
        {
            _logger.LogCritical(ex.Message);
            return null;
        }
    }

    public async Task<Registration> RegisterNewAgent(string name, string faction)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidOperationException("Unable to retrieve agent with an empty token.");

        var request = new RestRequest(@"register", Method.Post);
        if (string.IsNullOrWhiteSpace(faction))
            faction = "COSMIC";

        request.AddJsonBody(new { symbol = name, faction });

        var response = await PostRequest<Registration>(request);
        return response?.Data;
    }

    public void Login(Agent agent)
    {
        LoggedInAgent = agent;
        _authenticator = new(agent?.Token ?? "INVALID");
    }

    public async Task<GameStatus> GetStatus()
    {
        var request = new RestRequest("", Method.Get);

        try
        {
            var response = await _client.ExecuteGetAsync<GameStatus>(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(response.StatusDescription);
            }

            return response.IsSuccessful ? response.Data : null;

        } catch (Exception ex)
        {
            _logger.LogCritical(ex.Message);
            return null;
        }
    }

    public async Task<List<System>> GetAllSystems()
    {
        var request = new RestRequest("systems.json", Method.Get);

        try
        {
            var response = await _client.ExecuteGetAsync<List<System>>(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(response.StatusDescription);
            }

            return response.IsSuccessful ? response.Data : null;

        } catch (Exception ex)
        {
            _logger.LogCritical(ex.Message);
            return null;
        }
    }
}
