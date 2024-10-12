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
    [Authorize(Roles = "Admin,parent")]
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }
        //[Authorize(Roles = SD.Role_Admin)]
        // GET: Students
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("parent"))
            {
                // Get the ID of the logged-in parent
                var parentId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Assuming you use the NameIdentifier for the parent ID

                // Filter students where the parentId matches the logged-in parent
                var studentsForParent = await _context.students
                    .Where(s => s.parentId == parentId)
                    .Include(s => s.parent)
                    .ToListAsync();

                return View(studentsForParent);
            }

            // For Admins, display all students
            var applicationDbContext = _context.students.Include(s => s.parent);
            return View(await applicationDbContext.ToListAsync());
            //var applicationDbContext = _context.students.Include(s => s.parent);
            //return View(await applicationDbContext.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.students
                .Include(s => s.parent)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }
        //[Authorize(Roles = SD.Role_Admin)]
        // GET: Students/Create
        public IActionResult Create()
        {
            // If the user is an admin, show all parents. If a parent, only show the logged-in parent.
            if (User.IsInRole("Admin"))
            {
                ViewBag.parentId = new SelectList(_context.applicationUsers, "Id", "Email");
            }
            else if (User.IsInRole("parent"))
            {
                var parentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ViewBag.parentId = new SelectList(_context.applicationUsers.Where(u => u.Id == parentId), "Id", "Email", parentId);
            }

            // Populate dropdown for StudentGender using ViewBag
            ViewBag.StudentGender = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "Male", Text = "Male" },
                new SelectListItem { Value = "Female", Text = "Female" }
            }, "Value", "Text");

            // Populate dropdown for StudentGrade using ViewBag
            ViewBag.StudentGrade = new SelectList(new List<SelectListItem>
            {
               new SelectListItem { Value = "8A", Text = "8A" },
               new SelectListItem { Value = "8B", Text = "8B" },
               new SelectListItem { Value = "9A", Text = "9A" },
               new SelectListItem { Value = "9B", Text = "9B" },
               new SelectListItem { Value = "10A", Text = "10A" },
               new SelectListItem { Value = "10B", Text = "10B" },
               new SelectListItem { Value = "11A", Text = "11A" },
               new SelectListItem { Value = "11B", Text = "11B" },
               new SelectListItem { Value = "12A", Text = "12A" },
               new SelectListItem { Value = "12B", Text = "12B" }
            }, "Value", "Text");

            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentId,StudentName,StudentSurname,StudentAge,StudentGender,StudentGrade,StudentAddress,parentId")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["parentId"] = new SelectList(_context.applicationUsers, "Id", "Email", student.parentId);
            return View(student);
        }
        //[Authorize(Roles = SD.Role_Admin)]
        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }


            // If the user is an admin, show all parents. If a parent, only show the logged-in parent.
            if (User.IsInRole("Admin"))
            {
                ViewBag.parentId = new SelectList(_context.applicationUsers, "Id", "Email", student.parentId);
            }
            else if (User.IsInRole("parent"))
            {
                var parentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ViewBag.parentId = new SelectList(_context.applicationUsers.Where(u => u.Id == parentId), "Id", "Email", parentId);
            }


            // Populate dropdown for StudentGender using ViewBag
            ViewBag.StudentGender = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "Male", Text = "Male" },
                new SelectListItem { Value = "Female", Text = "Female" }
            }, "Value", "Text", student.StudentGender);

            // Populate dropdown for StudentGrade using ViewBag
            ViewBag.StudentGrade = new SelectList(new List<SelectListItem>
            {
               new SelectListItem { Value = "8A", Text = "8A" },
               new SelectListItem { Value = "8B", Text = "8B" },
               new SelectListItem { Value = "9A", Text = "9A" },
               new SelectListItem { Value = "9B", Text = "9B" },
               new SelectListItem { Value = "10A", Text = "10A" },
               new SelectListItem { Value = "10B", Text = "10B" },
               new SelectListItem { Value = "11A", Text = "11A" },
               new SelectListItem { Value = "11B", Text = "11B" },
               new SelectListItem { Value = "12A", Text = "12A" },
               new SelectListItem { Value = "12B", Text = "12B" }
            }, "Value", "Text", student.StudentGrade);


            return View(student);
        }


        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StudentId,StudentName,StudentSurname,StudentAge,StudentGender,StudentGrade,StudentAddress,parentId")] Student student)
        {
            if (id != student.StudentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.StudentId))
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
            ViewData["parentId"] = new SelectList(_context.applicationUsers, "Id", "Email", student.parentId);
            return View(student);
        }
        [Authorize(Roles = SD.Role_Admin)]
        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.students
                .Include(s => s.parent)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.students.FindAsync(id);
            if (student != null)
            {
                _context.students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.students.Any(e => e.StudentId == id);
        }
    }
}
