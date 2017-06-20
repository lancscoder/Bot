using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace BotApi.Repository
{
    public interface IRepository<TModel> where TModel : class
    {
        Task<Document> CreateDcumentAsync(TModel model);
    }
}