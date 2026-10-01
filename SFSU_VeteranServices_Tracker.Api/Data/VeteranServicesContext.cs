/******************************************************************************
 * Project: SFSU Veteran Services Tracker
 * Author: Clark Batungbakal 
 * Class Name: VeteranServicesContext.cs
 * 
 * Description:
 * Represents the Entity Framework Core database context for the application.
 * Defines the StudentCheckIns and StaffUsers database tables through DbSet
 * properties and provides the API with access to read, add, update, and save
 * data in the database.
 * 
 * ***************************************************************************/

using Microsoft.EntityFrameworkCore;
using SFSU_VeteranServices_Tracker.Api.Models;

namespace SFSU_VeteranServices_Tracker.Api.Data
{
    public class VeteranServicesContext : DbContext
    {
        // Constructor that accepts DbContextOptions and passes them to the base DbContext class
        public VeteranServicesContext(
            DbContextOptions<VeteranServicesContext> options): base(options)
        {
        }

        public DbSet<StudentCheckIn> StudentCheckIns { get; set; }
        public DbSet<StaffUser> StaffUsers { get; set; }
    }
}
