using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;
using AwesomeizeCS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AwesomeizeCS.Utils.Interfaces;
using AwesomeizeCS.Utils;
using Microsoft.AspNetCore.Authorization;
using TechnologySandbox.Data;


namespace AwesomeizeCS.Controllers
{
    public class StudentCoursesController : Controller
    {
        private readonly IStudentCoursesService _service;
        private readonly IStudentAssignmentsService _studentAssignmentsService;
        private readonly IStudentsService _studentsService;
        private readonly IExcelManager _excelManager;

        public StudentCoursesController(IStudentCoursesService service,
            IStudentAssignmentsService studentAssignmentsService, IExcelManager excelManager,
            IStudentsService studentsService)
        {
            _service = service;
            _excelManager = excelManager;
            _studentsService = studentsService;
            _studentAssignmentsService = studentAssignmentsService;
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpGet]
        public async Task<IActionResult> StudentDetail(Guid studentId)
        {
            var student = await _service.GetStudentCourseById(studentId);
            // teme lab, teme seminar, teme curs, medie so far
            var viewModel = new StudentDetailsViewModel
            {
                StudentCourse = student,
                StudentName = student.Student.FirstName + " " + student.Student.LastName,
                CourseAssignments = student.Student.Assignments.Where(a =>
                        a.Assignment.Type == InstructionType.Course && a.Assignment.Course == student.Course)
                    .OrderBy(a => a.Assignment.Order).ToList(),
                SeminarAssignments = student.Student.Assignments
                    .Where(a => a.Assignment.Type == InstructionType.Seminar && a.Assignment.Course == student.Course)
                    .OrderBy(a => a.Assignment.Order).ToList(),
                LaboratoryAssignments = student.Student.Assignments
                    .Where(a => a.Assignment.Type == InstructionType.Laboratory &&
                                a.Assignment.Course == student.Course).OrderBy(a => a.Assignment.Order).ToList(),
                AssignmentAmount = student.Student.Assignments.Count(),
                CourseAttendances = student.Attendances.Count(a => a.Time.Type == InstructionType.Course),
                SeminarAttendances = student.Attendances.Count(a => a.Time.Type == InstructionType.Seminar),
                LaboratoryAttendances = student.Attendances.Count(a => a.Time.Type == InstructionType.Laboratory),
                GradeSoFar = student.Student.Assignments.Where(a => a.Assignment.Course == student.Course)
                    // add together all of the assignments' values
                    .Sum(a => a.Grade == null ? 0 :
                        // if grade ends up bigger than 10 default it to 10, check null bonus
                        a.Bonus == null ? a.Grade.Value > 10
                            ? 10 * a.Assignment.PercentageOutOfTotal.Value / 100
                            : a.Grade.Value * a.Assignment.PercentageOutOfTotal.Value / 100 :
                        (a.Grade.Value + a.Bonus.Value > 10) ? 10 * a.Assignment.PercentageOutOfTotal.Value / 100 :
                        (a.Grade.Value + a.Bonus.Value) * a.Assignment.PercentageOutOfTotal.Value / 100)
            };
            return View(viewModel);
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        [Route("/api/update-assignment-detail")]
        public async Task<IActionResult> UpdateAssignment([FromBody] AssignmentGradeViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Field.Equals("Grade"))
                    {
                        await _studentAssignmentsService.UpdateStudentAssignmentGrade(model.AssignmentId,
                            model.NewValue);
                    }
                    else
                    {
                        await _studentAssignmentsService.UpdateStudentAssignmentBonus(model.AssignmentId,
                            model.NewValue);
                    }
                }
                catch (Exception ex)
                {
                    var errorMessageParts = ex.Message.Split(',');
                    var fieldName = errorMessageParts[0].Trim().Split(':')[1].Trim();
                    var errorMessage = errorMessageParts[1].Trim().Split(':')[1].Trim();
                    ModelState.AddModelError(fieldName, errorMessage);
                    return NotFound();
                }

                return Ok();
            }

            return NotFound();
        }

        [Authorize(Roles = "Admin,Teacher")]
        // GET: StudentCourses
        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAllStudentCourses());
        }

        [Authorize(Roles = "Admin,Teacher")]
        // GET: StudentCourses/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentCourse = await _service.GetStudentCourseById(id);
            if (studentCourse == null)
            {
                return NotFound();
            }

            return View(studentCourse);
        }

        [Authorize(Roles = "Admin,Teacher")]
        // GET: StudentCourses/Create
        public async Task<IActionResult> CreateAsync()
        {
            var students = await _service.GetAllStudents();
            var courses = await _service.GetAllCourses();

            ViewBag.Students = students
                .OrderBy(s => s.FirstName)
                .ThenBy(s => s.LastName)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.FirstName + " " + s.LastName })
                .ToList();
            ViewBag.Courses = courses
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();
            return View();
        }

        [Authorize(Roles = "Admin,Teacher")]
        // POST: StudentCourses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AttendingGroup,Student,Course")] StudentCourse studentCourse)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.CreateStudentCourse(studentCourse);
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    var errorMessageParts = ex.Message.Split(',');
                    var fieldName = errorMessageParts[0].Trim().Split(':')[1].Trim();
                    var errorMessage = errorMessageParts[1].Trim().Split(':')[1].Trim();

                    ModelState.AddModelError(fieldName, errorMessage);
                    var students = await _service.GetAllStudents();
                    var courses = await _service.GetAllCourses();

                    ViewBag.Students = students
                        .OrderBy(s => s.FirstName)
                        .ThenBy(s => s.LastName)
                        .Select(s => new SelectListItem
                            { Value = s.Id.ToString(), Text = s.FirstName + " " + s.LastName })
                        .ToList();
                    ViewBag.Courses = courses
                        .OrderBy(c => c.Name)
                        .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                        .ToList();

                    return View(studentCourse);
                }
            }

            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            return View(studentCourse);
        }

        [Authorize(Roles = "Admin,Teacher")]
        // GET: StudentCourses/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentCourse = await _service.GetStudentCourseById(id);
            if (studentCourse == null)
            {
                return NotFound();
            }

            return View(studentCourse);
        }

        [Authorize(Roles = "Admin,Teacher")]
        // POST: StudentCourses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,AttendingGroup")] StudentCourse studentCourse)
        {
            if (id != studentCourse.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateStudentCourse(studentCourse);
                }
                catch (ArgumentException ex)
                {
                    var errorMessageParts = ex.Message.Split(',');
                    var fieldName = errorMessageParts[0].Trim().Split(':')[1].Trim();
                    var errorMessage = errorMessageParts[1].Trim().Split(':')[1].Trim();
                    Console.WriteLine(errorMessage);

                    ModelState.AddModelError(fieldName, errorMessage);

                    return View(studentCourse);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_service.StudentCourseExists(studentCourse.Id))
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

            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            return View(studentCourse);
        }

        [Authorize(Roles = "Admin,Teacher")]
        // GET: StudentCourses/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentCourse = await _service.GetStudentCourseOrDefault(id);
            if (studentCourse == null)
            {
                return NotFound();
            }

            return View(studentCourse);
        }

        [Authorize(Roles = "Admin,Teacher")]
        // POST: StudentCourses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var studentCourse = await _service.GetStudentCourseById(id);
            if (studentCourse != null)
            {
                await _service.DeleteStudentCourse(studentCourse);
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("StudentCourse/addData")]
        [ValidateAntiForgeryToken]
        private Task<string> UploadFile()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "StudentCourseMaker");
            return Task.FromResult(path);
        }


        // GET: StudentCourses/Upload
        [Authorize(Roles = "Admin")]
        public IActionResult Upload()
        {
            var uploadDirectory = UploadFile().Result;
            ViewBag.UploadDirectory = uploadDirectory;
            return View();
        }

        // POST: StudentCourses/Upload
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, string uploadDirectory)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("File", "Please select a file to upload.");
                return View();
            }

            if (string.IsNullOrEmpty(uploadDirectory))
            {
                ModelState.AddModelError(string.Empty, "Upload directory not found.");
                return View();
            }

            try
            {
                if (!Directory.Exists(uploadDirectory))
                {
                    Directory.CreateDirectory(uploadDirectory);
                }

                var filePath = Path.Combine(uploadDirectory, file.FileName);


                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var studentCourseData = await _excelManager.GenereateListOfStudentCourses(filePath);
                await _studentsService.CreateMissingStudents(studentCourseData);
                await _service.CreateStudentCourse(studentCourseData);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while saving the file: " + ex.Message);
                return View();
            }
        }


        private bool StudentCourseExists(Guid id)
        {
            return _service.StudentCourseExists(id);
        }
    }
}