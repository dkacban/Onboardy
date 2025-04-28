using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text;
using Obnoarding.Plugins;
using Microsoft.Agents.Storage;
using OnboardyAgent.Plugins;

namespace Obnoarding.Agents;

public class OnboardingPlanAgent
{
    private readonly Kernel _kernel;
    private readonly ChatCompletionAgent _agent;
    private int retryCount;
    private IStorage _storage;

    private const string AgentName = "ObnoardingPlanAgent";
    private const string AgentInstructions = """
        You are a friendly assistant that helps new hires to get their obboarding plan.
        You may ask follow up questions until you have enough information to answer the employee question,
        but once you have a schedule, make sure to format it nicely using text.

        Employee can belong only to 1 of 2 departments:
        - "it"
        - "finance"

        """;

    public OnboardingPlanAgent(Kernel kernel, IStorage storage)
    {
        _kernel = kernel;
        _storage = storage;

        _agent = new ChatCompletionAgent()
        {
            Instructions = AgentInstructions,
            Name = AgentName,
            Kernel = _kernel,
            Arguments = new KernelArguments(
                new OpenAIPromptExecutionSettings() 
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(), 
                    ResponseFormat = "text" 
                }
            )
        };

        _agent.Kernel.Plugins.Add(KernelPluginFactory.CreateFromType<DateTimePlugin>());
        _agent.Kernel.Plugins.Add(KernelPluginFactory.CreateFromObject(new OnboardingSchedulePlugin(_storage)));
        _agent.Kernel.Plugins.Add(KernelPluginFactory.CreateFromObject(new SmsPlugin()));
    }

    public async Task<string> InvokeAgentAsync(string input, ChatHistory chatHistory, string userId)
    {
        ArgumentNullException.ThrowIfNull(chatHistory);

        ChatMessageContent systemMessage = new(AuthorRole.System, $"userId is: {userId}");
        chatHistory.Add(systemMessage);

        ChatMessageContent message = new(AuthorRole.User, input);
        chatHistory.Add(message);

        StringBuilder sb = new();
        await foreach (var response in _agent.InvokeAsync(chatHistory))
        {
            chatHistory.Add(response);
            sb.Append(response.Content);
        }

        return sb.ToString();
    }
}
