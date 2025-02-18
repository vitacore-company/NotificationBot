using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using Telegram.Bot;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NotificationsBot.Controllers;
[Route("api/[controller]")]
[ApiController]
public class BotApi : ControllerBase
{
    //[HttpPost("xml_notafication")]
    //[Consumes("application/xml")]
    //public async Task Notafication([FromBody] XElement xElement)
    //{
    //    string message = "";
    //    return;
    //}

    // GET api/<BotApi>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    // POST api/<BotApi>
    [HttpPost("postReq")]
    public void Post([FromBody] Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Event value)
    {
    }

    // PUT api/<BotApi>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<BotApi>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
