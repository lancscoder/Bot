using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BotApi.Controllers
{
    [Route("api/version")]
    public class VersionController
    {
        private readonly ILogger<VersionController> _logger;

        public VersionController(ILogger<VersionController> logger)
        {
            _logger = logger;
        }

        public Task<string> Get()
        {
            try
            {
                _logger.LogInformation("Version: GET");

                var versionAttribute = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();

                if (versionAttribute == null)
                {
                    return Task.FromResult("Unknown");
                }

                var version = versionAttribute.InformationalVersion;

                return Task.FromResult(version);
            }
            catch (Exception ex)
            {
                return Task.FromResult(ex.Message);
            }
        }
    }
}