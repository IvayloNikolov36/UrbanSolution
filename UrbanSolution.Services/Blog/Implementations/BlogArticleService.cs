using UrbanSolution.Services.Models;

namespace UrbanSolution.Services.Blog.Implementations
{
    using Microsoft.AspNetCore.Http;
    using Data;
    using Html;
    using Mapping;
    using Models;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using static Utilities.ServiceConstants;

    public class BlogArticleService : IBlogArticleService
    {
        private readonly UrbanSolutionDbContext db;
        private readonly IHtmlService htmlService;
        private readonly IPictureService pictureService;

        public BlogArticleService(
            UrbanSolutionDbContext db, 
            IHtmlService htmlService,
            IPictureService pictureService)
        {
            this.db = db;
            this.htmlService = htmlService;
            this.pictureService = pictureService;
        }

        public async Task<IEnumerable<BlogArticleListingServiceModel>> AllAsync(int page = 1)
        {
            var articles = await this.db
                .Articles
                .OrderByDescending(a => a.PublishDate)
                .Skip((page - 1) * BlogArticlesPageSize)
                .Take(BlogArticlesPageSize)
                .To<BlogArticleListingServiceModel>()
                .ToListAsync();

            return articles;
        }

        public async Task<int> TotalAsync()
        {
            var count = await this.db.Articles.CountAsync();

            return count;
        }

        public async Task<TModel> GetAsync<TModel>(int id)
        {
            var article = await this.db
                .Articles
                .Include(a => a.Comments)
                .Where(a => a.Id == id)
                .To<TModel>()
                .FirstOrDefaultAsync();

            return article;
        }

        public async Task<int> CreateAsync(string title, string content, IFormFile pictureFile, string authorId)
        {
            var sanitizedContent = this.htmlService.Sanitize(content);

            var imageId = await this.pictureService.UploadImageAsync(authorId, pictureFile);

            var article = new Article
            {
                Title = title,
                Content = sanitizedContent,
                PublishDate = DateTime.UtcNow,
                AuthorId = authorId,
                CloudinaryImageId = imageId
            };

            await this.db.Articles.AddAsync(article);

            await this.db.SaveChangesAsync();

            return article.Id;
        }

        public async Task<bool> UpdateAsync(int id, string authorId, string title, string content)
        {
            var article = await this.db.FindAsync<Article>(id);

            var articleAuthorId = article.AuthorId;

            if (articleAuthorId != authorId)
            {
                return false;
            }

            article.Title = title;
            article.Content = this.htmlService.Sanitize(content);

            await this.db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id, string authorId)
        {
            var article = await this.db.FindAsync<Article>(id);

            var articleAuthorId = article.AuthorId;

            if (articleAuthorId != authorId)
            {
                return false;
            }

            var articlePicId = article.CloudinaryImageId;

            //First delete article, than the image
            this.db.Articles.Remove(article);

            await this.db.SaveChangesAsync();

            await this.pictureService.DeleteImageAsync(articlePicId);

            return true;
        }

      
    }
}
