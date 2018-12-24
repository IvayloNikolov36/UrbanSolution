using System;
using System.Threading.Tasks;

namespace UrbanSolution.Services
{
    public interface IPictureService
    {
        Task<int> WritePictureInfo(string uploaderId, string pictureUrl, string pictureThumbnailUrl,
            string picturePublicId, DateTime uploadedOn, long pictureLength);
    }
}
