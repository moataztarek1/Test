using KidZone.API.Data;
using KidZone.API.DTOs;
using KidZone.Domain.Entities;
using KidZone.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KidZone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly DataContext _context;

        public ContentController(DataContext context)
        {
            _context = context;
        }

        // Create

        [HttpPost("video")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateVideo([FromBody] CreateVideoDto dto)
        {
            var content = new Content
            {
                Title = dto.Title,
                Description = dto.Description,
                ContentType = ContentAccessType.Video
            };

            var video = new Video
            {
                Url = dto.Url,
                Duration = dto.Duration,
                Content = content
            };
            content.Video = video;

            foreach (var categoryId in dto.CategoryIds)
            {
                var exists = await _context.Categories.AnyAsync(c => c.CategoryID == categoryId);
                if (!exists) return BadRequest($"Category ID {categoryId} not found");

                content.ContentCategories.Add(new ContentCategory
                {
                    CategoryID = categoryId,
                    Content = content
                });
            }

            await _context.Contents.AddAsync(content);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Video content created successfully", contentId = content.ContentID });
        }

        [HttpPost("story")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateStory([FromBody] CreateStoryDto dto)
        {
            var content = new Content
            {
                Title = dto.Title,
                Description = dto.Description,
                ContentType = ContentAccessType.Story
            };

            var story = new Story
            {
                Text_Content = dto.Text_Content,
                ImgUrl = dto.ImgUrl,
                Content = content
            };
            content.Story = story;

            foreach (var categoryId in dto.CategoryIds)
            {
                var exists = await _context.Categories.AnyAsync(c => c.CategoryID == categoryId);
                if (!exists) return BadRequest($"Category ID {categoryId} not found");

                content.ContentCategories.Add(new ContentCategory
                {
                    CategoryID = categoryId,
                    Content = content
                });
            }

            await _context.Contents.AddAsync(content);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Story content created successfully", contentId = content.ContentID });
        }

        // Read
        [HttpGet("videos")]
        public async Task<IActionResult> GetAllVideos()
        {
            var videos = await _context.Contents
                .Include(c => c.Video)
                .Include(c => c.ContentCategories).ThenInclude(cc => cc.Category)
                .Where(c => c.ContentType == ContentAccessType.Video)
                .ToListAsync();

            var result = videos.Select(content => new ContentDetailsDto
            {
                ContentID = content.ContentID,
                Title = content.Title,
                Description = content.Description,
                ContentType = content.ContentType,
                Url = content.Video?.Url,
                Duration = content.Video?.Duration,
                Categories = content.ContentCategories.Select(cc => cc.Category.Name).ToList()
            }).ToList();

            return Ok(result);
        }

        [HttpGet("stories")]
        public async Task<IActionResult> GetAllStories()
        {
            var stories = await _context.Contents
                .Include(c => c.Story)
                .Include(c => c.ContentCategories).ThenInclude(cc => cc.Category)
                .Where(c => c.ContentType == ContentAccessType.Story)
                .ToListAsync();

            var result = stories.Select(content => new ContentDetailsDto
            {
                ContentID = content.ContentID,
                Title = content.Title,
                Description = content.Description,
                ContentType = content.ContentType,
                Text_Content = content.Story?.Text_Content,
                ImgUrl = content.Story?.ImgUrl,
                Categories = content.ContentCategories.Select(cc => cc.Category.Name).ToList()
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContentById(int id)
        {
            var content = await _context.Contents
                .Include(c => c.Video)
                .Include(c => c.Story)
                .Include(c => c.ContentCategories).ThenInclude(cc => cc.Category)
                .FirstOrDefaultAsync(c => c.ContentID == id);

            if (content == null) return NotFound("Content not found");

            var dto = new ContentDetailsDto
            {
                ContentID = content.ContentID,
                Title = content.Title,
                Description = content.Description,
                ContentType = content.ContentType,
                Url = content.Video?.Url,
                Duration = content.Video?.Duration,
                Text_Content = content.Story?.Text_Content,
                ImgUrl = content.Story?.ImgUrl,
                Categories = content.ContentCategories.Select(cc => cc.Category.Name).ToList()
            };

            return Ok(dto);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllContents()
        {
            var contents = await _context.Contents
                .Include(c => c.Video)
                .Include(c => c.Story)
                .Include(c => c.ContentCategories).ThenInclude(cc => cc.Category)
                .ToListAsync();

            var result = contents.Select(content => new ContentDetailsDto
            {
                ContentID = content.ContentID,
                Title = content.Title,
                Description = content.Description,
                ContentType = content.ContentType,
                Url = content.Video?.Url,
                Duration = content.Video?.Duration,
                Text_Content = content.Story?.Text_Content,
                ImgUrl = content.Story?.ImgUrl,
                Categories = content.ContentCategories.Select(cc => cc.Category.Name).ToList()
            }).ToList();

            return Ok(result);
        }

        // Update

        [HttpPut("video/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateVideo(int id, [FromBody] UpdateVideoDto dto)
        {
            var content = await _context.Contents
                .Include(c => c.Video)
                .Include(c => c.ContentCategories)
                .FirstOrDefaultAsync(c => c.ContentID == id && c.ContentType == ContentAccessType.Video);

            if (content == null) return NotFound("Video content not found");

            if (!string.IsNullOrEmpty(dto.Title)) content.Title = dto.Title;
            if (!string.IsNullOrEmpty(dto.Description)) content.Description = dto.Description;
            if (!string.IsNullOrEmpty(dto.Url)) content.Video.Url = dto.Url;
            if (dto.Duration.HasValue) content.Video.Duration = dto.Duration;

            if (dto.CategoryIds != null)
            {
                content.ContentCategories.Clear();
                foreach (var categoryId in dto.CategoryIds)
                {
                    var exists = await _context.Categories.AnyAsync(c => c.CategoryID == categoryId);
                    if (!exists) return BadRequest($"Category ID {categoryId} not found");

                    content.ContentCategories.Add(new ContentCategory
                    {
                        CategoryID = categoryId,
                        ContentID = content.ContentID
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Video content updated successfully" });
        }

        [HttpPut("story/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStory(int id, [FromBody] UpdateStoryDto dto)
        {
            var content = await _context.Contents
                .Include(c => c.Story)
                .Include(c => c.ContentCategories)
                .FirstOrDefaultAsync(c => c.ContentID == id && c.ContentType == ContentAccessType.Story);

            if (content == null) return NotFound("Story content not found");

            if (!string.IsNullOrEmpty(dto.Title)) content.Title = dto.Title;
            if (!string.IsNullOrEmpty(dto.Description)) content.Description = dto.Description;
            if (!string.IsNullOrEmpty(dto.Text_Content)) content.Story.Text_Content = dto.Text_Content;
            if (!string.IsNullOrEmpty(dto.ImgUrl)) content.Story.ImgUrl = dto.ImgUrl;

            if (dto.CategoryIds != null)
            {
                content.ContentCategories.Clear();
                foreach (var categoryId in dto.CategoryIds)
                {
                    var exists = await _context.Categories.AnyAsync(c => c.CategoryID == categoryId);
                    if (!exists) return BadRequest($"Category ID {categoryId} not found");

                    content.ContentCategories.Add(new ContentCategory
                    {
                        CategoryID = categoryId,
                        ContentID = content.ContentID
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Story content updated successfully" });
        }

        // Delete

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteContent(int id)
        {
            var content = await _context.Contents.FirstOrDefaultAsync(c => c.ContentID == id);
            if (content == null) return NotFound("Content not found");

            _context.Contents.Remove(content);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Content deleted successfully" });
        }
    }


}
