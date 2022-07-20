using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRTest.Hubs;

namespace SignalRTest.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly IHubContext<ChatHub> _chatHub;
    private readonly CosmosService _db;

    public TestController(IHubContext<ChatHub> chatHub, CosmosService db)
    {
        _db = db;
        _chatHub = chatHub;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var message = new
        {
            user = new Random().Next(111, 999),
            message = Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID") ?? "localhost"
        };

        await _chatHub.Clients.Client(await _db.GetItem())
            .SendAsync("SendDataToClient", message);

        return Ok(message);
    }
}