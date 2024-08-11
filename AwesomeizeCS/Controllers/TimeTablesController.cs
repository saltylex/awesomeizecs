using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AwesomeizeCS.Domain;
using AwesomeizeCS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AwesomeizeCS.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class TimeTablesController : Controller
    {
        private readonly ITimeTablesService _service;

        public TimeTablesController(ITimeTablesService service)
        {
            _service = service;
        }
        
        // GET: TimeTables
        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAllTimeTables());
        }

        public async Task<IActionResult> ClassesOfTheDay()
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var timetables = await _service.GetTimetablesForTeacher(new Guid(teacherId));
            return View(timetables);
        }

        // GET: TimeTables/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeTable = await _service.GetTimeTableOrDefault(id);
            if (timeTable == null)
            {
                return NotFound();
            }

            return View(timeTable);
        }

        // GET: TimeTables/Create
        public async Task<IActionResult> CreateAsync()
        {

            var courses = await _service.GetAllCourses(); 
            ViewBag.Courses = courses
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();

            return View();
        }

        // POST: TimeTables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StartsAt,EndsAt,Type,For,Room,Week,Order,AcademicYear,TaughtBy,SpecificAttendanceTracking,Course")] TimeTable timeTable)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.CreateTimeTable(timeTable);
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    var errorMessageParts = ex.Message.Split(',');
                    var fieldName = errorMessageParts[0].Trim().Split(':')[1].Trim();
                    var errorMessage = errorMessageParts[1].Trim().Split(':')[1].Trim();

                    // Add an error to ModelState for the specific property
                    ModelState.AddModelError(fieldName, errorMessage);
                    var coursesException = await _service.GetAllCourses();
                    ViewBag.Courses = coursesException
                        .OrderBy(c => c.Name)
                        .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                        .ToList();
                    return View(timeTable);
                }
            }
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            var courses = await _service.GetAllCourses();
            ViewBag.Courses = courses
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();
            return View(timeTable);
        }

        // GET: TimeTables/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var courses = await _service.GetAllCourses(); 
            ViewBag.Courses = courses
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();

            var timeTable = await _service.GetTimeTableById(id);
            if (timeTable == null)
            {
                return NotFound();
            }
            return View(timeTable);
        }

        // POST: TimeTables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,StartsAt,EndsAt,For,Room,Week,Order,AcademicYear,TaughtBy,SpecificAttendanceTracking")] TimeTable timeTable)
        {
            if (id != timeTable.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bool isValidYear = int.TryParse(timeTable.AcademicYear, out int year) &&
                                       year >= 2023 &&
                                       year <= 2100;
                    if (!isValidYear)
                    {
                        ModelState.AddModelError(nameof(timeTable.AcademicYear), "Wrong Year.");
                        return View(timeTable);
                    }

                    if (timeTable.EndsAt < timeTable.StartsAt)
                    {
                        ModelState.AddModelError(nameof(timeTable.EndsAt), "End time must be after Start time");
                        return View(timeTable);
                    }
                    await _service.UpdateTimeTable(timeTable);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_service.TimeTableExists(timeTable.Id))
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
            return View(timeTable);
        }

        // GET: TimeTables/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeTable = await _service.GetTimeTableOrDefault(id);
            if (timeTable == null)
            {
                return NotFound();
            }

            return View(timeTable);
        }

        // POST: TimeTables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var timeTable = await _service.GetTimeTableById(id);
            if (timeTable != null)
            {
                await _service.DeleteTimeTable(timeTable);
            }

            return RedirectToAction(nameof(Index));
        }
        

        private bool TimeTableExists(Guid id)
        {
            return _service.TimeTableExists(id);
        }
    }
}
