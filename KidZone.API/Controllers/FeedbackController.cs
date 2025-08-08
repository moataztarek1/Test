using KidZone.API.Data;
using KidZone.API.DTOs;
using KidZone.Domain.Entities;
using KidZone.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KidZone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FeedbackController : ControllerBase
    {
        private readonly DataContext context;

        public FeedbackController(DataContext _context)
        {
            this.context = _context;
        }

        private int GetUserIdFromToken()
        {
            var uidClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(uidClaim))
                throw new UnauthorizedAccessException("User ID not found in token.");

            return int.Parse(uidClaim);
        }

        [HttpGet("GetCommentsByContentId/{contentId}")]
        public async Task<ActionResult<List<ReadCommentDto>>> GetCommentsByContentId(int contentId)
        {
            var commentsOnContent = await context.Comments
                .Where(c => c.ContentID == contentId)
                .Select(c => new ReadCommentDto
                {
                    CommentID = c.CommentID,
                    TextComment = c.TextComment,
                    CommentDate = c.CommentDate,
                    UserID = c.UserID,
                    UserName = c.User.UserName
                })
                .ToListAsync();

            if (!commentsOnContent.Any())
                return NotFound("No comments found for this content.");

            return Ok(commentsOnContent);
        }

        [HttpGet("GetCommentsByUserId/{contentId}")]
        public async Task<ActionResult<List<ReadCommentDto>>> GetCommentsByUserId(int contentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userComments = await context.Comments
                .Where(c => c.UserID == userId && c.ContentID == contentId)
                .Select(c => new ReadCommentDto
                {
                    CommentID = c.CommentID,
                    TextComment = c.TextComment,
                    CommentDate = c.CommentDate,
                    UserID = c.UserID,
                    UserName = c.User.UserName
                })
                .ToListAsync();

            if (!userComments.Any())
                return NotFound("You do not have comments yet");

            return Ok(userComments);
        }

        [HttpPost("AddComment")]
        public async Task<ActionResult> AddComment([FromBody] CreateCommentDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var newComment = new Comment
            {
                TextComment = dto.TextComment,
                CommentDate = DateTime.UtcNow,
                ContentID = dto.ContentID,
                UserID = userId
            };

            context.Comments.Add(newComment);
            await context.SaveChangesAsync();

            return Ok("The comment has been created");
        }

        [HttpPut("UpdateComment")]
        public async Task<IActionResult> UpdateComment([FromBody] UpdateCommentDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingComment = await context.Comments.FindAsync(dto.CommentID);

            if (existingComment == null)
                return NotFound("Comment not found.");

            if (existingComment.UserID != userId)
                return Unauthorized("You are not allowed to update this comment.");

            existingComment.TextComment = dto.TextComment;
            existingComment.CommentDate = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return Ok("Comment updated");
        }

        [HttpDelete("DeleteComment")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var comment = await context.Comments.FindAsync(commentId);

            if (comment == null)
                return NotFound("Comment not found.");

            if (comment.UserID != userId)
                return Unauthorized("You are not allowed to delete this comment.");

            context.Comments.Remove(comment);
            await context.SaveChangesAsync();

            return Ok("Comment has been deleted");
        }

        [HttpGet("GetRatingByContentId/{contentId}")]
        public async Task<ActionResult<List<ReadRatingDto>>> GetRatingByContentId(int contentId)
        {
            var ratings = await context.Ratings
                .Where(r => r.ContentID == contentId)
                .Select(r => new ReadRatingDto
                {
                    RatingID = r.RatingID,
                    Type = r.Type.ToString(),
                    UserName = r.User.UserName
                })
                .ToListAsync();

            if (!ratings.Any())
                return NotFound("No ratings found for this content.");

            return Ok(ratings);
        }

        [HttpGet("GetUserRatingForContent/{contentId}")]
        public async Task<ActionResult<UserRatingDto>> GetUserRatingForContent(int contentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var rating = await context.Ratings
                .Where(r => r.UserID == userId && r.ContentID == contentId)
                .Select(r => new UserRatingDto
                {
                    ContentID = r.ContentID,
                    UserID = r.UserID,
                    Rating = r.Type.ToString()
                })
                .FirstOrDefaultAsync();

            if (rating == null)
                return NotFound("No rating for this user on this content");

            return Ok(rating);
        }

        [HttpPost("AddOrUpdateRating")]
        public async Task<IActionResult> AddOrUpdateRating([FromBody] AddOrUpdateRatingDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Enum.TryParse<RatingType>(dto.Rating, true, out var ConvertedToRatingType))
                return BadRequest("Invalid rating type. Use 'Like' or 'Dislike'.");

            var existingRating = await context.Ratings
                .FirstOrDefaultAsync(r => r.UserID == userId && r.ContentID == dto.ContentID);

            if (existingRating == null)
            {
                var newRating = new Rating
                {
                    ContentID = dto.ContentID,
                    UserID = userId,
                    Type = ConvertedToRatingType
                };
                await context.Ratings.AddAsync(newRating);
            }
            else if (existingRating.Type == ConvertedToRatingType)
            {
                context.Ratings.Remove(existingRating);
            }
            else
            {
                existingRating.Type = ConvertedToRatingType;
            }

            await context.SaveChangesAsync();
            return Ok("Updated");
        }

        [HttpDelete("DeleteRating/{contentId}")]
        public async Task<IActionResult> DeleteRating(int contentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var rating = await context.Ratings
                .FirstOrDefaultAsync(r => r.UserID == userId && r.ContentID == contentId);

            if (rating == null)
                return NotFound("Rating not found for this content.");

            context.Ratings.Remove(rating);
            await context.SaveChangesAsync();

            return Ok("Rating deleted successfully.");
        }
    }
}
#region Before
//public class FeedbackController : ControllerBase
//{
//    DataContext context;
//    public FeedbackController(DataContext _context)
//    {
//        this.context = _context;
//    }

//    //public int GetUserIdFromToken()
//    //{
//    //    var uidClaim = User.FindFirst("uid")?.Value;

//    //    if (string.IsNullOrEmpty(uidClaim))
//    //        return -1;

//    //    var userId = int.Parse(uidClaim);

//    //    return userId;
//    //}



//    [HttpGet("GetCommentsByContentId/{contentId}")]
//    public async Task<ActionResult<List<ReadCommentDto>>> GetCommentsByContentId(int contentId)
//    {
//        var commentsOnContent = await context.Comments.Where(c => c.ContentID == contentId)
//        .Select(c => new ReadCommentDto
//        {
//            CommentID = c.CommentID,
//            TextComment = c.TextComment,
//            CommentDate = c.CommentDate,
//            UserID = c.UserID,
//            UserName = c.User.UserName
//        }).ToListAsync();

//        if (!commentsOnContent.Any())
//            return NotFound("No comments found for this content.");

//        return Ok(commentsOnContent);
//    }



//    //[Authorize]
//    [HttpGet("GetCommentsByUserId/{contentId}")]
//    public async Task<ActionResult<List<ReadCommentDto>>> GetCommentsByUserId(int contentId)
//    {

//        //var userIdStr = User.FindFirst("uid")?.Value;
//        //if (!int.TryParse(userIdStr, out int userId))
//        //     return Unauthorized("User ID not found in token.");

//        int userId = 1;

//        var userComments = await context.Comments
//        .Where(c => c.UserID == userId && c.ContentID == contentId)
//        .Select(c => new ReadCommentDto
//        {
//            CommentID = c.CommentID,
//            TextComment = c.TextComment,
//            CommentDate = c.CommentDate,
//            UserID = c.UserID,
//            UserName = c.User.UserName
//        })
//        .ToListAsync();

//        if (!userComments.Any())
//            return NotFound("you do not have comments yet");

//        return Ok(userComments);
//    }



//    //[Authorize]
//    [HttpPost("AddComment")]
//    public async Task<ActionResult> AddComment([FromBody] CreateCommentDto dto)
//    {

//        var userIdStr = User.FindFirst("uid")?.Value;
//        if (!int.TryParse(userIdStr, out int userId))
//            return Unauthorized("User ID not found in token.");

//        //int userId = 1; // 1 is an example you can try any number and it will work

//        var newComment = new Comment
//        {
//            TextComment = dto.TextComment,
//            CommentDate = DateTime.UtcNow,
//            ContentID = dto.ContentID,
//            UserID = userId
//        };

//        context.Comments.Add(newComment);
//        await context.SaveChangesAsync();

//        return Ok("the comment has created");
//    }



//    //[Authorize]
//    [HttpPut("UpdateComment")]
//    public async Task<IActionResult> UpdateComment([FromBody] UpdateCommentDto dto)
//    {
//        //var userIdStr = User.FindFirst("uid")?.Value;
//        //if (!int.TryParse(userIdStr, out int userId))
//        //     return Unauthorized("User ID not found in token.");

//        int userId = 1;

//        var existingComment = await context.Comments.FindAsync(dto.CommentID);

//        if (existingComment == null)
//            return NotFound("Comment not found.");

//        if (existingComment.UserID != userId)
//            return Unauthorized("You are not allowed to update this comment.");

//        existingComment.TextComment = dto.TextComment;
//        existingComment.CommentDate = DateTime.UtcNow;

//        await context.SaveChangesAsync();

//        return Ok("Comment updated");
//    }


//    //[Authorize]
//    [HttpDelete("DeleteComment")]
//    public async Task<IActionResult> DeleteComment(int commentId)
//    {

//        //var userIdStr = User.FindFirst("uid")?.Value;
//        //if (!int.TryParse(userIdStr, out int userId))
//        //     return Unauthorized("User ID not found in token.");


//        int userId = 1;

//        var comment = await context.Comments.FindAsync(commentId);

//        if (comment == null)
//            return NotFound("Comment not found.");

//        if (comment.UserID != userId)
//            return Unauthorized("You are not allowed to delete this comment.");

//        context.Comments.Remove(comment);
//        await context.SaveChangesAsync();

//        return Ok("Comment has been deleted");
//    }



//    [HttpGet("GetRatingByContentId/{contentId}")]
//    public async Task<ActionResult<List<ReadRatingDto>>> GetRatingByContentId(int contentId)
//    {
//        var ratings = await context.Ratings
//        .Where(r => r.ContentID == contentId)
//        .Select(r => new ReadRatingDto
//        {
//            RatingID = r.RatingID,
//            Type = r.Type.ToString(),
//            UserName = r.User.UserName
//        })
//        .ToListAsync();

//        if (!ratings.Any())
//            return NotFound("No ratings found for this content.");

//        return Ok(ratings);
//    }


//    //[Authorize]
//    [HttpGet("GetUserRatingForContent/{contentId}")]
//    public async Task<ActionResult<UserRatingDto>> GetUserRatingForContent(int contentId)
//    {

//        //var userIdStr = User.FindFirst("uid")?.Value;
//        //if (!int.TryParse(userIdStr, out int userId))
//        //     return Unauthorized("User ID not found in token.");

//        int userId = 1;

//        var rating = await context.Ratings
//        .Where(r => r.UserID == userId && r.ContentID == contentId)
//        .Select(r => new UserRatingDto
//        {
//            ContentID = r.ContentID,
//            UserID = r.UserID,
//            Rating = r.Type.ToString()
//        }).FirstOrDefaultAsync();

//        if (rating == null)
//            return NotFound("No rating for this user on this content");

//        return Ok(rating);
//    }


//    //[Authorize]
//    [HttpPost("AddOrUpdateRating")]
//    public async Task<IActionResult> AddOrUpdateRating([FromBody] AddOrUpdateRatingDto dto)
//    {

//        //var userIdStr = User.FindFirst("uid")?.Value;
//        //if (!int.TryParse(userIdStr, out int userId))
//        //     return Unauthorized("User ID not found in token.");

//        int userId = 1;

//        if (!Enum.TryParse<RatingType>(dto.Rating, true, out var ConvertedToRatingType))
//            return BadRequest("Invalid rating type. Use 'Like' or 'Dislike'.");

//        var existingRating = await context.Ratings
//            .FirstOrDefaultAsync(r => r.UserID == userId && r.ContentID == dto.ContentID);

//        if (existingRating == null)
//        {

//            var newRating = new Rating
//            {
//                ContentID = dto.ContentID,
//                UserID = userId,
//                Type = ConvertedToRatingType
//            };
//            await context.Ratings.AddAsync(newRating);
//        }
//        else if (existingRating.Type == ConvertedToRatingType)
//        {

//            context.Ratings.Remove(existingRating);
//        }
//        else
//        {
//            existingRating.Type = ConvertedToRatingType;
//        }

//        await context.SaveChangesAsync();
//        return Ok("Updated");
//    }


//    //[Authorize]
//    [HttpDelete("DeleteRating/{contentId}")]
//    public async Task<IActionResult> DeleteRating(int contentId)
//    {
//        //var userIdStr = User.FindFirst("uid")?.Value;
//        //if (!int.TryParse(userIdStr, out int userId))
//        //     return Unauthorized("User ID not found in token.");

//        int userId = 1;

//        var rating = await context.Ratings
//            .FirstOrDefaultAsync(r => r.UserID == userId && r.ContentID == contentId);

//        if (rating == null)
//            return NotFound("rating not found for this content.");

//        context.Ratings.Remove(rating);
//        await context.SaveChangesAsync();

//        return Ok("rating deleted successfully.");
//    }
//}

#endregion}
