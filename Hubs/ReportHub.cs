using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace RepoGateway.Hubs
{
    [Authorize]
    public class ReportHub : Hub
    {

    }
}
