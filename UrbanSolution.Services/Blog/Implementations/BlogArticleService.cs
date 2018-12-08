namespace UrbanSolution.Services.Blog.Implementations
{
    using Data;
    using Mapping;
    using Models;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using Utilities;

    public class BlogArticleService : IBlogArticleService
    {
        private readonly UrbanSolutionDbContext db;

        public BlogArticleService(UrbanSolutionDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<BlogArticleListingServiceModel>> AllAsync(int page = 1)
            => await this.db
                .Articles
                .OrderByDescending(a => a.PublishDate)
                .Skip((page - 1) * ServiceConstants.BlogArticlesPageSize)
                .Take(ServiceConstants.BlogArticlesPageSize)
                .To<BlogArticleListingServiceModel>()
                .ToListAsync();

        public async Task<int> TotalAsync()
            => await this.db.Articles.CountAsync();

        public async Task<BlogArticleDetailsServiceModel> ById(int id)
            => await this.db
                .Articles
                .Where(a => a.Id == id)
                .To<BlogArticleDetailsServiceModel>()
                .FirstOrDefaultAsync();

        public async Task CreateAsync(string title, string content, string authorId)
        {
            var article = new Article
            {
                Title = title,
                Content = content,
                PublishDate = DateTime.UtcNow,
                AuthorId = authorId
            };

            this.db.Add(article);

            await this.db.SaveChangesAsync();
        }
    }
}
