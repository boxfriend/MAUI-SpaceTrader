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

    private AgentData _loggedInAgent = null;
    private JwtAuthenticator _authenticator = null;

    public ApiClient(ILogger<ApiClient> logger)
    {
        var clientOptions = new RestClientOptions(@"https://api.spacetraders.io/v2");
        var serializeOptions = new JsonSerializerOptions(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        _client = new RestClient(clientOptions, configureSerialization: s => s.UseSystemTextJson(serializeOptions));
        _logger = logger;
    }

    public string DisplayName => _loggedInAgent?.Name ?? "SpaceTraders";

    public bool IsLoggedInAgent(AgentData agent) => _loggedInAgent is not null && _loggedInAgent.AccountID == agent.AccountID;
    public async Task<AgentData> RetrieveAgent (string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("Unable to retrieve agent with an empty token.");

        var request = new RestRequest(@"my/agent", Method.Get)
        {
            Authenticator = new JwtAuthenticator(token)
        };
        var response = await GetRequest<Agent>(request);
        return response is not null ? AgentData.FromAPIAgent(response.Data, token) : null;
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

    public void Login(AgentData agent)
    {
        _loggedInAgent = agent;
        _authenticator = new(agent.Token);
    }
}
