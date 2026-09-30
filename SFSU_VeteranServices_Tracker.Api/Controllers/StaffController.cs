using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SFSU_VeteranServices_Tracker.Api.Data;
using SFSU_VeteranServices_Tracker.Api.Models;

namespace SFSU_VeteranServices_Tracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly VeteranServicesContext _context;

        private readonly IPasswordHasher<StaffUser> _passwordHasher;

        public StaffController(VeteranServicesContext context,
            IPasswordHasher<StaffUser> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // Only a Manager is allowed to create staff accounts.
        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<ActionResult> CreateStaff(CreateStaffRequest request)
        {
            // Make sure both fields were entered.
            if (string.IsNullOrWhiteSpace(request.Username) || 
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(
                    "Username and password are required.");
            }

            // Don't allow duplicate usernames.
            bool usernameExists = await _context.StaffUsers.AnyAsync(user =>
                    user.Username == request.Username);

            if (usernameExists)
            {
                return BadRequest("That username already exists.");
            }

            StaffUser staffUser = new StaffUser
            {
                Username = request.Username,
                Role = "Staff",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // Hash the password before saving it.
            staffUser.PasswordHash = _passwordHasher.HashPassword(staffUser, request.Password);

            _context.StaffUsers.Add(staffUser);

            await _context.SaveChangesAsync();

            return Ok("Staff account created successfully.");
        }
    }
}