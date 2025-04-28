using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Agents.Hosting.AspNetCore;
using Microsoft.Agents.Builder;
using Microsoft.Agents.Core.Models;
using Microsoft.Agents.Storage;
using Onboardy.Contracts;

namespace Obnoarding;

[Authorize]
[ApiController]
[Route("api/messages")]
public class AgentIncomingController(IAgentHttpAdapter adapter, IAgent agent, IChannelAdapter channel, IStorage storage) : ControllerBase
{
    [HttpPost]
    public Task PostAsync(CancellationToken cancellationToken)
        => adapter.ProcessAsync(Request, Response, agent, cancellationToken);

    [Route("/push")]
    [HttpPost]
    public async Task<IActionResult> ProcessPushNotification([FromBody] NotificationDto notification, CancellationToken cancellationToken)
    {
        var data = await storage.ReadAsync<ConversationReference>([notification.UserId]);
        var conversationReference = data[notification.UserId];
        if (conversationReference == null)
        {
            return BadRequest("No active conversation found.");
        }

        await channel.ContinueConversationAsync(
            claimsIdentity: new System.Security.Claims.ClaimsIdentity(),
            reference: conversationReference,
            callback: async (turnContext, ct) =>
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(notification.Text), ct);
            },
            cancellationToken: cancellationToken
        );

        return Ok("Message sent.");
    }

}
