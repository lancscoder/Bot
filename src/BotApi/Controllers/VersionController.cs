using System;
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
            try
            {
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