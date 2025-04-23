using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Agents.Hosting.AspNetCore;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Agents.Builder;

namespace Obnoarding;

[Authorize]
[ApiController]
[Route("api/messages")]
public class AgentIncomingController(IAgentHttpAdapter adapter, IAgent agent) : ControllerBase
{
    [HttpPost]
    public Task PostAsync(CancellationToken cancellationToken)
        => adapter.ProcessAsync(Request, Response, agent, cancellationToken);

}
