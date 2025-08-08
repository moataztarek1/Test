namespace KidZone.API.DTOs
{
    public class ReadCommentDto
    {
        public int CommentID { get; set; }
        public string TextComment { get; set; }
        public DateTime CommentDate { get; set; }

        public string UserID { get; set; }
        public string? UserName { get; set; }
    }
}
