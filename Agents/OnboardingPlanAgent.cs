using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text;
using System.Text.Json;
using System;
using Obnoarding.Plugins;

namespace Obnoarding.Agents;

public class OnboardingPlanAgent
{
    private readonly Kernel _kernel;
    private readonly ChatCompletionAgent _agent;
    private int retryCount;

    private const string AgentName = "ObnoardingPlanAgent";
    private const string AgentInstructions = """
        You are a friendly assistant that helps new hires to get their obboarding plan.
        You may ask follow up questions until you have enough information to answer the employee question,
        but once you have a schedule, make sure to format it nicely using text.

        Respond in JSON format with the following JSON schema:
        
        {
            "contentType": "'Text',
            "content": "{The content of the response - plain text}"
        }

        Employee can belong only to 1 of 2 departments:
        - "it"
        - "finance"
        """;

    public OnboardingPlanAgent(Kernel kernel)
    {
        _kernel = kernel;

        _agent = new ChatCompletionAgent()
        {
            Instructions = AgentInstructions,
            Name = AgentName,
            Kernel = _kernel,
            Arguments = new KernelArguments(
                new OpenAIPromptExecutionSettings() 
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(), 
                    ResponseFormat = "json_object" 
                }
            )
        };

        _agent.Kernel.Plugins.Add(KernelPluginFactory.CreateFromType<DateTimePlugin>());
        _agent.Kernel.Plugins.Add(KernelPluginFactory.CreateFromType<OnboardingSchedulePlugin>());
    }

    public async Task<ObnoardingPlanAgentResponse> InvokeAgentAsync(string input, ChatHistory chatHistory)
    {
        ArgumentNullException.ThrowIfNull(chatHistory);

        ChatMessageContent message = new(AuthorRole.User, input);
        chatHistory.Add(message);

        StringBuilder sb = new();
        await foreach (var response in _agent.InvokeAsync(chatHistory))
        {
            chatHistory.Add(response);
            sb.Append(response.Content);    
        }

        try
        {
            var resultContent = sb.ToString();
            var result = JsonSerializer.Deserialize<ObnoardingPlanAgentResponse>(resultContent);
            retryCount = 0;
            return result;
        }
        catch (JsonException je)
        {
            if (this.retryCount > 2)
            {
                throw;
            }

            this.retryCount++;

            return await InvokeAgentAsync($"That response did not match the expected format. Please try again. Error: {je.Message}", chatHistory);
        }
    }
}
