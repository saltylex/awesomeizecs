using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AwesomeizeCS.Domain;
using AwesomeizeCS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using AwesomeizeCS.InstantFeedback;
using AwesomeizeCS.Data;

namespace AwesomeizeCS.Controllers
{
    public class StudentAssignmentsController : Controller
    {
        private readonly IStudentAssignmentsService _service;
        private readonly IStudentCoursesService _studentCoursesService;
        private readonly IAssignmentsService _assignmentsService;

        public StudentAssignmentsController(IStudentAssignmentsService service,
            IStudentCoursesService studentCoursesService, IAssignmentsService assignmentsService)
        {
            _service = service;
            _studentCoursesService = studentCoursesService;
            _assignmentsService = assignmentsService;
        }

        [Authorize(Roles = "Admin,Teacher")]
        // GET: StudentAssignments
        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAllStudentAssignments());
        }

        
        // GET: StudentAssignments/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentAssignment = await _service.GetStudentAssignmentOrDefault(id);
            if (studentAssignment == null)
            {
                return NotFound();
            }

            return View(studentAssignment);
        }

        private async Task<string> GetUploadDirectory(Guid id)
        {
            var studentAssignment = await _service.GetStudentAssignmentById(id);
            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads",
                $"{studentAssignment.Assignment.Course.Name}", $"{studentAssignment.Assignment.Type}",
                $"{studentAssignment.Assignment.Name}",
                $"{studentAssignment.Student.Subgroup}",
                $"{ studentAssignment.Student.FirstName}{ studentAssignment.Student.LastName}");
        }

        private async Task<string> GetUploadFilePath(Guid id, string fileName)
        {
            return Path.Combine(await GetUploadDirectory(id), fileName);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> AssignmentDetails(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentAssignment = await _service.GetStudentAssignmentOrDefault(id);
            if (studentAssignment == null || User.Identity.Name != studentAssignment.Student.EmailAddress)
            {
                return NotFound();
            }

            var uploadsDirectory = await GetUploadDirectory(id.Value);
            var files = Directory.Exists(uploadsDirectory)
                ? Directory.GetFiles(uploadsDirectory)
                : Array.Empty<string>();

            ViewBag.Files = files.Select(Path.GetFileName).ToList();
            return View(studentAssignment);
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> DeleteFile(Guid id, string fileName)
        {
            var filePath = await GetUploadFilePath(id, fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return RedirectToAction("AssignmentDetails", new { id = id });
        }

        [Authorize(Roles = "Admin,Teacher")]
// GET: StudentAssignments/Create
        public async Task<IActionResult> CreateAsync()
        {
            var students = await _service.GetAllStudents();
            ViewBag.Students = students
                .OrderBy(s => s.FirstName)
                .ThenBy(s => s.LastName)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.FirstName + " " + s.LastName })
                .ToList();
            var assignments = await _service.GetAllAssignments();
            ViewBag.Assignments = assignments
                .OrderBy(a => a.Name)
                .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name })
                .ToList();
            return View();
        }
        [Authorize(Roles = "Admin,Teacher")]
        // POST: /StudentAssignments/AssignToAll
        [HttpPost("StudentAssignments/AssignToAll")]
        public async Task<IActionResult> AssignToAll([Bind("Id")] Guid Id)
        {
            var assignments = await _service.GetAllStudentAssignments();
            var isAssigned = assignments.Exists(sa => sa.Assignment.Id == Id);
            if (!isAssigned)
            {
                var assignmentUsed = await _assignmentsService.GetAssignmentOrDefault(Id);
                var studentCourses = await _studentCoursesService.GetAllStudentCoursesByCourse(assignmentUsed.Course);
                foreach (var studentCourse in studentCourses)
                {
                    var studentAssignment = new StudentAssignment
                    {
                        Id = Guid.NewGuid(),
                        Student = studentCourse.Student,
                        Assignment = assignmentUsed
                    };
                    await _service.CreateStudentAssignment(studentAssignment);
                }
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Details", "Assignments", new { Id = Id });
        }
        [Authorize(Roles = "Admin,Teacher")]
        // POST: /StudentAssignments/AssignSubproblemRandomlyToAll
        [HttpPost("StudentAssignments/AssignSubproblemRandomlyToAll")]
        public async Task<IActionResult> AssignSubproblemRandomlyToAll([Bind("Id")] Guid Id)
        {
            // get all studentcourses at the course of the assignment
            // get all assignments that have this assignment as a parent.
            // create new studentassignment for all 
            var random = new Random();
            var studentAssignments = await _service.GetAllStudentAssignments();
            var subProblems = await _assignmentsService.GetAllSubproblems(Id);
            var isParentAssigned = studentAssignments.Any(sa => sa.Assignment.Id == Id);
            var isAssigned = studentAssignments.Any(sa=>subProblems.Any(sp=>sp.Id == sa.Assignment.Id));
            if (isParentAssigned && !isAssigned)
            {
                var parent = await _assignmentsService.GetAssignmentOrDefault(Id);
                var studentCourses = await _studentCoursesService.GetAllStudentCoursesByCourse(parent.Course);
                foreach (var studentCourse in studentCourses)
                {
                    // choose a random assignment from the subproblems list
                    var assignmentUsed = subProblems[random.Next(subProblems.Count)];
                    var studentAssignment = new StudentAssignment
                    {
                        Id = Guid.NewGuid(),
                        Student = studentCourse.Student,
                        Assignment = assignmentUsed
                    };
                    await _service.CreateStudentAssignment(studentAssignment);
                }

                return RedirectToAction("Index", "Home", new { Id = "uau" });
            }

            return RedirectToAction("Details", "Assignments", new { Id = Id });
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(Guid id, IFormFile file)
        {
            Guid codeVersionId = Guid.NewGuid();
            var studentAssignment = await _service.GetStudentAssignmentById(id);
            if (file != null && file.Length > 0)
            {
                var uploadsRootFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads",
                    $"{studentAssignment.Assignment.Course.Name}", $"{studentAssignment.Assignment.Type}",
                    $"{studentAssignment.Assignment.Name}",
                    $"{studentAssignment.Student.Subgroup}",
                    $"{studentAssignment.Student.FirstName}{studentAssignment.Student.LastName}",
                    $"CodeVersion{DateTime.Now:yyyyMMddHHmmss}");
                if (!Directory.Exists(uploadsRootFolder))
                {
                    Directory.CreateDirectory(uploadsRootFolder);
                }

                var extension = Path.GetExtension(file.FileName);
                var newFileName =
                    $"{studentAssignment.Student.FirstName}{studentAssignment.Student.LastName}{DateTime.Now:yyyyMMddHHmmss}{extension}";

                var filePath = Path.Combine(uploadsRootFolder, newFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                _service.RunTests(uploadsRootFolder, studentAssignment.Assignment.Id, studentAssignment.Student.Id, codeVersionId);

                return RedirectToAction("ShowResults", "TestResults", new { id = codeVersionId });
                //return RedirectToAction("AssignmentDetails", new { id = id }); // Adjust the redirection as necessary
            }

            return View("Error"); 
        }
        [Authorize(Roles = "Admin,Teacher")]

        // POST: StudentAssignments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Grade,Bonus,Student,Assignment")]
            StudentAssignment studentAssignment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.CreateStudentAssignment(studentAssignment);
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    var errorMessageParts = ex.Message.Split(',');
                    var fieldName = errorMessageParts[0].Trim().Split(':')[1].Trim();
                    var errorMessage = errorMessageParts[1].Trim().Split(':')[1].Trim();

                    ModelState.AddModelError(fieldName, errorMessage);
                    var students = await _service.GetAllStudents();
                    ViewBag.Students = students
                        .OrderBy(s => s.FirstName)
                        .ThenBy(s => s.LastName)
                        .Select(s => new SelectListItem
                            { Value = s.Id.ToString(), Text = s.FirstName + " " + s.LastName })
                        .ToList();
                    var assignments = await _service.GetAllAssignments();
                    ViewBag.Assignments = assignments
                        .OrderBy(a => a.Name)
                        .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name })
                        .ToList();
                    return View(studentAssignment);
                }
            }

            return View(studentAssignment);
        }
        [Authorize(Roles = "Admin,Teacher")]
        // GET: StudentAssignments/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var students = await _service.GetAllStudents();
            ViewBag.Students = students
                .OrderBy(s => s.FirstName)
                .ThenBy(s => s.LastName)
                .Select(s => new SelectListItem
                    { Value = s.Id.ToString(), Text = s.FirstName + " " + s.LastName })
                .ToList();
            var assignments = await _service.GetAllAssignments();
            ViewBag.Assignments = assignments
                .OrderBy(a => a.Name)
                .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name })
                .ToList();
            var studentAssignment = await _service.GetStudentAssignmentById(id);
            if (studentAssignment == null)
            {
                return NotFound();
            }

            return View(studentAssignment);
        }

        // POST: StudentAssignments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Grade,Bonus")] StudentAssignment studentAssignment)
        {
            if (id != studentAssignment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateStudentAssignment(studentAssignment);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_service.StudentAssignmentExists(studentAssignment.Id))
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

            var students = await _service.GetAllStudents();
            ViewBag.Students = students
                .OrderBy(s => s.FirstName)
                .ThenBy(s => s.LastName)
                .Select(s => new SelectListItem
                    { Value = s.Id.ToString(), Text = s.FirstName + " " + s.LastName })
                .ToList();
            var assignments = await _service.GetAllAssignments();
            ViewBag.Assignments = assignments
                .OrderBy(a => a.Name)
                .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name })
                .ToList();
            return View(studentAssignment);
        }

        // GET: StudentAssignments/Delete/5
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentAssignment = await _service.GetStudentAssignmentOrDefault(id);
            if (studentAssignment == null)
            {
                return NotFound();
            }

            return View(studentAssignment);
        }

        // POST: StudentAssignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]

        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var studentAssignment = await _service.GetStudentAssignmentById(id);
            if (studentAssignment != null)
            {
                await _service.DeleteStudentAssignment(studentAssignment);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool StudentAssignmentExists(Guid id)
        {
            return _service.StudentAssignmentExists(id);
        }
    }
}