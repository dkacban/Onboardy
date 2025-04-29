using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text;
using Obnoarding.Plugins;
using Microsoft.Agents.Storage;
using OnboardyAgent.Plugins;
using Microsoft.Extensions.Options;

namespace Obnoarding.Agents;

public class OnboardingPlanAgent
{
    private readonly Kernel _kernel;
    private readonly ChatCompletionAgent _agent;
    private int retryCount;
    private IStorage _storage;

    private const string AgentName = "ObnoardingPlanAgent";
    private const string AgentInstructions = """
        You are a friendly assistant at Microsoft that helps new hires to get their obboarding plan.
        You may ask follow up questions until you have enough information to answer the employee question,
        but once you have a schedule, make sure to format it nicely using text.
        You must keep your answers concise and extremely polite.


        RULES:
        Employee can belong only to 1 of 2 departments:
        - "it"
        - "finance"

        Whenever you answer user's question using your knowledge, you should provice clickable document path.
        Whenever user asks for a knowledge, or documents, you must searh your knowledge.

        When you create the schedule for employee, ask if they want to get SMS summary.

        when user starts conversation, ask it for 3 things:
        - phone number
        - department
        """;

    public OnboardingPlanAgent(Kernel kernel, IStorage storage, IOptions<AzureAISearchSettings> aiSearchSettings)
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
        _agent.Kernel.Plugins.Add(KernelPluginFactory.CreateFromObject(new KnowledgePlugin(aiSearchSettings)));
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
