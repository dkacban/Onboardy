using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Obnoarding.Agents;

public enum ObnoardingPlanAgentResponseContentType
{
    [JsonPropertyName("text")]
    Text,

    [JsonPropertyName("adaptive-card")]
    AdaptiveCard
}

public class ObnoardingPlanAgentResponse
{
    [JsonPropertyName("contentType")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ObnoardingPlanAgentResponseContentType ContentType { get; set; }

    [JsonPropertyName("content")]
    [Description("The content of the response, may be plain text, or JSON based adaptive card but must be a string.")]
    public string Content { get; set; }
}
