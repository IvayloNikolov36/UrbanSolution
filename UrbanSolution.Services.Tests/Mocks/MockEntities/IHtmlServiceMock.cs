namespace UrbanSolution.Services.Tests.Mocks.MockEntities
{
    using Html;
    using Moq;

    public class IHtmlServiceMock
    {
        public static Mock<IHtmlService> New()
        {
            var htmlServiceMock = new Mock<IHtmlService>();

            htmlServiceMock.Setup(h => h.Sanitize(It.IsAny<string>()))
                .Returns(It.IsAny<string>());

            return htmlServiceMock;
        }
    }
}
