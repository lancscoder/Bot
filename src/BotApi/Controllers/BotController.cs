using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BotApi.Controllers
{
    public class BotController
    {
        [HttpGet("api/bot")]
        public Task<string> Get()
        {
            return Task.FromResult("Blah 3");
        }
    }
}