using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    [Authorize(Roles = "Admin,Bus_Driver")]
    public class BusAttendencesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BusAttendencesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BusAttendences
        //[Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Index(DateOnly? attendanceDate)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (attendanceDate != null)
            {
                // Fetch records for the selected date using DateOnly comparison
                var records = await _context.busAttendence
                    .Include(b => b.Bus_Driver)
                    .Include(b => b.student)
                    .Where(b => b.Date == attendanceDate && (User.IsInRole("Admin") || b.Bus_DriverId == currentUserId)) // Only show records for the current user or admin
                    .ToListAsync();

                if (records.Any())
                {
                    return View(records); // Pass the records to the view
                }
                else
                {
                    ViewBag.Message = "Records don't exist for the selected date.";
                    return View(new List<BusAttendence>()); // Return empty list
                }
            }

            // If no date is provided, return all records for the logged-in user
            var applicationDbContext = _context.busAttendence
                .Include(b => b.Bus_Driver)
                .Include(b => b.student)
                .Where(b => User.IsInRole("Admin") || b.Bus_DriverId == currentUserId); // Filter records based on user role

            return View(await applicationDbContext.ToListAsync());
        }



        // GET: BusAttendences/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var busAttendence = await _context.busAttendence
                .Include(b => b.Bus_Driver)
                .Include(b => b.student)
                .FirstOrDefaultAsync(m => m.BusId == id);
            if (busAttendence == null)
            {
                return NotFound();
            }

            return View(busAttendence);
        }
        [Authorize(Roles = "Admin,Bus_Driver")]


        // GET: BusAttendences/Create
        public IActionResult Create()
        {
            // Fetch students based on their grades and corresponding bus numbers
            var eighthGraders = _context.students
                .Where(s => s.StudentGrade.StartsWith("8"))
                .ToList();
            var ninthGraders = _context.students
                .Where(s => s.StudentGrade.StartsWith("9"))
                .ToList();
            var tenthGraders = _context.students
                .Where(s => s.StudentGrade.StartsWith("10"))
                .ToList();
            var eleventhGraders = _context.students
                .Where(s => s.StudentGrade.StartsWith("11"))
                .ToList();
            var twelfthGraders = _context.students
                .Where(s => s.StudentGrade.StartsWith("12"))
                .ToList();

            // Combine all lists into a single ViewBag
            ViewBag.StudentList = new List<Student>(); // Assuming 'Student' is your model
            ViewBag.StudentList.AddRange(eighthGraders);
            ViewBag.StudentList.AddRange(ninthGraders);
            ViewBag.StudentList.AddRange(tenthGraders);
            ViewBag.StudentList.AddRange(eleventhGraders);
            ViewBag.StudentList.AddRange(twelfthGraders);


            // Populate Bus_DriverId dropdown
            var isAdmin = User.IsInRole("Admin");
            if (isAdmin)
            {
                ViewBag.Bus_DriverId = new SelectList(_context.applicationUsers, "Id", "Email");
            }
            else
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ViewBag.Bus_DriverId = new SelectList(new List<ApplicationUser> { _context.applicationUsers.Find(currentUserId) }, "Id", "Email");
            }

            return View(new List<BusAttendence>(10)); // Create a list with 10 items
        }


        // POST: BusAttendences/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Create(List<BusAttendence> busAttendences)
        {
            if (ModelState.IsValid)
            {
                // Add all bus attendance records to the context
                _context.AddRange(busAttendences);

                // Loop through the attendance records to find absentees
                var absentees = new List<Notification>();
                foreach (var attendence in busAttendences)
                {
                    if (attendence.Morning_Attendance == "Absent" || attendence.Afternoon_Attendance == "Absent")
                    {
                        var student = await _context.students.FindAsync(attendence.StudentId);
                        var notification = new Notification
                        {
                            StudentId = student.StudentId,
                            StudentName = student.StudentName,
                            BusNumber = attendence.BusNumber,
                            AttendanceDate = attendence.Date,
                            MorningAttendance = attendence.Morning_Attendance,
                            AfternoonAttendance = attendence.Afternoon_Attendance
                        };
                        absentees.Add(notification);
                    }
                }

                // Save notifications
                if (absentees.Count > 0)
                {
                    _context.notifications.AddRange(absentees);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Fetch students based on their grades and corresponding bus numbers
            var eighthGraders = _context.students
                .Where(s => s.StudentGrade.StartsWith("8"))
                .ToList();
            var ninthGraders = _context.students
                .Where(s => s.StudentGrade.StartsWith("9"))
                .ToList();
            var tenthGraders = _context.students
                .Where(s => s.StudentGrade.StartsWith("10"))
                .ToList();
            var eleventhGraders = _context.students
                .Where(s => s.StudentGrade.StartsWith("11"))
                .ToList();
            var twelfthGraders = _context.students
                .Where(s => s.StudentGrade.StartsWith("12"))
                .ToList();

            // Combine all lists into a single ViewBag
            ViewBag.StudentList = new List<Student>(); // Assuming 'Student' is your model
            ViewBag.StudentList.AddRange(eighthGraders);
            ViewBag.StudentList.AddRange(ninthGraders);
            ViewBag.StudentList.AddRange(tenthGraders);
            ViewBag.StudentList.AddRange(eleventhGraders);
            ViewBag.StudentList.AddRange(twelfthGraders);


            var isAdmin = User.IsInRole("Admin");
            if (isAdmin)
            {
                ViewBag.Bus_DriverId = new SelectList(_context.applicationUsers, "Id", "Email");
            }
            else
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ViewBag.Bus_DriverId = new SelectList(new List<ApplicationUser> { _context.applicationUsers.Find(currentUserId) }, "Id", "Email");
            }

            return View(busAttendences);
        }


        // GET: BusAttendences/Edit/5
        [Authorize(Roles = "Admin,Bus_Driver")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var busAttendence = await _context.busAttendence.FindAsync(id);
            if (busAttendence == null)
            {
                return NotFound();
            }
            // Check if the logged-in user is an admin
            var isAdmin = User.IsInRole("Admin");

            // Populate Bus_DriverId dropdown
            if (isAdmin)
            {
                ViewBag.Bus_DriverId = new SelectList(_context.applicationUsers, "Id", "Email", busAttendence.Bus_DriverId);
            }
            else
            {
                // If not admin, only show the current driver's ID
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ViewBag.Bus_DriverId = new SelectList(new List<ApplicationUser> { _context.applicationUsers.Find(currentUserId) }, "Id", "Email", busAttendence.Bus_DriverId);
            }

            // Populate StudentId dropdown
            ViewBag.StudentId = new SelectList(_context.students, "StudentId", "StudentName", busAttendence.StudentId);

            // Dropdown for Morning Attendance
            ViewBag.Morning_Attendance = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "Present", Text = "Present" },
                new SelectListItem { Value = "Absent", Text = "Absent" }
            }, "Value", "Text", busAttendence.Morning_Attendance);

            // Dropdown for Afternoon Attendance
            ViewBag.Afternoon_Attendance = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "Present", Text = "Present" },
                new SelectListItem { Value = "Absent", Text = "Absent" }
            }, "Value", "Text", busAttendence.Afternoon_Attendance);

            // Dropdown for Bus Number
            ViewBag.BusNumber = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "1010", Text = "1010" },
                new SelectListItem { Value = "1011", Text = "1011" },
                new SelectListItem { Value = "1012", Text = "1012" },
                new SelectListItem { Value = "1013", Text = "1013" },
                new SelectListItem { Value = "1014", Text = "1014" }
            }, "Value", "Text", busAttendence.BusNumber);



            return View(busAttendence);
        }

        // POST: BusAttendences/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BusId,BusNumber,StudentId,Date,Morning_Attendance,Afternoon_Attendance,Bus_DriverId")] BusAttendence busAttendence)
        {
            if (id != busAttendence.BusId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(busAttendence);
                    await _context.SaveChangesAsync();


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BusAttendenceExists(busAttendence.BusId))
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

            // If we reach here, it means there was a validation error.
            // Repopulate dropdowns in case of a validation error
            var isAdmin = User.IsInRole("Admin");
            if (isAdmin)
            {
                ViewBag.Bus_DriverId = new SelectList(_context.applicationUsers, "Id", "Email", busAttendence.Bus_DriverId);
            }
            else
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ViewBag.Bus_DriverId = new SelectList(new List<ApplicationUser> { _context.applicationUsers.Find(currentUserId) }, "Id", "Email", busAttendence.Bus_DriverId);
            }

            ViewBag.StudentId = new SelectList(_context.students, "StudentId", "StudentName", busAttendence.StudentId);

            ViewBag.Morning_Attendance = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "Present", Text = "Present" },
                new SelectListItem { Value = "Absent", Text = "Absent" }
            }, "Value", "Text", busAttendence.Morning_Attendance);

            ViewBag.Afternoon_Attendance = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "Present", Text = "Present" },
                new SelectListItem { Value = "Absent", Text = "Absent" }
            }, "Value", "Text", busAttendence.Afternoon_Attendance);

            ViewBag.BusNumber = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "1010", Text = "1010" },
                new SelectListItem { Value = "1011", Text = "1011" },
                new SelectListItem { Value = "1012", Text = "1012" },
                new SelectListItem { Value = "1013", Text = "1013" },
                new SelectListItem { Value = "1014", Text = "1014" }
            }, "Value", "Text", busAttendence.BusNumber);


            return View(busAttendence);
        }

        // GET: BusAttendences/Delete/5
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var busAttendence = await _context.busAttendence
                .Include(b => b.Bus_Driver)
                .Include(b => b.student)
                .FirstOrDefaultAsync(m => m.BusId == id);
            if (busAttendence == null)
            {
                return NotFound();
            }

            return View(busAttendence);
        }

        // POST: BusAttendences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var busAttendence = await _context.busAttendence.FindAsync(id);
            if (busAttendence != null)
            {
                _context.busAttendence.Remove(busAttendence);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BusAttendenceExists(int id)
        {
            return _context.busAttendence.Any(e => e.BusId == id);
        }

        public async Task<IActionResult> Notifications(DateOnly? date)
        {
            IQueryable<Notification> notificationsQuery = _context.notifications;

            // If a date is provided, filter notifications by that date
            if (date.HasValue)
            {
                notificationsQuery = notificationsQuery.Where(n => n.AttendanceDate == date.Value);
            }

            var notifications = await notificationsQuery.ToListAsync();

            if (notifications == null || !notifications.Any())
            {
                ViewBag.Message = "No notifications found.";
            }

            return View(notifications);
        }



    }
}
