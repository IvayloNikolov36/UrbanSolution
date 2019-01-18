
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UrbanSolution.Data;
using UrbanSolution.Models;
using UrbanSolution.Services.Implementations;
using UrbanSolution.Services.Mapping;
using UrbanSolution.Services.Models;
using UrbanSolution.Services.Tests.Seed;
using Xunit;

namespace UrbanSolution.Services.Tests.NoArea
{
    public class ArticleCommentServiceTests
    {
        private const int NotExistingArticleId = 2999999;
        private readonly UrbanSolutionDbContext db;

        public ArticleCommentServiceTests()
        {
            this.db = InMemoryDatabase.Get();
            AutomapperInitializer.Initialize();
        }

        [Fact]
        public async Task SubmitAsyncShould_SetsTheCorrectPropsAnd_SavesEntityInDB()
        {
            const string commentContent = "Content";

            //Arrange
            var user = UserCreator.Create();
            this.db.Add(user);

            var article = ArticleCreator.Create(user.Id);
            this.db.Add(article);

            await this.db.SaveChangesAsync();

            var service = new ArticleCommentService(this.db);

            //Act
            var result = await service.SubmitAsync(article.Id, user.Id, commentContent);

            var commentModel = await this.db.Comments
                .Where(c => c.Id == result.Id)
                .To<CommentListingServiceModel>()
                .FirstOrDefaultAsync();

            result.Should().BeOfType<CommentListingServiceModel>();

            result.ArticleAuthor.Should().BeEquivalentTo(user.UserName);

            result.ArticleId.Should().Be(article.Id);

            result.Should().BeEquivalentTo(commentModel);
        }

        [Fact]
        public async Task SubmitAsyncShould_ReturnsNullIf_NoArticleFound()
        {
            const string commentContent = "Content";


            //Arrange
            var user = UserCreator.Create();
            this.db.Add(user);

            await this.db.SaveChangesAsync();

            var service = new ArticleCommentService(this.db);

            //Act
            var result = await service.SubmitAsync(NotExistingArticleId, user.Id, commentContent);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAsyncShould_ReturnsCorrectModel()
        {
            //Arrange
            var user = UserCreator.Create();
            this.db.Add(user);

            var article = ArticleCreator.Create(user.Id);
            this.db.Add(article);

            var comment = this.CreateArticleComment(article.Id, user.Id);
            this.db.Add(comment);

            await this.db.SaveChangesAsync();

            var service = new ArticleCommentService(this.db);

            //Act
            var result = await service.GetAsync(article.Id);

            var expected = await this.db.Comments
                .Where(c => c.Id == article.Id)
                .To<CommentListingServiceModel>()
                .FirstOrDefaultAsync();

            //Assert
            result.Should().BeOfType<CommentListingServiceModel>();

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task AllAsyncShould_ReturnsCorrectComments()
        {
            //Arrange
            var user = UserCreator.Create();
            this.db.Add(user);

            var article = ArticleCreator.Create(user.Id);
            this.db.Add(article);

            var comment = this.CreateArticleComment(article.Id, user.Id);
            var secondComment = this.CreateArticleComment(NotExistingArticleId, user.Id);
            this.db.AddRange(comment, secondComment);

            await this.db.SaveChangesAsync();

            var service = new ArticleCommentService(this.db);

            //Act
            var result = await service.AllAsync(article.Id);

            var expectedComments = await this.db.Comments
                .Where(c => c.ArticleId == article.Id)
                .OrderByDescending(c => c.PostedOn)
                .To<CommentListingServiceModel>()
                .ToListAsync();

            var expectedCount = await this.db.Comments.Where(c => c.ArticleId == article.Id).CountAsync();

            //Assert
            result.Should().BeOfType<List<CommentListingServiceModel>>();

            result.Should().HaveCount(expectedCount);

            result.Should().BeEquivalentTo(expectedComments);
        }

        [Fact]
        public async Task DeleteAsyncShould_ReturnsFalseIf_CommentIdDoesNotExists()
        {
            const int notExistingCommentId = 556698;

            var service = new ArticleCommentService(this.db);

            var result = await service.DeleteAsync(notExistingCommentId);

            result.Should().Be(false);
        }

        [Fact]
        public async Task DeleteAsyncShould_DeletesCorrectCommentIf_CommentIdExists()
        {
            //Arrange
            var user = UserCreator.Create();
            this.db.Add(user);

            var article = ArticleCreator.Create(user.Id);
            this.db.Add(article);

            var comment = this.CreateArticleComment(article.Id, user.Id);
            var secondComment = this.CreateArticleComment(article.Id, user.Id);
            this.db.AddRange(comment, secondComment);

            await this.db.SaveChangesAsync();

            var service = new ArticleCommentService(this.db);

            var commentsCount = this.db.Comments.Count();

            //Act
            var result = await service.DeleteAsync(comment.Id);

            //Assert
            result.Should().BeTrue();

            this.db.Comments.Count().Should().Be(commentsCount - 1);

            this.db.Find<Comment>(comment.Id).Should().BeNull();
        }

        private Comment CreateArticleComment(int articleId, string publisherId)
        {
            return new Comment
            {
                ArticleId = articleId,
                Content = Guid.NewGuid().ToString(),
                PostedOn = DateTime.UtcNow,
                PublisherId = publisherId
            };
        }
    }
}
