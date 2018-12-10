namespace UrbanSolution.Services.Blog
{
    using Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IBlogArticleService
    {
        Task<IEnumerable<BlogArticleListingServiceModel>> AllAsync(int page = 1);

        Task<int> TotalAsync();

        Task<BlogArticleDetailsServiceModel> GetAsync(int id);

        Task CreateAsync(string title, string content, string authorId);
    }
}
