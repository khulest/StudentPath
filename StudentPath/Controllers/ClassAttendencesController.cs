using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentPath.DB;
using StudentPath.Models;

namespace StudentPath.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class ClassAttendencesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClassAttendencesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ClassAttendences
        public async Task<IActionResult> Index(DateOnly? attendanceDate)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (attendanceDate != null)
            {
                // Fetch records for the selected date using DateOnly comparison
                var records = await _context.classAttendence
                    .Include(c => c.Teacher)
                    .Include(c => c.student)
                    .Where(c => c.Date == attendanceDate && (User.IsInRole("Admin") || c.TeacherId == currentUserId)) // Show records for current user or admin
                    .ToListAsync();

                if (records.Any())
                {
                    return View(records); // Pass the records to the view
                }
                else
                {
                    ViewBag.Message = "Records don't exist for the selected date.";
                    return View(new List<ClassAttendence>()); // Return empty list
                }
            }

            // If no date is provided, return all records for the logged-in user
            var applicationDbContext = _context.classAttendence
                .Include(c => c.Teacher)
                .Include(c => c.student)
                .Where(c => User.IsInRole("Admin") || c.TeacherId == currentUserId); // Filter records based on user role

            return View(await applicationDbContext.ToListAsync());
        }


        // GET: ClassAttendences/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classAttendence = await _context.classAttendence
                .Include(c => c.Teacher)
                .Include(c => c.student)
                .FirstOrDefaultAsync(m => m.ClassId == id);
            if (classAttendence == null)
            {
                return NotFound();
            }

            return View(classAttendence);
        }

        // GET: ClassAttendences/Create
        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the logged-in user's ID
            var isTeacher = User.IsInRole("Teacher"); // Check if the logged-in user is a teacher
            var isAdmin = User.IsInRole("Admin"); // Check if the logged-in user is an admin

            if (isTeacher)
            {
                // Restrict to logged-in teacher
                ViewBag.TeacherId = new SelectList(_context.applicationUsers.Where(u => u.Id == userId), "Id", "Email");
            }
            else if (isAdmin)
            {
                // Allow admin to select from all teachers
                ViewBag.TeacherId = new SelectList(_context.applicationUsers, "Id", "Email");
            }

            ViewBag.StudentId = new SelectList(_context.students, "StudentId", "StudentName");

            // Create a list of 10 empty ClassAttendence objects
            var attendances = new List<ClassAttendence>();
            for (int i = 0; i < 10; i++)
            {
                attendances.Add(new ClassAttendence());
            }

            return View(attendances);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<ClassAttendence> classAttendences)
        {
            if (ModelState.IsValid)
            {
                var currentDate = DateOnly.FromDateTime(DateTime.Now); // Get the current date
               
                

                foreach (var classAttendence in classAttendences)
                {
                    // Set the date to current date if not already set
                    classAttendence.Date = currentDate;
                    _context.Add(classAttendence);

                    // If the student is marked absent, create a ClassNotification
                    if (classAttendence.Attended == "Absent")
                    {
                        var notification = new ClassNotification
                        {
                            Class = classAttendence.Class,
                            TeacherId = classAttendence.TeacherId,
                            StudentId = classAttendence.StudentId,
                            Date = currentDate,
                            Status = "Absent"
                        };

                        _context.classNotifications.Add(notification);
                    }

                }
                
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the logged-in user's ID
            var isTeacher = User.IsInRole("Teacher");
            var isAdmin = User.IsInRole("Admin");

            if (isTeacher)
            {
                ViewBag.TeacherId = new SelectList(_context.applicationUsers.Where(u => u.Id == userId), "Id", "Email");
            }
            else if (isAdmin)
            {
                ViewBag.TeacherId = new SelectList(_context.applicationUsers, "Id", "Email");
            }

            ViewBag.StudentId = new SelectList(_context.students, "StudentId", "StudentName");

            return View(classAttendences);
        }



        // GET: ClassAttendences/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classAttendence = await _context.classAttendence.FindAsync(id);
            if (classAttendence == null)
            {
                return NotFound();
            }

            // Dropdown for class
            ViewBag.Class = new SelectList(new List<string> { "8A", "8B", "9A", "9B", "10A", "10B", "11A", "11B", "12A", "12B" });

            // Date must be DateOnly; display current date
            classAttendence.Date = DateOnly.FromDateTime(DateTime.Now);

            // Set TeacherId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (User.IsInRole("Teacher"))
            {
                ViewBag.TeacherId = new SelectList(_context.applicationUsers.Where(u => u.Id == userId), "Id", "Email", classAttendence.TeacherId);
            }
            else if (User.IsInRole("Admin"))
            {
                ViewBag.TeacherId = new SelectList(_context.applicationUsers, "Id", "Email", classAttendence.TeacherId);
            }

            ViewBag.StudentId = new SelectList(_context.students, "StudentId", "StudentName", classAttendence.StudentId);

            // Set Attend dropdown
            ViewBag.Attend = new SelectList(new List<string> { "Unmarked", "Absent", "Present" }, classAttendence.Attended);

            return View(classAttendence);
        }

        // POST: ClassAttendences/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("ClassId,Class,StudentId,Date,Attended,TeacherId")] ClassAttendence classAttendence)
        {
            if (id != classAttendence.ClassId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(classAttendence);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassAttendenceExists(classAttendence.ClassId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Class = new SelectList(new List<string> { "8A", "8B", "9A", "9B", "10A", "10B", "11A", "11B", "12A", "12B" }, classAttendence.Class);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (User.IsInRole("Teacher"))
            {
                ViewBag.TeacherId = new SelectList(_context.applicationUsers.Where(u => u.Id == userId), "Id", "Email", classAttendence.TeacherId);
            }
            else if (User.IsInRole("Admin"))
            {
                ViewBag.TeacherId = new SelectList(_context.applicationUsers, "Id", "Email", classAttendence.TeacherId);
            }

            ViewBag.StudentId = new SelectList(_context.students, "StudentId", "StudentName", classAttendence.StudentId);

            // Set Attend dropdown
            ViewBag.Attend = new SelectList(new List<string> { "Unmarked", "Absent", "Present" }, classAttendence.Attended);

            return View(classAttendence);
        }




        // GET: ClassAttendences/Delete/5
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classAttendence = await _context.classAttendence
                .Include(c => c.Teacher)
                .Include(c => c.student)
                .FirstOrDefaultAsync(m => m.ClassId == id);
            if (classAttendence == null)
            {
                return NotFound();
            }

            return View(classAttendence);
        }

        // POST: ClassAttendences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var classAttendence = await _context.classAttendence.FindAsync(id);
            if (classAttendence != null)
            {
                _context.classAttendence.Remove(classAttendence);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassAttendenceExists(int id)
        {
            return _context.classAttendence.Any(e => e.ClassId == id);
        }

        public async Task<IActionResult> ClassNotifications(DateOnly? date)
        {
            IQueryable<ClassNotification> classNotificationsQuery = _context.classNotifications;

            // If a date is provided, filter class notifications by that date
            if (date.HasValue)
            {
                classNotificationsQuery = classNotificationsQuery.Where(n => n.Date == date.Value);
            }

            var classNotifications = await classNotificationsQuery
                .Include(n => n.Teacher)  // Assuming you want to include related data like Teacher
                .Include(n => n.students)  // Include student details
                .ToListAsync();

            if (classNotifications == null || !classNotifications.Any())
            {
                ViewBag.Message = "No class notifications found.";
            }

            return View(classNotifications);
        }




    }
}
