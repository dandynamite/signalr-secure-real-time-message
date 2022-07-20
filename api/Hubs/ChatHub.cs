using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SignalRTest.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly CosmosService _db;

    public ChatHub(CosmosService db)
    {
        _db = db;
    }

    public override async Task OnConnectedAsync()
    {
        await _db.UpsertItem(Context.ConnectionId);
        await base.OnConnectedAsync();
    }
}
