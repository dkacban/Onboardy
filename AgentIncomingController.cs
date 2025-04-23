using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Agents.Hosting.AspNetCore;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Agents.Builder;
using Microsoft.Agents.Core.Models;
using OnboardyAgent;

namespace Obnoarding;

[Authorize]
[ApiController]
[Route("api/messages")]
public class AgentIncomingController(IAgentHttpAdapter adapter, IAgent agent, IChannelAdapter channel) : ControllerBase
{
    [HttpPost]
    public Task PostAsync(CancellationToken cancellationToken)
        => adapter.ProcessAsync(Request, Response, agent, cancellationToken);

    [Route("/push")]
    [HttpGet]
    public async Task<IActionResult> SendMessage([FromBody] string message, CancellationToken cancellationToken)
    {
        var conversationReference = ConversationReferenceStorage.Get();
        if (conversationReference == null)
        {
            return BadRequest("No active conversation found.");
        }

        await channel.ContinueConversationAsync(
            claimsIdentity: new System.Security.Claims.ClaimsIdentity(),
            reference: conversationReference,
            callback: async (turnContext, ct) =>
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(message), ct);
            },
            cancellationToken: cancellationToken
        );

        return Ok("Message sent.");
    }

}
