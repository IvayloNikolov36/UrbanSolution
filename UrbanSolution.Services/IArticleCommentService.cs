namespace UrbanSolution.Services
{
    using Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IArticleCommentService
    {
        Task<TModel> SubmitAsync<TModel>(int articleId, string userId, string content);

        Task<TModel> GetAsync<TModel>(int id);

        Task<IEnumerable<TModel>> AllAsync<TModel>(int articleId);

        Task<bool> DeleteAsync(int commentId);
    }
}
