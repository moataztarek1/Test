namespace KidZone.API.DTOs
{
    public class UpdateStoryDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }

        // Story-specific
        public string? Text_Content { get; set; }
        public string? ImgUrl { get; set; }

        // Categories
        public List<int>? CategoryIds { get; set; }
    }

}
