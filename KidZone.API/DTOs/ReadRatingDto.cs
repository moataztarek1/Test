namespace KidZone.API.DTOs
{
    public class ReadRatingDto
    {
        public int RatingID { get; set; }
        public string UserName { get; set; }
        public string Type { get; set; } // Like or Dislike
    }
}
