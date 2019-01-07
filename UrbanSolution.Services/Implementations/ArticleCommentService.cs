namespace UrbanSolution.Services.Implementations
{
    using Data;
    using Mapping;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models;

    public class ArticleCommentService : IArticleCommentService
    {
        private readonly UrbanSolutionDbContext db;

        public ArticleCommentService(UrbanSolutionDbContext db)
        {
            this.db = db;
        }

        public async Task<CommentListingServiceModel> SubmitAsync(int articleId, string userId, string content)
        {
            var article = await this.db.FindAsync<Article>(articleId);

            if (article == null)
            {
                return null;
            }

            var comment = new Comment
            {
                Content = content,
                PostedOn = DateTime.UtcNow,
                PublisherId = userId,
                ArticleId = articleId
            };

            await this.db.Comments.AddAsync(comment);

            await this.db.SaveChangesAsync();

            var result = await this.db.Comments
                .Where(c => c.Id == comment.Id)
                .To<CommentListingServiceModel>()
                .FirstOrDefaultAsync();

            return result;
        }

        public async Task<CommentListingServiceModel> GetAsync(int id)
        {
            var comment = await this.db.Comments
                .Where(c => c.Id == id)
                .To<CommentListingServiceModel>()
                .FirstOrDefaultAsync();

            return comment;
        }

        public async Task<IEnumerable<CommentListingServiceModel>> AllAsync(int articleId)
        {
            var comments = await this.db.Comments
                .Where(c => c.ArticleId == articleId)
                .OrderByDescending(c => c.PostedOn)
                .To<CommentListingServiceModel>()
                .ToListAsync();

            return comments;
        }
    }
}
