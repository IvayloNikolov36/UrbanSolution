using Microsoft.AspNetCore.Http;

namespace UrbanSolution.Services.Tests.Blog
{
    using Data;
    using FluentAssertions;
    using Mocks;
    using Mocks.MockEntities;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Blog.Implementations;
    using UrbanSolution.Services.Blog.Models;
    using Utilities;
    using Xunit;

    public class BlogArticleServiceTests
    {
        private int id;
        private const int DefaultImageId = 5599;
        private const string DefaultUserName = "Username{0}";

        private readonly UrbanSolutionDbContext db;

        public BlogArticleServiceTests()
        {
            AutomapperInitializer.Initialize();
            this.db = InMemoryDatabase.Get();
        }

        [Fact]
        public async Task AllAsyncShould_ReturnsCorrectArticles_WithDefaultPageEqualToOne()
        {
            //Arrange
            var service = new BlogArticleService(db, null, null);

            var user = this.CreateBlogAuthor();
            var secondUser = this.CreateBlogAuthor();
            var thirdUser = this.CreateBlogAuthor();
            await this.db.AddRangeAsync(user, secondUser, thirdUser);

            var image = this.CreateImage(null, string.Empty);
            await this.db.AddAsync(image);

            var article = this.CreateArticle(user.Id, null);
            var secondArticle = this.CreateArticle(secondUser.Id, null);
            var thirdArticle = this.CreateArticle(thirdUser.Id, null);
            await this.db.AddRangeAsync(article, secondArticle, thirdArticle);

            await this.db.SaveChangesAsync();

            //Act
            var result = await service.AllAsync();  //Skip(page-1 * BlogArticlesPageSize ).Take(BlogArticlesPageSize)

            //Assert
            result.Should().HaveCount(ServiceConstants.BlogArticlesPageSize); //2
            result.Should().BeOfType<List<BlogArticleListingServiceModel>>();
            result.Should().BeInDescendingOrder(x => x.PublishDate);
            result.Should().NotContain(a => a.Id == thirdArticle.Id);
        }

        [Fact]
        public async Task AllAsyncShould_ReturnsCorrectArticles_WithPageParameter()
        {
            //Arrange
            var service = new BlogArticleService(db, null, null);

            var user = this.CreateBlogAuthor();
            var secondUser = this.CreateBlogAuthor();
            var thirdUser = this.CreateBlogAuthor();
            await this.db.AddRangeAsync(user, secondUser, thirdUser);

            var image = this.CreateImage(null, string.Empty);
            await this.db.AddAsync(image);

            var article = this.CreateArticle(user.Id, null);
            var secondArticle = this.CreateArticle(secondUser.Id, null);
            var thirdArticle = this.CreateArticle(thirdUser.Id, null);
            var fourthArticle = this.CreateArticle(thirdUser.Id, null);
            await this.db.AddRangeAsync(article, secondArticle, thirdArticle, fourthArticle);

            await this.db.SaveChangesAsync();

            //Act
            var result = await service.AllAsync(page: 2);

            //Assert
            result.Should().HaveCount(ServiceConstants.BlogArticlesPageSize); //2
            result.Should().BeOfType<List<BlogArticleListingServiceModel>>();
            result.Should().BeInDescendingOrder(x => x.PublishDate);

            result.Should().NotContain(a => a.Id == article.Id);
            result.Should().NotContain(a => a.Id == secondArticle.Id);
            result.Should().Contain(a => a.Id == thirdArticle.Id);
            result.Should().Contain(a => a.Id == fourthArticle.Id);
        }

        [Fact]
        public async Task GetAsyncShould_ReturnsCorrectArticleModel()
        {
            //Arrange
            var service = new BlogArticleService(db, null, null);

            var user = this.CreateBlogAuthor();
            await this.db.AddAsync(user);

            var image = this.CreateImage(null, string.Empty);
            await this.db.AddAsync(image);

            var article = this.CreateArticle(user.Id, null);
            var secondArticle = this.CreateArticle(user.Id, null);
            var thirdArticle = this.CreateArticle(user.Id, null);
            await this.db.AddRangeAsync(article, secondArticle, thirdArticle);

            await this.db.SaveChangesAsync();

            //Act
            var result = await service.GetAsync<BlogArticleDetailsServiceModel>(secondArticle.Id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BlogArticleDetailsServiceModel>();
            result.Id.Should().Be(secondArticle.Id);
        }

        [Fact]
        public async Task CreateAsyncShould_SetTheCorrectParametersForArtcilePropertiesAnd_ReturnsArticleId()
        {
            this.id = 0;
            const int NewImageId = 258;
            const int FirstArticleId = 1;
            const string Title = "Title";
            const string Content = "Content123";
            const string AuthorId = "8945opi7563k87";

            //Arrange
            var pictureService = IPictureServiceMock.New(NewImageId);

            var htmlServiceMock = IHtmlServiceMock.New(Content);

            var service = new BlogArticleService(db, htmlServiceMock.Object, pictureService.Object);

            //Act
            var resultId = await service.CreateAsync(Title, Content, null, AuthorId);
            var savedEntry = await db.FindAsync<Article>(resultId);

            //Assert
            resultId.Should().Be(FirstArticleId);

            htmlServiceMock.Verify(h => h.Sanitize(It.IsAny<string>()), Times.Once);

            pictureService.Verify(p => 
                p.UploadImageAsync(It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Once);

            savedEntry.Title.Should().Match(Title);
            savedEntry.Content.Should().Match(Content);
            savedEntry.AuthorId.Should().Match(AuthorId);
            savedEntry.CloudinaryImageId.Should().Be(NewImageId);
            savedEntry.PublishDate.Should().NotBeCloseTo(DateTime.UtcNow);
        }

        [Fact]
        public async Task UpdateAsyncShould_UpdateTheCorrectPropertiesAnd_ShouldReturnsTrueIf_EventCreatorIsTheSame()
        {
            const string TitleForUpdate = "Title";
            const string ContentForUpdate = "<p>Content</p>";

            //Arrange
            var htmlServiceMock = IHtmlServiceMock.New(ContentForUpdate);

            var service = new BlogArticleService(db, htmlServiceMock.Object, null);

            var author = this.CreateBlogAuthor();
            await this.db.AddAsync(author);

            var article = this.CreateArticle(author.Id, null);
            await this.db.AddAsync(article);

            await this.db.SaveChangesAsync();

            //Act
            var result = await service.UpdateAsync(article.Id, author.Id, TitleForUpdate, ContentForUpdate);
            var updatedEntry = await db.FindAsync<Article>(article.Id);

            //Assert
            result.Should().BeTrue();

            htmlServiceMock.Verify(h => h.Sanitize(It.IsAny<string>()), Times.Once);

            updatedEntry.Title = TitleForUpdate;
            updatedEntry.Content = ContentForUpdate;
        }

        [Fact]
        public async Task UpdateAsyncShould_ReturnsFalseIf_EventCreatorIsAnotherUser()
        {
            const string AuthorIdWhoWantsToUpdate = "789ioptee89897714w78ex5";
            const string TitleForUpdate = "Title";
            const string ContentForUpdate = "<p>Content</p>";

            //Arrange
            var htmlServiceMock = IHtmlServiceMock.New(ContentForUpdate);

            var service = new BlogArticleService(db, htmlServiceMock.Object, null);

            var author = this.CreateBlogAuthor();
            await this.db.AddAsync(author);

            var article = this.CreateArticle(author.Id, null);
            await this.db.AddAsync(article);

            await this.db.SaveChangesAsync();

            //Act
            var result = await service.UpdateAsync(article.Id, AuthorIdWhoWantsToUpdate, TitleForUpdate, ContentForUpdate);

            //Assert
            result.Should().BeFalse();

            htmlServiceMock.Verify(h => h.Sanitize(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsyncShould_ReturnsTrueIf_EventCreatorIsAnotherUser_AndShouldDeleteArticle_AndImage()
        {
            int imageToDeleteId = 58963;

            //Arrange
            var picServiceMock = IPictureServiceMock.New(imageToDeleteId);

            var service = new BlogArticleService(db, null, picServiceMock.Object);

            var author = this.CreateBlogAuthor();
            await this.db.AddAsync(author);

            var image = this.CreateImage(imageToDeleteId, author.Id);
            await this.db.AddAsync(image);

            var article = this.CreateArticle(author.Id, imageToDeleteId);
            await this.db.AddAsync(article);

            await this.db.SaveChangesAsync();

            //Act
            var result = await service.DeleteAsync(article.Id, author.Id);

            //Assert
            result.Should().BeTrue();

            picServiceMock.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Once);

            this.db.Articles.Should().NotContain(a => a.Id == article.Id);
        }

        [Fact]
        public async Task DeleteAsyncShould_ReturnsFalseIf_EventCreatorIsAnotherUser_And_ShouldNotDeleteArticle_AndImage()
        {
            int imageToDeleteId = 58963;
            const string anotherAuthorId = "899f4fgg5f57dmmmmmmm";

            //Arrange
            var picServiceMock = IPictureServiceMock.New(imageToDeleteId);           

            var service = new BlogArticleService(db, null, picServiceMock.Object);
                
            var author = this.CreateBlogAuthor();
            await this.db.AddAsync(author);

            var article = this.CreateArticle(author.Id, null);
            await this.db.AddAsync(article);

            await this.db.SaveChangesAsync();

            //Act
            var result = await service.DeleteAsync(article.Id, anotherAuthorId);

            //Assert
            result.Should().BeFalse();

            picServiceMock.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Never);

            this.db.Articles.Should().Contain(a => a.Id == article.Id);
        }

        private User CreateBlogAuthor()
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = string.Format(DefaultUserName, ++this.id)
            };

            return user;
        }

        private CloudinaryImage CreateImage(int? imageId, string uploaderId)
        {
            var image = new CloudinaryImage
            {
                Id = imageId ?? DefaultImageId,
                PictureUrl = Guid.NewGuid().ToString(),
                Length = long.MaxValue,
                PicturePublicId = Guid.NewGuid().ToString(),
                PictureThumbnailUrl = Guid.NewGuid().ToString(),
                UploadedOn = DateTime.UtcNow,
                UploaderId = uploaderId 
            };

            return image;
        }

        private Article CreateArticle(string userId, int? cloudinaryImageId)
        {
            var article = new Article
            {
                AuthorId = userId,
                CloudinaryImageId = cloudinaryImageId ?? DefaultImageId,
                Content = Guid.NewGuid().ToString(),
                PublishDate = new DateTime(2018, 12, 05),
                Title = Guid.NewGuid().ToString(),
            };

            return article;
        }
    }
}
