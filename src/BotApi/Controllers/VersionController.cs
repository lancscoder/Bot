using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BotApi.Controllers
{
    [Route("api/version")]
    public class VersionController
    {
        public Task<string> Get()
        {
            var version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyVersionAttribute>().Version;

            return Task.FromResult(version);
        }
    }
}