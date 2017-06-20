using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using BotApi.Models;
using System;
using Microsoft.Extensions.Configuration;

namespace BotApi.Repository
{
    public class LoggingRepository : IRepository<LoggingItem>
    {
        private readonly DocumentClient _client;
        private readonly string _environment;

        public LoggingRepository(IConfiguration configuration)
        {
            // TODO  Pass this in as options somehow....
            var url = configuration.GetValue<string>("RPS_DB_URL");
            var key = configuration.GetValue<string>("RPS_KEY");
            _environment = configuration.GetValue<string>("RPS_ENVRIONMENT");

            _client = new DocumentClient(new Uri(url), key);
        }

        public async Task<Document> CreateDcumentAsync(LoggingItem model)
        {
            model.Environment = _environment;
            return await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("RpsLogging", "Logs"), model);
        }
    }
}