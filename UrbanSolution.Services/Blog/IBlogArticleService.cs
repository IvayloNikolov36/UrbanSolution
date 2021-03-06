﻿namespace UrbanSolution.Services.Blog
{
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IBlogArticleService
    {
        Task<IEnumerable<TModel>> AllAsync<TModel>(int page = 1);

        Task<int> TotalAsync();

        Task<TModel> GetAsync<TModel>(int id);

        Task<int> CreateAsync(string title, string content, IFormFile pictureFile, string authorId);

        Task<bool> UpdateAsync(int id, string authorId, string title, string content);

        Task<bool> DeleteAsync(int id, string authorId);

    }
}
