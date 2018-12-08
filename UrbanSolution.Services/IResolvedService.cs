namespace UrbanSolution.Services
{
    using Models;
    using System.Threading.Tasks;

    public interface IResolvedService
    {
        Task<ResolvedDetailsServiceModel> GetAsync(int id);
    }
}
