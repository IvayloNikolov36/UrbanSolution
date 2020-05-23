namespace UrbanSolution.Services.Implementations
{
    using Data;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using UrbanSolution.Models;

    public class UserIssuesService //: IUserIssuesService
    {
        private readonly UrbanSolutionDbContext db;
        private readonly IPictureService pictureService;

        public UserIssuesService(UrbanSolutionDbContext db, IPictureService pictureService)
        {
            this.db = db;
            this.pictureService = pictureService;
        }

        //public async Task<int> UploadAsync(string userId, string title, string description, 
        //    IFormFile pictureFile, string issueType, string region, string address, string latitude, string longitude)
        //{
        //    var picId = await this.pictureService.UploadImageAsync(userId, pictureFile);

        //    var issue = new UrbanIssue
        //    {
        //        Title = title,
        //        Description = description,
        //        CloudinaryImageId = picId,
        //        Type = Enum.Parse<IssueType>(issueType),
        //        Region = Enum.Parse<RegionType>(region),
        //        PublishedOn = DateTime.UtcNow,
        //        PublisherId = userId,
        //        AddressStreet = address,
        //        Latitude = double.Parse(latitude.Trim(), CultureInfo.InvariantCulture),
        //        Longitude = double.Parse(longitude.Trim(), CultureInfo.InvariantCulture)
        //    };

        //    this.db.Add(issue);
        //    await this.db.SaveChangesAsync();

        //    return issue.Id;
        //}

        //public async Task<int> UploadIssueImageAsync(string userId, IFormFile pictureFile)
        //{
        //    var picId = await this.pictureService.UploadImageAsync(userId, pictureFile);

        //    return picId;
        //}
        
        //public async Task<int> UploadIssueAsync(string userId, string title, string description,
        //    int pictureId, string issueType, string region, string address, string latitude, string longitude)
        //{
        //    var issue = new UrbanIssue
        //    {
        //        Title = title,
        //        Description = description,
        //        CloudinaryImageId = pictureId,
        //        Type = Enum.Parse<IssueType>(issueType),
        //        Region = Enum.Parse<RegionType>(region),
        //        PublishedOn = DateTime.UtcNow,
        //        PublisherId = userId,
        //        AddressStreet = address,
        //        Latitude = double.Parse(latitude.Trim(), CultureInfo.InvariantCulture),
        //        Longitude = double.Parse(longitude.Trim(), CultureInfo.InvariantCulture)
        //    };

        //    this.db.Add(issue);
        //    await this.db.SaveChangesAsync();

        //    return issue.Id;
        //}
    }
}
