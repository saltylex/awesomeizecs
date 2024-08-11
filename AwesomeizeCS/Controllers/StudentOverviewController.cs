using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;
using Microsoft.AspNetCore.Mvc;
using AwesomeizeCS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using TechnologySandbox.Data;

namespace AwesomeizeCS.Controllers;

[Authorize(Roles = "Student")]
public class StudentOverviewController : Controller
{
    private readonly IStudentCoursesService _service;


    public StudentOverviewController(IStudentCoursesService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var userEmail = User.Identity?.Name ?? "";
        var studentCourses = await _service.GetStudentCourseAndDetailsByEmail(userEmail);


        var viewModel = studentCourses.Select(studentCourse => new StudentOverviewViewModel
        {
            CourseName = studentCourse.Course.Name,
            CourseId = studentCourse.Course.Id,
            AcademicYear = studentCourse.Course.AcademicYear,
            CourseAssignments = studentCourse.Student.Assignments == null
                ? new List<AssignmentOverviewViewModel>()
                : studentCourse.Student.Assignments
                    .Where(a => a.Assignment.Type == InstructionType.Course)
                    .Where(a => a.Assignment.Course == studentCourse.Course)
                    .Select(a => CreateAssignmentOverview(a, userEmail))
                    .ToList(),
            LaboratoryAssignments = studentCourse.Student.Assignments == null
                ? new List<AssignmentOverviewViewModel>()
                : studentCourse.Student.Assignments
                    .Where(a => a.Assignment.Type == InstructionType.Laboratory).OrderBy(a => a.Assignment.Order)
                    .Where(a => a.Assignment.Course == studentCourse.Course)
                    .Select(a => CreateAssignmentOverview(a, userEmail))
                    .ToList(),
            SeminarAssignments = studentCourse.Student.Assignments == null
                ? new List<AssignmentOverviewViewModel>()
                : studentCourse.Student.Assignments
                    .Where(a => a.Assignment.Type == InstructionType.Seminar)
                    .Where(a => a.Assignment.Course == studentCourse.Course)
                    .Select(a => CreateAssignmentOverview(a, userEmail))
                    .ToList(),
            CourseAttendances = studentCourse.Attendances?.Where(a => a.IsValidated)
                .Count(a => a.Time.Type == InstructionType.Course) ?? 0,
            LaboratoryAttendances = studentCourse.Attendances?.Where(a => a.IsValidated)
                .Count(a => a.Time.Type == InstructionType.Laboratory) ?? 0,
            SeminarAttendances = studentCourse.Attendances?.Where(a => a.IsValidated)
                .Count(a => a.Time.Type == InstructionType.Seminar) ?? 0,
            GradeSoFar = studentCourse.Student.Assignments == null
                ? 0
                : studentCourse.Student.Assignments.Where(a => a.Assignment.Course == studentCourse.Course)
                    // add together all of the assignments' values
                    .Sum(a => a.Grade == null ? 0 :
                        // if grade ends up bigger than 10 default it to 10, check null bonus
                        a.Bonus == null ? a.Grade.Value > 10
                            ? 10 * a.Assignment.PercentageOutOfTotal.Value / 100
                            : a.Grade.Value * a.Assignment.PercentageOutOfTotal.Value / 100 :
                        (a.Grade.Value + a.Bonus.Value > 10) ? 10 * a.Assignment.PercentageOutOfTotal.Value / 100 :
                        (a.Grade.Value + a.Bonus.Value) * a.Assignment.PercentageOutOfTotal.Value / 100)
        }).ToList().OrderBy(studentCourse => studentCourse.CourseName);

        return View(viewModel);
    }

    private AssignmentOverviewViewModel CreateAssignmentOverview(StudentAssignment assignment, string userEmail)
    {
        return new AssignmentOverviewViewModel
        {
            Id = assignment.Id,
            Name = assignment.Assignment.Name,
            Grade = assignment.Grade,
            Bonus = assignment.Bonus
        };
    }
}