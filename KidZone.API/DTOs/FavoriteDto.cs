using KidZone.Domain.Enums;

namespace KidZone.API.DTOs
{
    public class FavoriteDto
    {
        public int FavoriteID { get; set; }
        public int ContentID { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public ContentAccessType ContentType { get; set; }
    }
}
