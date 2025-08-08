using KidZone.API.DTOs;
using KidZone.Domain.Entities;
using KidZone.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KidZone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        [HttpPost("register-parent")]
        public async Task<IActionResult> RegisterParent(RegisterDto model)
        {
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, "Parent");

            return Ok(new { message = "Parent registered successfully" });
        }

        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin(RegisterDto model)
        {
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, "Admin");

            return Ok(new { message = "Admin registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized("Invalid email or password");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return Unauthorized("Invalid email or password");

            var roles = await _userManager.GetRolesAsync(user);

            var token = GenerateJwtToken(user, roles.FirstOrDefault());

            return Ok(new
            {
                token,
                role = roles.FirstOrDefault()
            });
        }



        #region Test
        //[HttpGet("admin")]
        //[Authorize(Roles = "Admin")]
        //public IActionResult AdminOnly()
        //{
        //    return Ok("Welcome Admin, you have access to this endpoint.");
        //}

        //[HttpGet("parent")]
        //[Authorize(Roles = "Parent")]
        //public IActionResult ParentOnly()
        //{
        //    return Ok("Welcome Parent, you have access to this endpoint.");
        //} 
        #endregion

        private string GenerateJwtToken(User user, string role)
        {
            //Console.WriteLine($"GenerateJwtToken -> User.Id: {user.Id}");
            //Console.WriteLine($"GenerateJwtToken -> User.Email: {user.Email}");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),          
                new Claim(ClaimTypes.NameIdentifier, user.Id),             
                new Claim("uid", user.Id),                                
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role ?? "")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
