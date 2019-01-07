namespace UrbanSolution.Services
{
    using Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IArticleCommentService
    {
        Task<CommentListingServiceModel> SubmitAsync(int articleId, string userId, string content);

        Task<CommentListingServiceModel> GetAsync(int id);

        Task<IEnumerable<CommentListingServiceModel>> AllAsync(int articleId);

        Task<bool> DeleteAsync(int commentId, int articleId);
    }
}
