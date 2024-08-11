using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AwesomeizeCS.Data;
using AwesomeizeCS.Domain;
using AwesomeizeCS.Services;
using AwesomeizeCS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AwesomeizeCS.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class TeacherCoursesController : Controller
    {
        private readonly ITeacherCoursesService _service;

        public TeacherCoursesController(ITeacherCoursesService service)
        {
            _service = service;
        }

        // GET: TeacherCourses
        [Route("TeacherCourses")]
        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAllTeacherCourses());
        }

        // GET: TeacherCourses
        [Route("TeacherCourses/{id:guid}")]
        public async Task<IActionResult> Index(Guid id)
        {
            ViewBag.currentCourse = id;
            return View(await _service.GetAllTeacherCourses());
        }

        // GET: TeacherCourses/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacherCourse = await _service.GetTeacherCourseOrDefault(id);
            if (teacherCourse == null)
            {
                return NotFound();
            }

            return View(teacherCourse);
        }

        // GET: TeacherCourses/Create
        public async Task<IActionResult> Create(Guid? id)
        {
            if (id != null)
            {
                var course = await _service.GetCourseById(id);
                var selectedCourse = new List<SelectListItem>();
                selectedCourse.Add(new SelectListItem { Value = course.Id.ToString(), Text = course.Name });
                ViewBag.Courses = selectedCourse;
            }
            else
            {
                var courses = await _service.GetAllCourses();
                ViewBag.Courses = courses
                    .OrderBy(c => c.Name)
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToList();
            }


            var teachers = await _service.GetAllTeachers();
            ViewBag.Teachers = teachers
                .OrderBy(c => c.UserName)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.UserName })
                .ToList();
            return View();
        }

        // POST: TeacherCourses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TeacherId,Course,IsMainTeacher")] TeacherCourse teacherCourse)
        {
            if (ModelState.IsValid)
            {  var teachers = await _service.GetAllTeachers();
                ViewBag.Teachers = teachers
                    .OrderBy(c => c.UserName)
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.UserName })
                    .ToList();
                // check if it is already added
                var teacherCourses = await _service.GetAllTeachersByCourse(teacherCourse.Course);
                foreach (var teacher in teacherCourses)
                {
                    if (teacher.TeacherId == teacherCourse.TeacherId)
                    {
                        ModelState.AddModelError(nameof(teacherCourse.TeacherId),
                            "Teacher already enrolled at course!");
                        var course = await _service.GetCourseById(teacherCourse.Course.Id);
                        var selectedCourse = new List<SelectListItem>();
                        selectedCourse.Add(new SelectListItem { Value = course.Id.ToString(), Text = course.Name });
                        ViewBag.Courses = selectedCourse;
                        
                        return View(teacherCourse);
                    }
                }
                await _service.CreateTeacherCourse(teacherCourse);
                // EDIT HERE TO REDIRECT TO COURSEDETAILS RAHHHHHH
                return RedirectToAction(nameof(Index));
            }

            var courses = await _service.GetAllCourses();
            ViewBag.Courses = courses
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();
            return View(teacherCourse);
        }

        // GET: TeacherCourses/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacherCourse = await _service.GetTeacherCourseOrDefault(id);
            if (teacherCourse == null)
            {
                return NotFound();
            }

            var courses = await _service.GetAllCourses();

            var teachers = await _service.GetAllTeachers();
            ViewBag.Courses = courses
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();
            ViewBag.Teachers = teachers
                .OrderBy(c => c.UserName)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.UserName })
                .ToList();
            return View(teacherCourse);
        }

        // POST: TeacherCourses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id,
            [Bind("Id,TeacherId,Course,IsMainTeacher")]
            TeacherCourse teacherCourse)
        {
            if (id != teacherCourse.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateTeacherCourse(teacherCourse);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_service.TeacherCourseExists(teacherCourse.Id))
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

            var courses = await _service.GetAllCourses();
            var teachers = await _service.GetAllTeachers();
            ViewBag.Courses = courses
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();
            ViewBag.Teachers = teachers
                .OrderBy(c => c.UserName)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.UserName })
                .ToList();
            return View(teacherCourse);
        }

        // GET: TeacherCourses/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacherCourse = await _service.GetTeacherCourseOrDefault(id);
            if (teacherCourse == null)
            {
                return NotFound();
            }

            return View(teacherCourse);
        }

        // POST: TeacherCourses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var teacherCourse = await _service.GetTeacherCourseOrDefault(id);
            if (teacherCourse != null)
            {
                await _service.DeleteTeacherCourse(teacherCourse);
            }

            return RedirectToAction(nameof(Index));
        }
        // controller where a teacher can see all the teachers at the courses they teach
    }
}