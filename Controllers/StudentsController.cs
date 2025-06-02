using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortalDBFirst.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace StudentPortalDBFirst.Controllers
{
    [Authorize]
    public class StudentsController : Controller
    {
        private readonly StudentDbContext _context;

        public StudentsController(StudentDbContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            // Explicitly cast to IEnumerable<Students> to match the view's model type
            IEnumerable<Students> students = await _context.Students.ToListAsync();
            return View(students);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var students = await _context.Students.FirstOrDefaultAsync(m => m.Id == id);
            if (students == null)
                return NotFound();

            return View(students);
        }

        // GET: Students/Create
        public IActionResult Create()

        {
            return View();
        }
        //[Bind("Id,StudentName,Email,PhoneNumber,CollegeName,Address,Gender")]
        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Students students)
        {
            if (ModelState.IsValid)
            {
                _context.Add(students);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Student created successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(students);  // data view in create
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var students = await _context.Students.FindAsync(id);
            if (students == null)
                return NotFound();

            return View(students);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentName,Email,PhoneNumber,CollegeName,Address,Gender")] Students students)
        {
            if (id != students.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(students);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Student updated successfully";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentsExists(students.Id))
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
            return View(students);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var students = await _context.Students.FirstOrDefaultAsync(m => m.Id == id);
            if (students == null)
                return NotFound();

            return View(students);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var students = await _context.Students.FindAsync(id);
            if (students != null)
            {
                _context.Students.Remove(students);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Student deleted successfully";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool StudentsExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}





