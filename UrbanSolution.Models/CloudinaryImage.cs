namespace UrbanSolution.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CloudinaryImage
    {
        [Key]
        public int Id { get; set; }

        public string PicturePublicId { get; set; }

        public string PictureUrl { get; set; }

        public string PictureThumbnailUrl { get; set; }

        public long Length { get; set; }

        public DateTime UploadedOn { get; set; }

        public string UploaderId { get; set; }

        public User Uploader { get; set; }

    }
}
