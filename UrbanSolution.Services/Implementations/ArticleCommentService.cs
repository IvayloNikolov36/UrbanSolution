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

        public async Task<TModel> SubmitAsync<TModel>(int articleId, string userId, string content)
        {
            var article = await this.db.FindAsync<Article>(articleId);

            if (article == null)
            {
                return default;
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

            var result = await this.db.Comments.AsNoTracking()
                .Where(c => c.Id == comment.Id)
                .To<TModel>()
                .FirstOrDefaultAsync();

            return result;
        }

        public async Task<TModel> GetAsync<TModel>(int id)
        {
            var comment = await this.db.Comments
                .Where(c => c.Id == id)
                .To<TModel>()
                .FirstOrDefaultAsync();

            return comment;
        }

        public async Task<IEnumerable<TModel>> AllAsync<TModel>(int articleId)
        {
            var comments = await this.db.Comments.AsNoTracking()
                .Where(c => c.ArticleId == articleId)
                .OrderByDescending(c => c.PostedOn)
                .To<TModel>()
                .ToListAsync();

            return comments;
        }

        public async Task<bool> DeleteAsync(int commentId)
        {
            var commentToDelete = await this.db.FindAsync<Comment>(commentId);

            if (commentToDelete == null)
            {
                return false;
            }

            this.db.Remove(commentToDelete);
            await this.db.SaveChangesAsync();

            return true;
        }
    }
}
