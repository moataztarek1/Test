namespace KidZone.API.DTOs
{
    public class UpdateVideoDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }

        // Video-specific
        public string? Url { get; set; }
        public TimeSpan? Duration { get; set; }

        // Categories
        public List<int>? CategoryIds { get; set; }
    }

}
