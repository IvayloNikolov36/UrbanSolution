namespace UrbanSolution.Services.Tests.Mocks
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Moq;

    public class IPictureServiceMock
    {
        public static Mock<IPictureService> New(int imageId)
        {
            var pictureService = new Mock<IPictureService>();

            pictureService.Setup(p => p.UploadImageAsync(It.IsAny<string>(), It.IsAny<IFormFile>()))
                .Returns(Task.FromResult(imageId));

            pictureService.Setup(p => p.DeleteImageAsync(imageId)).Returns(Task.CompletedTask);

            return pictureService;
        }

    }
}
