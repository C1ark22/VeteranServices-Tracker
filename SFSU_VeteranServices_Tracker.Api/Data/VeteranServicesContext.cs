using Microsoft.EntityFrameworkCore;
using SFSU_VeteranServices_Tracker.Api.Models;

namespace SFSU_VeteranServices_Tracker.Api.Data
{
    public class VeteranServicesContext : DbContext
    {
        // Constructor that accepts DbContextOptions and passes them to the base DbContext class
        public VeteranServicesContext(
            DbContextOptions<VeteranServicesContext> options)
            : base(options)
        {
        }

        public DbSet<StudentCheckIn> StudentCheckIns { get; set; }
        public DbSet<StaffUser> StaffUsers { get; set; }
    }
}
