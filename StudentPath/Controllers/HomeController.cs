using System.Diagnostics;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPath.DB;
using StudentPath.Models;

namespace StudentPath.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context; // Add this line


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context; // Initialize the context
        }

        //[Authorize(Roles = SD.Role_Admin)]
        [Authorize]
        public IActionResult Index()
        {
            if (User.IsInRole("Teacher") || User.IsInRole("Bus_Driver") )
            {
                // Redirect teachers to a different page (for example, a teacher dashboard)
                return RedirectToAction("Welcome", "Home");
            }
            return View();
        }
        public IActionResult Welcome()
        {
            return View();
        }
        public IActionResult EmergencyContact()
        {
            return View();
        }
        public async Task<IActionResult> BusAttendances(DateOnly? attendanceDate) // Add attendanceDate as a parameter
{
    // Check if attendanceDate is provided
    if (attendanceDate != null)
    {
        // Fetch records for the selected date using DateOnly comparison
        var records = await _context.busAttendence
            .Include(b => b.Bus_Driver)
            .Include(b => b.student)
            .Where(b => b.Date == attendanceDate) // Compare using DateOnly
            .ToListAsync();

        if (records.Any())
        {
            return View(records); // Pass the records to the view
        }
        else
        {
            ViewBag.Message = "Records don't exist for the selected date.";
            return View(new List<BusAttendence>()); // Return an empty list
        }
    }

    // If no date is provided, return all records
    var allRecords = await _context.busAttendence
        .Include(b => b.Bus_Driver)
        .Include(b => b.student)
        .ToListAsync();

    return View(allRecords); // Pass all records to the view
}

        public async Task<IActionResult> ClassAttendance(DateOnly? attendanceDate) // Marked as async
        {
            // Check if attendanceDate is provided
            if (attendanceDate != null)
            {
                // Fetch records for the current date
                var records1 = await _context.classAttendence
                .Include(c => c.Teacher)
                .Include(c => c.student)
                .Where(c => c.Date == attendanceDate) // Compare using DateOnly
                .ToListAsync();

                if (records1.Any())
                {
                    return View(records1); // Pass the records to the view
                }
                else
                {
                    ViewBag.Message = "Records don't exist for the selected date.";
                    return View(new List<ClassAttendence>()); // Return an empty list
                }
            }
                var records = await _context.classAttendence
                    .Include(c => c.Teacher)
                    .Include(c => c.student)
                    
                    .ToListAsync();
                return View(records); // Pass the records to the view
            
        }

        //public IActionResult Notifications()
        //{
        //    return View();
        //}
        public async Task<IActionResult> Notifications(DateOnly? date)
        {
            // Get the current logged-in user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Adjust this if you're using a different claim type

            // Get the current user's role
            var isAdmin = User.IsInRole("Admin"); // Assuming you're using role-based authorization

            IQueryable<Notification> notificationsQuery = _context.notifications;

            // If the user is not an admin, filter by their child's ID
            if (!isAdmin)
            {
                var childIds = await _context.students
                    .Where(s => s.parentId == userId) // Assuming you have a ParentId property in the Student model
                    .Select(s => s.StudentId) // Get the IDs of the children
                    .ToListAsync();

                notificationsQuery = notificationsQuery.Where(n => childIds.Contains(n.StudentId)); // Filter notifications by child's ID
            }

            // If a date is provided, filter notifications by that date
            if (date.HasValue)
            {
                // Ensure date filtering uses the correct DateOnly comparison
                notificationsQuery = notificationsQuery.Where(n => n.AttendanceDate == date.Value);
            }

            // Fetch the filtered notifications
            var notifications = await notificationsQuery.ToListAsync();

            if (notifications == null || !notifications.Any())
            {
                ViewBag.Message = "No notifications found.";
            }

            return View(notifications);
        }
        public async Task<IActionResult> ClassNotifications(DateOnly? date)
        {
            // Assuming you have a way to get the current user ID and role
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get current user's ID
            var isAdmin = User.IsInRole("Admin"); // Check if the user is an admin

            // Start with the base query for class notifications
            IQueryable<ClassNotification> classNotificationsQuery = _context.classNotifications;

            // If the user is not an admin, filter notifications by the logged-in student's ID
            if (!isAdmin)
            {
                // Assuming you have a way to get the student's ID associated with the parent
                // You may need to adjust this based on how your application tracks parent-student relationships
                var studentIds = _context.students
                    .Where(s => s.parentId == userId) // Filter students by parent ID
                    .Select(s => s.StudentId); // Get the IDs of the students

                classNotificationsQuery = classNotificationsQuery.Where(n => studentIds.Contains(n.StudentId));
            }

            // If a date is provided, filter class notifications by that date
            if (date.HasValue)
            {
                classNotificationsQuery = classNotificationsQuery.Where(n => n.Date == date.Value);
            }

            var classNotifications = await classNotificationsQuery
                .Include(n => n.Teacher) // Assuming you want to include related data like Teacher
                .Include(n => n.students) // Include student details
                .ToListAsync();

            if (classNotifications == null || !classNotifications.Any())
            {
                ViewBag.Message = "No class notifications found.";
            }

            return View(classNotifications);
        }




        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
       
    }
}
