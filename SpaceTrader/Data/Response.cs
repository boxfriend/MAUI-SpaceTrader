using System.Text.Json.Serialization;

namespace SpaceTrader.Data;
internal class Response<T> where T : ApiObject
{
    [JsonPropertyName("data")] public T Data { get; set; }
    [JsonPropertyName("meta")] public Meta Meta { get; set; }
}

internal record Meta(int Total, int Page, int Limit);

internal abstract record ApiObject();

internal record Agent([property:JsonPropertyName("accountId")]string AccountID, [property: JsonPropertyName("symbol")] string Name, 
    [property: JsonPropertyName("headquarters")] string Headquarters, [property: JsonPropertyName("credits")] int Credits) : ApiObject;
