using hackaton.Models.Injectors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
namespace hackaton.Models.WebSocket
{
    public class RedirectClient:Hub
    {
        [ServiceFilter(typeof(RequireLoginAttributeFactory))]
        public async Task addClientToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
        [ServiceFilter(typeof(RequireLoginAttributeFactory))]
        public async Task removeClientToGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

    }
}
