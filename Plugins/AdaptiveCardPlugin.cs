using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Threading.Tasks;

namespace Obnoarding.Plugins;

public class AdaptiveCardPlugin
{

    [KernelFunction]
    public async Task<string> GetAdaptiveCardForData(Kernel kernel, string data)
    {
        var instruction = """
            When given data about the weather forecast for a given time and place, please generate an adaptive card
            that displays the information in a visually appealing way. Make sure to only return the valid adaptive card
            JSON string in the response.
            """;

        ChatHistory chat = new(instruction)
        {
            new ChatMessageContent(AuthorRole.User, data)
        };

        var chatCompletion = kernel.GetRequiredService<IChatCompletionService>();
        var response = await chatCompletion.GetChatMessageContentAsync(chat);

        return response.ToString();
    }
}
