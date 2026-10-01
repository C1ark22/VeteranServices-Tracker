/******************************************************************************
 * Project: SFSU Veteran Services Tracker
 * Author: Clark Batungbakal 
 * Class Name: AuthController.cs
 * 
 * Description:
 * Handles authentication for Staff and Manager accounts.
 * Verifies the username and hashed password against the database and,
 * when the credentials are valid, generates a JWT containing the user's
 * identity and role. Returns an unauthorized response when login fails.
 * 
 * ***************************************************************************/

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SFSU_VeteranServices_Tracker.Api.Data;
using SFSU_VeteranServices_Tracker.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SFSU_VeteranServices_Tracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly VeteranServicesContext _context;
        private readonly IPasswordHasher<StaffUser> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthController(VeteranServicesContext context,
            IPasswordHasher<StaffUser> passwordHasher,
            IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(
            LoginRequest request)
        {
            // Look for the staff member by username.
            StaffUser? user =
                await _context.StaffUsers
                    .FirstOrDefaultAsync(user =>
                        user.Username == request.Username);

            // User doesn't exist or their account was disabled.
            if (user == null || !user.IsActive)
            {
                return Unauthorized("Invalid username or password.");
            }

            // Compare the password entered by the user
            // with the stored password hash.
            PasswordVerificationResult passwordResult =
                _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    request.Password);

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid username or password.");
            }

            string token = CreateToken(user);

            LoginResponse response = new LoginResponse
            {
                Username = user.Username,
                Role = user.Role,
                Token = token
            };

            return Ok(response);
        }

        private string CreateToken(StaffUser user)
        {
            string jwtKey =
                _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "JWT key has not been configured.");

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            SymmetricSecurityKey key =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey));

            SigningCredentials credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token =
                new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(8),
                    signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}
