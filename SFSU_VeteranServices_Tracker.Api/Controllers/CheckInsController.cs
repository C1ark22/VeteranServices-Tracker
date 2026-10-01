/******************************************************************************
 * Project: SFSU Veteran Services Tracker
 * Author: Clark Batungbakal 
 * Class Name: CheckInsController.cs
 * 
 * Description:
 * Handles API requests for student check-in records.
 * Allows the application to create new student check-ins and retrieve
 * existing check-in history. Uses Entity Framework Core to communicate
 * with the database and protects check-in history so only authenticated
 * Staff and Manager users can access it.
 * 
 * ***************************************************************************/

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SFSU_VeteranServices_Tracker.Api.Data;
using SFSU_VeteranServices_Tracker.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace SFSU_VeteranServices_Tracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckInsController : ControllerBase
    {
        // Dependency injection of the database context
        private readonly VeteranServicesContext _context;

        public CheckInsController(VeteranServicesContext context)
        {
            _context = context;
        }


        // GET: api/checkins
        [Authorize(Roles = "Manager,Staff")]
        [HttpGet]
        public async Task<ActionResult<List<StudentCheckIn>>> GetCheckIns()
        {
            List<StudentCheckIn> checkIns =
                await _context.StudentCheckIns
                    .OrderByDescending(student =>
                        student.CheckInTime)
                    .ToListAsync();

            return Ok(checkIns);
        }


        // POST: api/checkins
        [HttpPost]
        public async Task<ActionResult<StudentCheckIn>> CreateCheckIn(
            StudentCheckIn student)
        {
            // Add the new check-in to the database
            _context.StudentCheckIns.Add(student);

            await _context.SaveChangesAsync();

            return Ok(student);
        }
    }
}