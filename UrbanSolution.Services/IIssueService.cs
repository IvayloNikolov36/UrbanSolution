namespace UrbanSolution.Services
{
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UrbanSolution.Models;

    public interface IIssueService
    {
        Task<(int, IEnumerable<TModel>)> AllAsync<TModel>(bool isApproved, int rowsCount, int page, string regionFilter, string typeFilter, string sortType);

        Task<int> TotalAsync(bool isApproved);

        Task<TModel> GetAsync<TModel>(int id);

        Task<int> UploadAsync(string userId, string title, string description, IFormFile pictureFile,
            string issueType, string region, string address, string latitude, string longitude);

        Task<int> UploadIssueImageAsync(string userId, IFormFile pictureFile);

        Task<int> UploadIssueAsync(string userId, string title, string description,
            int pictureId, string issueType, string region, string address, string latitude, string longitude);

        Task<IEnumerable<TModel>> AllMapInfoDetailsAsync<TModel>(bool areApproved, RegionType? region);
    }
}
