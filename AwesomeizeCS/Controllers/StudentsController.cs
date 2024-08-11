using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AwesomeizeCS.Domain;
using AwesomeizeCS.Services;
using AwesomeizeCS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace AwesomeizeCS.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class StudentsController : Controller
    {
        private readonly IStudentsService _service;

        public StudentsController(IStudentsService service)
        {
            _service = service;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAllStudents());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _service.GetStudentOrDefault(id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,EmailAddress,Subgroup")] Student student)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.CreateStudent(student);
                }

                catch (ArgumentException ex)
                {
                    var errorMessageParts = ex.Message.Split(',');
                    var fieldName = errorMessageParts[0].Trim().Split(':')[1].Trim();
                    var errorMessage = errorMessageParts[1].Trim().Split(':')[1].Trim();


                    ModelState.AddModelError(fieldName, errorMessage);
                    return View(student);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _service.GetStudentById(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,FirstName,LastName,EmailAddress,Subgroup")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    await _service.UpdateStudent(student);
                }
                catch (ArgumentException ex)
                {
                    var errorMessageParts = ex.Message.Split(',');
                    var fieldName = errorMessageParts[0].Trim().Split(':')[1].Trim();
                    var errorMessage = errorMessageParts[1].Trim().Split(':')[1].Trim();

                    
                    ModelState.AddModelError(fieldName, errorMessage);
                    return View(student);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_service.StudentExists(student.Id))
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
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _service.GetStudentOrDefault(id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var student = await _service.GetStudentById(id);
            if (student != null)
            {
                await _service.DeleteStudent(student);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(Guid id)
        {
            return _service.StudentExists(id);
        }
    }
}
