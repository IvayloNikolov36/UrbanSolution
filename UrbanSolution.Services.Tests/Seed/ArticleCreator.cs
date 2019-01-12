namespace UrbanSolution.Services.Tests.Seed
{
    using System;
    using UrbanSolution.Models;

    public class ArticleCreator
    {
        private const int DefaultImageId = 9612;

        public static Article Create(string userId, int? imageId = null)
        {
            return new Article
            {
                AuthorId = userId,
                CloudinaryImageId = imageId ?? DefaultImageId,
                Content = Guid.NewGuid().ToString(),
                PublishDate = new DateTime(2018, 12, 05),
                Title = Guid.NewGuid().ToString(),
            };

        }
    }
}
