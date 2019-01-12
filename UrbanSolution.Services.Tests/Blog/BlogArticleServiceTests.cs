namespace UrbanSolution.Services.Tests.Blog
{
    using Data;
    using FluentAssertions;
    using Mapping;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Mocks;
    using Mocks.MockEntities;
    using Moq;
    using Seed;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Blog.Implementations;
    using UrbanSolution.Services.Blog.Models;
    using Utilities;
    using Xunit;
    using System.Linq;

    public class BlogArticleServiceTests : BaseServiceTest
    {
        private const int imageToDeleteId = 58963;
        private const string TitleForUpdate = "Title";
        private const string ContentForUpdate = "Content";

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task AllAsyncShould_ReturnsCorrectArticles_WithDefaultPageEqualToOne(int page)
        {
            //Arrange
            var service = new BlogArticleService(Db, null, null);

            var user = UserCreator.Create();
            var secondUser = UserCreator.Create();
            var thirdUser = UserCreator.Create();
            await this.Db.AddRangeAsync(user, secondUser, thirdUser);

            var image = ImageInfoCreator.Create();
            await this.Db.AddAsync(image);

            var article = ArticleCreator.Create(user.Id, null);
            var secondArticle = ArticleCreator.Create(secondUser.Id, null);
            var thirdArticle = ArticleCreator.Create(thirdUser.Id, null);
            var fourthArticle = ArticleCreator.Create(thirdUser.Id, null);
            await this.Db.AddRangeAsync(article, secondArticle, thirdArticle, fourthArticle);

            await this.Db.SaveChangesAsync();

            //Act
            var result = await service.AllAsync(page: page);

            var expectedResult = await this.Db
                .Articles
                .OrderByDescending(a => a.PublishDate)
                .Skip((page - 1) * ServiceConstants.BlogArticlesPageSize)
                .Take(ServiceConstants.BlogArticlesPageSize)
                .To<BlogArticleListingServiceModel>()
                .ToListAsync();

            //Assert
            result.Should().BeOfType<List<BlogArticleListingServiceModel>>();

            result.Should().HaveCount(expectedResult.Count);

            result.Should().BeInDescendingOrder(x => x.PublishDate);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetAsyncShould_ReturnsCorrectArticleModel()
        {
            //Arrange
            var user = UserCreator.Create();
            await this.Db.AddAsync(user);

            var image = ImageInfoCreator.CreateWithFullData(user.Id);
            await this.Db.AddAsync(image);

            var article = ArticleCreator.Create(user.Id, image.Id);
            var secondArticle = ArticleCreator.Create(user.Id, image.Id);
            var thirdArticle = ArticleCreator.Create(user.Id, image.Id);
            await this.Db.AddRangeAsync(article, secondArticle, thirdArticle);

            await this.Db.SaveChangesAsync();

            var service = new BlogArticleService(Db, null, null);

            //Act
            var result = await service.GetAsync<BlogArticleDetailsServiceModel>(secondArticle.Id);

            var expected = await this.Db
                .Articles
                .Include(a => a.Comments)
                .Where(a => a.Id == secondArticle.Id)
                .To<BlogArticleDetailsServiceModel>()
                .FirstOrDefaultAsync();

            var secondResult = await service.GetAsync<EditArticleServiceViewModel>(article.Id);

            var secondExpected = await this.Db.Articles.Include(a => a.Comments).Where(a => a.Id == article.Id)
                .To<EditArticleServiceViewModel>().FirstOrDefaultAsync();

            //Assert
            result.Should().BeOfType<BlogArticleDetailsServiceModel>();
            result.Should().BeEquivalentTo(expected);

            secondResult.Should().BeOfType<EditArticleServiceViewModel>();
            secondResult.Should().BeEquivalentTo(secondExpected);
        }

        [Fact]
        public async Task CreateAsyncShould_SetTheCorrectParametersForArticlePropertiesAnd_ReturnsArticleId()
        {
            const int NewImageId = 258;
            const string Title = "Title";
            const string Content = "Content123";
            const string AuthorId = "8945opi7563k87";

            //Arrange
            var picService = IPictureServiceMock.New(NewImageId);

            var htmlService = IHtmlServiceMock.New(Content);

            var service = new BlogArticleService(Db, htmlService.Object, picService.Object);

            //Act
            var resultId = await service.CreateAsync(Title, Content, null, AuthorId);

            var savedEntry = await Db.FindAsync<Article>(resultId);

            //Assert
            resultId.Should().Be(savedEntry.Id);

            htmlService.Verify(h => h.Sanitize(It.IsAny<string>()), Times.Once);

            picService.Verify(p => 
                p.UploadImageAsync(It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Once);

            savedEntry.Id.Should().Be(resultId);
            savedEntry.Title.Should().Match(Title);
            savedEntry.Content.Should().Match(Content);
            savedEntry.AuthorId.Should().Match(AuthorId);
            savedEntry.CloudinaryImageId.Should().Be(NewImageId);
        }

        [Fact]
        public async Task UpdateAsyncShould_UpdateTheCorrectPropertiesAnd_ShouldReturnsTrueIf_EventCreatorIsTheSame()
        {
            //Arrange
            var htmlService = IHtmlServiceMock.New(ContentForUpdate);

            var service = new BlogArticleService(Db, htmlService.Object, null);

            var author = UserCreator.Create();
            await this.Db.AddAsync(author);

            var article = ArticleCreator.Create(author.Id, null);
            await this.Db.AddAsync(article);

            await this.Db.SaveChangesAsync();

            //Act
            var result = await service.UpdateAsync(article.Id, author.Id, TitleForUpdate, ContentForUpdate);

            var updatedEntry = await Db.FindAsync<Article>(article.Id);

            //Assert
            result.Should().BeTrue();

            htmlService.Verify(h => h.Sanitize(It.IsAny<string>()), Times.Once);

            updatedEntry.Title = TitleForUpdate;
            updatedEntry.Content = ContentForUpdate;
        }

        [Fact]
        public async Task UpdateAsyncShould_ReturnsFalseIf_EventCreatorIsAnotherUser()
        {
            const string UpdaterId = "789io87714w78ex5";
            
            //Arrange
            var htmlServiceMock = IHtmlServiceMock.New(ContentForUpdate);

            var service = new BlogArticleService(Db, htmlServiceMock.Object, null);

            var author = UserCreator.Create();
            await this.Db.AddAsync(author);

            var article = ArticleCreator.Create(author.Id, null);
            await this.Db.AddAsync(article);

            await this.Db.SaveChangesAsync();

            //Act
            var result = await service.UpdateAsync(article.Id, UpdaterId, TitleForUpdate, ContentForUpdate);

            //Assert
            result.Should().BeFalse();

            htmlServiceMock.Verify(h => h.Sanitize(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsyncShould_ReturnsTrueIf_EventCreatorIsAnotherUser_AndShouldDeleteArticle_AndImage()
        {
            //Arrange
            var picServiceMock = IPictureServiceMock.New(imageToDeleteId);

            var service = new BlogArticleService(Db, null, picServiceMock.Object);

            var author = UserCreator.Create();
            await this.Db.AddAsync(author);

            var image = ImageInfoCreator.CreateWithFullData(author.Id);
            await this.Db.AddAsync(image);

            var article = ArticleCreator.Create(author.Id, imageToDeleteId);
            await this.Db.AddAsync(article);

            await this.Db.SaveChangesAsync();

            //Act
            var result = await service.DeleteAsync(article.Id, author.Id);

            //Assert
            result.Should().BeTrue();

            picServiceMock.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Once);

            this.Db.Articles.Should().NotContain(a => a.Id == article.Id);
        }

        [Fact]
        public async Task DeleteAsyncShould_ReturnsFalseIf_EventCreatorIsAnotherUser_And_ShouldNotDeleteArticle_AndImage()
        {
            const string anotherAuthorId = "899f4fgg5f57dm888m";

            //Arrange
            var picServiceMock = IPictureServiceMock.New(imageToDeleteId);           

            var service = new BlogArticleService(Db, null, picServiceMock.Object);
                
            var author = UserCreator.Create();
            await this.Db.AddAsync(author);

            var article = ArticleCreator.Create(author.Id, null);
            await this.Db.AddAsync(article);

            await this.Db.SaveChangesAsync();

            //Act
            var result = await service.DeleteAsync(article.Id, anotherAuthorId);

            //Assert
            result.Should().BeFalse();

            picServiceMock.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Never);

            this.Db.Articles.Should().Contain(a => a.Id == article.Id);
        }
        
    }
}
