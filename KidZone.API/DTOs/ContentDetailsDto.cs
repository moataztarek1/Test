using KidZone.Domain.Enums;

namespace KidZone.API.DTOs
{
    public class ContentDetailsDto
    {
        public int ContentID { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public ContentAccessType ContentType { get; set; }
        public DateTime? CreatedAt { get; set; }

        // For Video
        public string? Url { get; set; }
        public TimeSpan? Duration { get; set; }

        // For Story
        public string? Text_Content { get; set; }
        public string? ImgUrl { get; set; }

        public List<string> Categories { get; set; } = new List<string>();
    }

}
