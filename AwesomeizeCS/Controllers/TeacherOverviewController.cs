using System.Security.Claims;
using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;
using AwesomeizeCS.Services;
using AwesomeizeCS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AwesomeizeCS.Controllers;

[Authorize(Roles = "Admin,Teacher")]
public class TeacherOverviewController : Controller
{
    private readonly IStudentCoursesService _studentCoursesService;
    private readonly IStudentAssignmentsService _studentAssignmentsService;
    private readonly ITimeTablesService _timeTablesService;
    private readonly IAttendancesService _attendancesService;

    public TeacherOverviewController(IStudentCoursesService studentCoursesService, ITimeTablesService timeTablesService,
        IAttendancesService attendancesService, IStudentAssignmentsService studentAssignmentsService)
    {
        _studentCoursesService = studentCoursesService;
        _timeTablesService = timeTablesService;
        _attendancesService = attendancesService;
        _studentAssignmentsService = studentAssignmentsService;
    }

    public async Task<IActionResult> Index()
    {
        // get current logged in teacher
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // current activity for that teacher
        var currentActivity = await _timeTablesService.GetCurrentActivityTimeTable(DateTime.Now, teacherId) ?? null;
        if (currentActivity == null) return View();

        // get all the students from that attending subgroup, AND 
        // all their studentassignments that do not have a grade
        // THAT ARE PRESENT
        var currentStudents = await _studentCoursesService.GetStudentCoursesByActivity(currentActivity);

        // get all the attendances that the students sent through
        var currentAttendances =
            await _attendancesService.GetAttendanceForCurrentActivityForValidation(currentActivity);

        if (currentAttendances.Count == 0)
        {
            return View(new List<TeacherOverviewViewModel>());
        }

        var viewModel = currentStudents.Select(studentCourse => new TeacherOverviewViewModel
        {
            Attendance = currentAttendances.Find(a => a.StudentCourse == studentCourse),
            Time = currentActivity,
            // show current assignment first
            Assignments =
                studentCourse.Student.Assignments.Where(a=>a.Assignment.Course == studentCourse.Course).OrderByDescending(a => a.Assignment.SolvableToWeek).ToList(),
            CourseName = currentActivity.Type + " " + currentActivity.Course?.Name ?? "Software Engineering",
            StudentCourse = studentCourse,
            StudentName = studentCourse.Student.FirstName + " " + studentCourse.Student.LastName,
        });
        return View(viewModel);
    }

    [HttpPost("TeacherOverview/AttendanceValidation")]
    public async Task<IActionResult> AttendanceValidation([Bind("Id,IsValidated")] List<Attendance> attendanceRecords)
    {
        foreach (var record in attendanceRecords)
        {
            await _attendancesService.UpdateAttendanceValidation(record.Id, record.IsValidated);
        }

        ViewBag.Message = "Attendances Validated!";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [Route("/api/update-assignment")]
    public async Task<IActionResult> UpdateAssignment([FromBody] AssignmentGradeViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                if (model.Field.Equals("Grade"))
                {
                    await _studentAssignmentsService.UpdateStudentAssignmentGrade(model.AssignmentId, model.NewValue);
                }
                else
                {
                    await _studentAssignmentsService.UpdateStudentAssignmentBonus(model.AssignmentId, model.NewValue);
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
}