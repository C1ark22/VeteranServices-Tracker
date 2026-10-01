/******************************************************************************
 * Project: SFSU Veteran Services Tracker
 * Author: Clark Batungbakal 
 * Class Name: Program.cs
 * 
 * Description:
 * Configures and starts the ASP.NET Core Web API for the Veteran Services
 * Tracker. Registers application services such as controllers, Entity
 * Framework Core, Azure SQL database access, password hashing, JWT
 * authentication, and role-based authorization. It also creates the
 * initial Manager account when needed and starts the API application.
 * 
 * ***************************************************************************/

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SFSU_VeteranServices_Tracker.Api.Data;
using SFSU_VeteranServices_Tracker.Api.Models;
using System.Text;

namespace SFSU_VeteranServices_Tracker.Api
{
    public class Program
    {
        // The main entry point for the application
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add controllers to the API
            builder.Services.AddControllers();

            // Used to securely hash and verify staff passwords.
            builder.Services.AddScoped<IPasswordHasher<StaffUser>,PasswordHasher<StaffUser>>();

            // Get my secret JWT key. If it doesn't exist, stop the application.
            // Tell ASP.NET Core that we're using JWT authentication. When someone sends a token,
            // make sure it hasn't expired and make sure it was signed using my application's
            // secret key.
            string jwtKey = builder.Configuration["Jwt:Key"] 
                ?? throw new InvalidOperationException("JWT key has not been configured.");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = false,
                            ValidateAudience = false,

                            ValidateLifetime = true,

                            ValidateIssuerSigningKey = true,

                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(jwtKey))
                        };
                });

            builder.Services.AddAuthorization();

            // Allows Swagger to discover API endpoints
            builder.Services.AddEndpointsApiExplorer();

            // Creates the Swagger documentation
            builder.Services.AddSwaggerGen();

            // Register EF Core and the database
            builder.Services.AddDbContext<VeteranServicesContext>(
                options =>
                    options.UseSqlServer(
                        builder.Configuration.GetConnectionString(
                            "VeteranServicesDatabase")));

            var app = builder.Build();

            // Create the first manager account if one does not already exist.
            using (var scope = app.Services.CreateScope())
            {
                VeteranServicesContext context =
                    scope.ServiceProvider
                        .GetRequiredService<VeteranServicesContext>();

                IPasswordHasher<StaffUser> passwordHasher =
                    scope.ServiceProvider
                        .GetRequiredService<IPasswordHasher<StaffUser>>();

                IConfiguration configuration =
                    scope.ServiceProvider
                        .GetRequiredService<IConfiguration>();

                // Check if a manager already exists in the database.
                bool managerExists =
                    context.StaffUsers.Any(user =>
                        user.Role == "Manager");

                if (!managerExists)
                {
                    string? username =
                        configuration["BootstrapManager:Username"];

                    string? password =
                        configuration["BootstrapManager:Password"];

                    if (string.IsNullOrWhiteSpace(username) ||
                        string.IsNullOrWhiteSpace(password))
                    {
                        throw new InvalidOperationException(
                            "Bootstrap manager credentials are missing.");
                    }

                    StaffUser manager = new StaffUser
                    {
                        Username = username,
                        Role = "Manager",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    // Hash the password before saving it.
                    manager.PasswordHash =
                        passwordHasher.HashPassword(
                            manager,
                            password);

                    context.StaffUsers.Add(manager);

                    context.SaveChanges();
                }
            }

            // Only use Swagger while developing the application
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
