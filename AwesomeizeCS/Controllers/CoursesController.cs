using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;
using AwesomeizeCS.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using AwesomeizeCS.Services;
using AwesomeizeCS.Models;
using AwesomeizeCS.Utils;
using AwesomeizeCS.Utils.Interfaces;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace AwesomeizeCS.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ICoursesService _service;
        private readonly ITeacherCoursesService _teacherCoursesService;
        private readonly IExcelManager _excelManager;


        public CoursesController(ICoursesService service, IExcelManager excelManager,
            ITeacherCoursesService teacherCoursesService)
        {
            _service = service;
            _teacherCoursesService = teacherCoursesService;
            _excelManager = excelManager;
        }

        [Authorize(Roles = "Admin,Teacher")]
        // GET: Courses
        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAllCourses());
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _service.GetCourseOrDefault(id);
            if (course == null)
            {
                return NotFound();
            }

            var uploadsDirectory = await GetUploadDirectory((Guid)id);
            var files = Directory.Exists(uploadsDirectory)
                ? Directory.GetFiles(uploadsDirectory)
                : Array.Empty<string>();

            ViewBag.Files = files.Select(Path.GetFileName).ToList();
            return View(course);
        }
        [Authorize(Roles = "Admin,Teacher")]
        // GET: Courses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Create(
            [Bind(
                "Id,Name,MainTeacherId,Language,AcademicYear,NumberOfCourses,NumberOfSeminars,NumberOfLabs,DefaultCourseAttendanceTracking,DefaultSeminarAttendanceTracking,DefaultLaboratoryAttendanceTracking")]
            Course course)
        {
            if (ModelState.IsValid)
            {
                if (course.NumberOfSeminars < 0)
                {
                    ModelState.AddModelError(nameof(course.NumberOfSeminars),
                        "Number of seminars must be a positive number.");
                    return View(course);
                }

                if (course.NumberOfLabs < 0)
                {
                    ModelState.AddModelError(nameof(course.NumberOfLabs), "Number of labs must be a positive number.");
                    return View(course);
                }

                if (course.NumberOfCourses < 0)
                {
                    ModelState.AddModelError(nameof(course.NumberOfCourses),
                        "Number of courses must be a positive number.");
                    return View(course);
                }

                bool isValidYear = int.TryParse(course.AcademicYear, out int year) &&
                                   year >= 2023 &&
                                   year <= 2100;
                if (!isValidYear)
                {
                    ModelState.AddModelError(nameof(course.AcademicYear), "Wrong Year.");
                    return View(course);
                }

                await _service.CreateCourse(course);
                return RedirectToAction(nameof(Index));
            }

            return View(course);
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _service.GetCourseById(id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Edit(Guid id,
            [Bind(
                "Id,Name,MainTeacherId,Language,AcademicYear,NumberOfCourses,NumberOfSeminars,NumberOfLabs,DefaultCourseAttendanceTracking,DefaultSeminarAttendanceTracking,DefaultLaboratoryAttendanceTracking")]
            Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (course.NumberOfSeminars < 0)
                    {
                        ModelState.AddModelError(nameof(course.NumberOfSeminars),
                            "Number of seminars must be a positive number.");
                        return View(course);
                    }

                    if (course.NumberOfLabs < 0)
                    {
                        ModelState.AddModelError(nameof(course.NumberOfLabs),
                            "Number of labs must be a positive number.");
                        return View(course);
                    }

                    if (course.NumberOfCourses < 0)
                    {
                        ModelState.AddModelError(nameof(course.NumberOfCourses),
                            "Number of courses must be a positive number.");
                        return View(course);
                    }

                    bool isValidYear = int.TryParse(course.AcademicYear, out int year) &&
                                       year >= 2023 &&
                                       year <= 2100;
                    if (!isValidYear)
                    {
                        ModelState.AddModelError(nameof(course.AcademicYear), "Wrong Year.");
                        return View(course);
                    }

                    await _service.UpdateCourse(course);
                    return RedirectToAction("Index", "Home");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_service.CourseExists(course.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(course);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _service.GetCourseOrDefault(id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var course = await _service.GetCourseById(id);
            if (course != null)
            {
                await _service.DeleteCourse(course);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CourseDetails(Guid id)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            if (teacherId == "")
                return View();
            
            var coursesForTeacher = await _service.GetCoursesByMainTeacher(new Guid(teacherId));
            if (!coursesForTeacher.Any())
            {
                return View();
            }

            var teachersForCourse = new List<List<TeacherCourse>>();
            foreach (var course in coursesForTeacher)
            {
                teachersForCourse.Add(await _teacherCoursesService.GetAllTeachersByCourse(course));
            }

            var teacherNames = new List<List<string?>>();
            foreach (var teacherList in teachersForCourse)
            {
                var temp = new List<string?>();
                foreach (var teacher in teacherList)
                {
                    temp.Add(await _teacherCoursesService.GetTeacherNameById(teacher.TeacherId) ?? "");
                }

                teacherNames.Add(temp);
            }

            var viewModel = new CourseViewModel(coursesForTeacher, teacherNames);
            return View(viewModel);
        }


        //GET: /Course/StudentSituation
        [Authorize(Roles = "Admin,Teacher")]
        [HttpGet("Courses/StudentSituation")]
        public async Task<IActionResult> StudentSituation()
        {
            var studentSituation = await _service.GetStudentSituationData();
            var courses = await _service.GetAllCourses();
            ViewBag.CourseNames = courses.OrderBy(c => c.CourseInfo.Name).Select(c => c.CourseInfo.Name).ToList();


            return View(studentSituation);
        }
        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost("Courses/StudentSituation/GenerateStudentSituation")]
        public async Task<IActionResult> GenerateStudentSituation(string courseName)
        {
            var studentSituation = await _service.GetStudentSituationData();
            await _excelManager.GenerateClassSituationExcelAsync(studentSituation, courseName);
            string filePath = courseName + "_Class_Situation.csv";
            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            string fileName = courseName + "_Class_Situation.csv";
            var csvToDownload = File(fileBytes, "text/csv", fileName);

            System.IO.File.Delete(filePath);
            return csvToDownload;
        }

        private async Task<string> GetUploadDirectory(Guid id)
        {
            var course = await _service.GetCourseById(id);
            var courseName = course.Name.Replace(" ", "");
            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads",
                $"{courseName}");
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> UploadCourseFile(Guid id, IFormFile file)
        {
            var course = await _service.GetCourseById(id);
            if (file != null && file.Length > 0)
            {
                var courseName = course.Name.Replace(" ", "");

                var uploadsRootFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads",
                    $"{courseName}");
                if (!Directory.Exists(uploadsRootFolder))
                {
                    Directory.CreateDirectory(uploadsRootFolder);
                }

                var extension = Path.GetExtension(file.FileName);

                var filePath = Path.Combine(uploadsRootFolder, file.FileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var uploadsDirectory = await GetUploadDirectory(id);
                var files = Directory.Exists(uploadsDirectory)
                    ? Directory.GetFiles(uploadsDirectory)
                    : Array.Empty<string>();

                ViewBag.Files = files.Select(Path.GetFileName).ToList();
                return RedirectToAction("Details", new { id = id }); // Adjust the redirection as necessary
            }

            return View("Error");
            
        }

        public async Task<IActionResult> DownloadFile(Guid id, string fileName)
        {
            var uploadsDirectory = await GetUploadDirectory(id);
            var filePath = Path.Combine(uploadsDirectory, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var contentType = GetContentType(filePath);
            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, contentType, fileName);
        }

        private string GetContentType(string path)
        {
            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(path, out var contentType))
            {
                contentType = "application/octet-stream"; // default type if unknown
            }

            return contentType;
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> DeleteFile(Guid id, string fileName)
        {
            var uploadsDirectory = await GetUploadDirectory(id);
            var filePath = Path.Combine(uploadsDirectory, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            try
            {
                System.IO.File.Delete(filePath);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}