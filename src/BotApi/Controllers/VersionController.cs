using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BotApi.Controllers
{
    [Route("api/version")]
    public class VersionController
    {
        public Task<string> Get()
        {
            return Task.FromResult("Version 1");
        }
    }
}