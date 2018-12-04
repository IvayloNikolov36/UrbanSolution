using System.Threading.Tasks;
using UrbanSolution.Services.Models;

namespace UrbanSolution.Services
{
    public interface IResolvedService
    {
        Task<ResolvedDetailsServiceModel> GetAsync(int id);
    }
}
