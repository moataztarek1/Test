using System.Security.Claims;

namespace KidZone.API.Services
{
    public class SomeService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SomeService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public int? UserId
        {
            get
            {
                var userIdValue = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdValue, out int userId))
                {
                    return userId;
                }
                return null;
            }
        }
        public string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        //public int? GetCurrentUserId()
        //{
        //    return UserId;
        //}


    }
}
