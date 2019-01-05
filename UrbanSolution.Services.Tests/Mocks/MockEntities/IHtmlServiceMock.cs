namespace UrbanSolution.Services.Tests.Mocks.MockEntities
{
    using Html;
    using Moq;

    public class IHtmlServiceMock
    {
        public static Mock<IHtmlService> New(string content)
        {
            var htmlServiceMock = new Mock<IHtmlService>();

            htmlServiceMock.Setup(h => h.Sanitize(content))
                .Returns(content);

            return htmlServiceMock;
        }
    }
}
