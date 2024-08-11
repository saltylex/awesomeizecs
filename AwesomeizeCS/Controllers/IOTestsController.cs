using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AwesomeizeCS.Data;
using AwesomeizeCS.Domain;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Serilog.Parsing;
using AwesomeizeCS.Services.Interfaces;

namespace AwesomeizeCS.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class IOTestsController : Controller
    {
        private readonly IIOTestsService _context;

        public IOTestsController(IIOTestsService context)
        {
            _context = context;
        }

        // GET: IOTests
        public async Task<IActionResult> Index()
        {
            return View(await _context.GetAllTestsAsync());
        }

        // GET: IOTests/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var iOTest = await _context.GetTestByIdWithStepsAsync((Guid)id);
            if (iOTest == null)
            {
                return NotFound();
            }

            return View(iOTest);
        }

        // GET: IOTests/Create
        public IActionResult Create(Guid? id = null)
        {
            ViewBag.AssignmentId = id;
            return View();
        }

        // POST: IOTests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid? assignmentId, IOTest iOTest)
        {
            {
                iOTest.Id = Guid.NewGuid();
                await _context.CreateTestAsync(assignmentId, iOTest);
                return RedirectToAction("Details", "IOTests", new { id = iOTest.Id }); //change this accordingly

            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateStep(Guid testId, TestStep step)
        {
            if (step.ExpectedOutput == null)
                step.ExpectedOutput = "";
            {
                await _context.AddStepToTestAsync(testId, step);
                return RedirectToAction("Details", "IOTests", new { id = testId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> TextToStepView(Guid testId, string testStepFromText)
        {
            if (ModelState.IsValid)
            {
                await _context.TextToStepView(testId, testStepFromText);
                return RedirectToAction("Details", "IOTests", new { id = testId });
            }

            return View();
        }

        // GET: IOTests/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var iOTest = await _context.GetTestByIdAsync((Guid)id);
            if (iOTest == null)
            {
                return NotFound();
            }
            return View(iOTest);
        }

        // POST: IOTests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Priority,Hint")] IOTest iOTest)
        {
            if (id != iOTest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.UpdateTestAsync(iOTest);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IOTestExists(iOTest.Id))
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
            return View(iOTest);
        }

        // GET: IOTests/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var iOTest = await _context.GetTestByIdAsync((Guid)id);
            if (iOTest == null)
            {
                return NotFound();
            }

            return View(iOTest);
        }

        // POST: IOTests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _context.DeleteTestAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStep(Guid? id, Guid? testId)
        {
            if (id == null || testId == null)
            {
                return NotFound();
            }
            await _context.DeleteStepFromTestAsync((Guid)id, (Guid)testId);
            return RedirectToAction("Details", "IOTests", new { id = testId });
        }

        public async Task<IActionResult> MoveUpStep(Guid? id, Guid? testId)
        {
            if (id == null || testId == null)
            {
                return NotFound();
            }
            await _context.MoveStepUpAsync((Guid)id, (Guid)testId);

            return RedirectToAction("Details", "IOTests", new { id = testId });
        }

        public async Task<IActionResult> MoveDownStep(Guid? id, Guid? testId)
        {
            if (id == null || testId == null)
            {
                return NotFound();
            }
            await _context.MoveStepDownAsync((Guid)id, (Guid)testId);

            return RedirectToAction("Details", "IOTests", new { id = testId });
        }

        private bool IOTestExists(Guid id)
        {
            return _context.IOTestExists(id);
        }
    }
}
