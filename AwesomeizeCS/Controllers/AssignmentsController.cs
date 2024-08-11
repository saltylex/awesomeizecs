using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;
using AwesomeizeCS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AwesomeizeCS.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class AssignmentsController : Controller
    {
        private readonly IAssignmentsService _service;

        public AssignmentsController(IAssignmentsService service)
        {
            _service = service;
        }

        // GET: Assignments
        [Route("Assignments")]
        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAllAssignments());
        }


        // GET: Assignments/5
        [Route("Assignments/{id:guid}")]
        public async Task<IActionResult> Index(Guid id)
        {
            ViewBag.currentCourse = id;
            return View(await _service.GetAllAssignments());
        }

        // GET: Assignments/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _service.GetAssignmentOrDefault(id);
            if (assignment == null)
            {
                return NotFound();
            }

            var children = await _service.GetChildrenAssignments(assignment);

            var viewModel = new AssignmentDetailViewModel { Assignment = assignment, Children = children };
            return View(viewModel);
        }

        // GET: Assignments/Create
        public async Task<IActionResult> CreateAsync()
        {
            var courses = await _service.GetAllCourses() ?? new List<Course>();;
            var parents = await _service.GetAllAssignments() ?? new List<Assignment>();;
            ViewBag.Courses = courses
                .OrderBy(s => s.Name)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
                .ToList();
            ViewBag.Parents = parents
                .OrderBy(s => s.Name)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
                .ToList();
            
            return View();
        }

        // POST: Assignments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
                "Id,Name,Order,ShortDescription,Content,VisibleFromWeek,SolvableFromWeek,SolvableToWeek,Type,HasGrade,Bonus,Course,Parent,PercentageOutOfTotal")]
            Assignment assignment)
        {
            var courses = await _service.GetAllCourses() ?? new List<Course>();;
            var parents = await _service.GetAllAssignments() ?? new List<Assignment>();;
            ViewBag.Courses = courses
                .OrderBy(s => s.Name)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
                .ToList();
            ViewBag.Parents = parents
                .OrderBy(s => s.Name)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
                .ToList();

            if (ModelState.IsValid)
            {
                if (assignment.VisibleFromWeek < 0)
                {
                    ModelState.AddModelError(nameof(assignment.VisibleFromWeek),
                        "The Value must be a positive number.");
                    return View(assignment);
                }

                if (assignment.SolvableFromWeek < 0)
                {
                    ModelState.AddModelError(nameof(assignment.SolvableFromWeek),
                        "The Value must be a positive number.");
                    return View(assignment);
                }

                if (assignment.SolvableToWeek < 0)
                {
                    ModelState.AddModelError(nameof(assignment.SolvableToWeek),
                        "The Value must be a positive number.");
                    return View(assignment);
                }

                if (assignment.SolvableFromWeek < assignment.VisibleFromWeek)
                {
                    ModelState.AddModelError(nameof(assignment.SolvableFromWeek),
                        "The week must be after or equal to 'Visible from week'.");
                    return View(assignment);
                }

                if (assignment.SolvableToWeek < assignment.SolvableFromWeek)
                {
                    ModelState.AddModelError(nameof(assignment.SolvableToWeek),
                        "The week must be after or equal to 'Solvable from week'.");
                    return View(assignment);
                }

                // if it's ungraded it also means that we shouldn't count it.
                if (!assignment.HasGrade)
                {
                    assignment.PercentageOutOfTotal = 0;
                }

                await _service.CreateAssignment(assignment);

                return RedirectToAction("Details", "Assignments", new {id = assignment.Id});
            }
            return View(assignment);
        }

        // GET: Assignments/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _service.GetAssignmentById(id);
            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        // POST: Assignments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id,
            [Bind(
                "Id,Name,Order,ShortDescription,Content,VisibleFromWeek,SolvableFromWeek,SolvableToWeek,Type,HasGrade,Bonus,PercentageOutOfTotal")]
            Assignment assignment)
        {
            if (id != assignment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (assignment.VisibleFromWeek < 0)
                    {
                        ModelState.AddModelError(nameof(assignment.VisibleFromWeek),
                            "The Value must be a positive number.");
                        return View(assignment);
                    }

                    if (assignment.SolvableFromWeek < assignment.VisibleFromWeek)
                    {
                        ModelState.AddModelError(nameof(assignment.SolvableFromWeek),
                            "The week must be after 'Visible from week'.");
                        return View(assignment);
                    }

                    if (assignment.SolvableToWeek < assignment.SolvableFromWeek)
                    {
                        ModelState.AddModelError(nameof(assignment.SolvableToWeek),
                            "The week must be after or equal to 'Solvable from week'.");
                        return View(assignment);
                    }

                    await _service.UpdateAssignment(assignment);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_service.AssignmentExists(assignment.Id))
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

            return View(assignment);
        }

        // GET: Assignments/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _service.GetAssignmentOrDefault(id);
            if (assignment == null)
            {
                return NotFound();
            }
            return View(assignment);
        }

        // POST: Assignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var assignment = await _service.GetAssignmentById(id);
            if (assignment != null)
            {
                await _service.DeleteAssignment(assignment);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}