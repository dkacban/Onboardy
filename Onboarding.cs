using Microsoft.Agents.Builder;
using Microsoft.Agents.Builder.App;
using Microsoft.Agents.Builder.State;
using Microsoft.Agents.Core.Models;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Threading;
using System.Threading.Tasks;
using Obnoarding.Agents;

namespace Obnoarding;

public class Onboarding(AgentApplicationOptions options, OnboardingPlanAgent agent) : AgentApplication(options)
{
    [Route(RouteType = RouteType.Conversation, EventName = ConversationUpdateEvents.MembersAdded)]
    protected async Task WelcomeMessageAsync(ITurnContext turnContext, ITurnState turnState, CancellationToken cancellationToken)
    {
        foreach (ChannelAccount member in turnContext.Activity.MembersAdded)
        {
            if (member.Id != turnContext.Activity.Recipient.Id)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Hello! My name is Onboardy. I'm here to help with your onboarding process! I'll be sending you the tasks and do my best to answer your questions about our company."), cancellationToken);
            }
        }
    }

    [Route(RouteType = RouteType.Activity, Type = ActivityTypes.Message, Rank = RouteRank.Last)]
    protected async Task MessageActivityAsync(ITurnContext turnContext, ITurnState turnState, CancellationToken cancellationToken)
    {
        var chatHistory = turnState.GetValue("conversation.chatHistory", () => new ChatHistory());

        var response = await agent.InvokeAgentAsync(turnContext.Activity.Text, chatHistory);
        if (response == null)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text("Sorry, I couldn't answer your queston. Please, try again."), cancellationToken);
            return;
        }

        IActivity ativityResponse = MessageFactory.Text(response.Content);
        await turnContext.SendActivityAsync(ativityResponse, cancellationToken);
    }
}