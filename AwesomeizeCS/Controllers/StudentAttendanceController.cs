using Microsoft.AspNetCore.Mvc;
using AwesomeizeCS.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using AwesomeizeCS.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using AwesomeizeCS.Models;
using AwesomeizeCS.Services;

namespace AwesomeizeCS.Controllers;


public class StudentAttendanceController: Controller
    {
    private readonly IAttendancesService _attendancesService;
    private readonly IStudentAssignmentsService _studentAssignmentsService;

    public StudentAttendanceController(IAttendancesService attendancesService, IStudentAssignmentsService studentAssignmentsService)
    {
        _attendancesService = attendancesService;
        _studentAssignmentsService = studentAssignmentsService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _attendancesService.GetAllAttendances());
        
    }

    // GET: /StudentAttendance/Course/123
    [HttpGet("StudentAttendance/Course/{courseName}")]
    public async Task<IActionResult> ViewAttendance(String courseName)
    {
        var userEmail = User.Identity?.Name ?? "";
        var attendances = await _attendancesService.GetAttendancesForCourse(courseName, userEmail);
        return View(attendances);
    }

    //// GET: /StudentAttendance/Create
    [HttpGet("StudentAttendance/Create")]
    public async Task<IActionResult> CreateAsync()
    {
        var userEmail = User.Identity?.Name ?? "";
        var attendanceData = await _attendancesService.GetAttendanceFormData(userEmail);

        if (attendanceData == null)
        {
            return NotFound("Required data not found.");
        }
        return View(attendanceData);
    }

    // POST: /StudentAttendance/Create
    [HttpPost("StudentAttendance/Create")]
    public async Task<IActionResult> Create([Bind("Id,IsValidated,Time,StudentCourse")] AttendanceViewModel model)
    {
       
            try
            {
                await _attendancesService.CreateAttendance(model);
                return RedirectToAction("Index", "Home");
            }
            catch (ArgumentException ex)
            {
                var errorMessageParts = ex.Message.Split(',');
                var fieldName = errorMessageParts[0].Trim().Split(':')[1].Trim();
                var errorMessage = errorMessageParts[1].Trim().Split(':')[1].Trim();
                ModelState.AddModelError(fieldName, errorMessage);
            }

        return RedirectToAction("Index", "StudentAttendance");
    }

    //GET: /StudentAttendance/AttendanceValidation
    [HttpGet("StudentAttendance/AttendanceValidation")]
    public async Task<IActionResult> AttendanceValidation()
    {
        var attendanceRecords = await _attendancesService.GetAttendanceForValidation();
        var assignmentRecords = await _studentAssignmentsService.GetStudentAssignmentsForGrading();


        return View((attendanceRecords,assignmentRecords));
    }

    [Authorize(Roles = "Admin,Teacher")]
    // POST: /StudentAttendance/AttendanceValidation
    [HttpPost("StudentAttendance/AttendanceValidation")]
    public async Task<IActionResult> AttendanceValidation([Bind("Id,IsValidated")] List<Attendance> attendanceRecords)
    {
        foreach (var record in attendanceRecords)
        {
            await _attendancesService.UpdateAttendanceValidation(record.Id, record.IsValidated);
        }

        return RedirectToAction("Index", "StudentAttendance");
    }

    [Authorize(Roles = "Admin,Teacher")]
    // POST: /StudentAttendance/StudentAssignmentsGrading
    [HttpPost("StudentAttendance/StudentAssignmentsGrading")]
    public async Task<IActionResult> StudentAssignmentsGrades([Bind("Id,Grade,Bonus")] List<StudentAssignment> assignmentRecords)
    {
        foreach (var record in assignmentRecords)
        {
            await _studentAssignmentsService.UpdateStudentAssignmentGrade(record.Id, record.Grade, record.Bonus);
        }

        return RedirectToAction("Index", "StudentAttendance");
    }


}

