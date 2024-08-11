using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AwesomeizeCS.Domain;
using AwesomeizeCS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AwesomeizeCS.Controllers
{
    public class AttendancesController : Controller
    {
        private readonly IAttendancesService _service;


        public AttendancesController(IAttendancesService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin,Teacher")]
        // GET: Attendances
        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAllAttendances());
        }

        [Authorize(Roles = "Admin,Teacher")]
        // GET: Attendances/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _service.GetAttendanceOrDefault(id);
            if (attendance == null)
            {
                return NotFound();
            }

            return View(attendance);
        }

        [Authorize(Roles = "Admin,Teacher")]
        // GET: Attendances/Create
        public async Task<IActionResult> CreateAsync()
        {
            var studentCourse = await _service.GetAllStudentCourses();
            ViewBag.StudentCourses = studentCourse
        .OrderBy(s => s.AttendingGroup)
        .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Course.Name + " " + s.Student.FirstName + " " + s.Student.LastName })
        .ToList();
            var time = await _service.GetAllTimeTables();
            ViewBag.TimeTable = time
        .OrderBy(s => s.StartsAt)
        .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.StartsAt.ToString() })
        .ToList();
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            return View();
        }

        [Authorize(Roles = "Admin,Teacher")]
        // POST: Attendances/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IsValidated,Time,StudentCourse")] Attendance attendance)
        {
            //if (ModelState.IsValid)
            {
                try
                {
                    await _service.CreateAttendance(attendance);
                    return RedirectToAction(nameof(Index));
                }
                catch(ArgumentException ex)
                {
                    var errorMessageParts = ex.Message.Split(',');
                    var fieldName = errorMessageParts[0].Trim().Split(':')[1].Trim();
                    var errorMessage = errorMessageParts[1].Trim().Split(':')[1].Trim();

                    // Add an error to ModelState for the specific property
                    ModelState.AddModelError(fieldName, errorMessage);
                    var studentCourse = await _service.GetAllStudentCourses();
                    ViewBag.StudentCourses = studentCourse
                .OrderBy(s => s.AttendingGroup) 
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.AttendingGroup })
                .ToList();
                    var time = await _service.GetAllTimeTables();
                    ViewBag.TimeTable = time
                .OrderBy(s => s.StartsAt)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.For })
                .ToList();
                    return View(attendance);
                }
            }
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            return View(attendance);


        }

        [Authorize(Roles = "Admin,Teacher")]
        // GET: Attendances/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            var studentCourse = await _service.GetAllStudentCourses();
            ViewBag.StudentCourses = studentCourse
                .OrderBy(s => s.AttendingGroup)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Course.Name + " " + s.Student.FirstName + " " + s.Student.LastName })
                .ToList();
            var time = await _service.GetAllTimeTables();
            ViewBag.TimeTable = time
                .OrderBy(s => s.StartsAt)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.StartsAt.ToString() })
                .ToList();
            
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _service.GetAttendanceById(id);
            if (attendance == null)
            {
                return NotFound();
            }

            return View(attendance);
        }

        [Authorize(Roles = "Admin,Teacher")]
        // POST: Attendances/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,IsValidated")] Attendance attendance)
        {
            var studentCourse = await _service.GetAllStudentCourses();
            ViewBag.StudentCourses = studentCourse
                .OrderBy(s => s.AttendingGroup)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Course.Name + " " + s.Student.FirstName + " " + s.Student.LastName })
                .ToList();
            var time = await _service.GetAllTimeTables();
            ViewBag.TimeTable = time
                .OrderBy(s => s.StartsAt)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.StartsAt.ToString() })
                .ToList();
            if (id != attendance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateAttendance(attendance);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_service.AttendanceExists(attendance.Id))
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

            return View(attendance);
        }

        [Authorize(Roles = "Admin,Teacher")]
        // GET: Attendances/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _service.GetAttendanceOrDefault(id);
            if (attendance == null)
            {
                return NotFound();
            }

            return View(attendance);
        }

        [Authorize(Roles = "Admin,Teacher")]
        // POST: Attendances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var attendance = await _service.GetAttendanceById(id);
            if (attendance != null)
            {
                await _service.DeleteAttendance(attendance);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}