namespace UrbanSolution.Services
{
    using System;
    using System.Threading.Tasks;

    public interface IPictureInfoWriterService
    {
        Task<int> WriteToDbAsync(string uploaderId, string pictureUrl, string pictureThumbnailUrl,
            string picPublicId, DateTime uploadedOn, long picLength);
    }
}
