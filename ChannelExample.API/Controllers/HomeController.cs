using System.Threading.Channels;
using ChannelExample.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChannelExample.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class HomeController : ControllerBase
{

    [HttpGet]
    public IActionResult Send()
    {
        return Ok();
    }
    [HttpGet]
    public Task<bool> SendB([FromServices] Notifications notifications)
    {
        return notifications.Send();
    }
    [HttpGet]
    public bool SendA([FromServices] Notifications notifications)
    {
        return notifications.SendA();
    }
    [HttpGet]
    public async Task<bool> SendC([FromServices] Channel<string> channel)
    {
        await channel.Writer.WriteAsync("Hello");
        return true;
    }
}
