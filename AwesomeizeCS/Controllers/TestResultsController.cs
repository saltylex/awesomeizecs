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
using AwesomeizeCS.Models;
using AwesomeizeCS.Services.Interfaces;

namespace AwesomeizeCS.Controllers
{
    public class TestResultsController : Controller
    {
        private readonly ITestResultsService _context;

        public TestResultsController(ITestResultsService context)
        {
            _context = context;
        }

        // // GET: TestResults
        // [Authorize(Roles = "Admin")]
        // public async Task<IActionResult> Index()
        // {
        //     return View(await _context.TestResult.ToListAsync());
        // }

        public static ResultsViewModel Map(CodeVersion codeVersion)
        {
            ResultsViewModel test =  new ResultsViewModel
            {
                Exercise = new Assignment
                {
                    Id = codeVersion.CodeFor.Assignment.Id,
                    Order = codeVersion.CodeFor.Assignment.Order,
                    ShortDescription = codeVersion.CodeFor.Assignment.ShortDescription,
                    Content = codeVersion.CodeFor.Assignment.Content,
                    Name = codeVersion.CodeFor.Assignment.Name,
                    VisibleFromWeek = codeVersion.CodeFor.Assignment.VisibleFromWeek,
                    SolvableFromWeek = codeVersion.CodeFor.Assignment.SolvableFromWeek,
                    SolvableToWeek = codeVersion.CodeFor.Assignment.SolvableFromWeek
                    
                },
                TestResults = codeVersion.Results.Where(r => r.Test != null).Select(r => new TestResultViewModel
                {
                    Id = r.Id,
                    Result = r.Result,
                    Output = r.Output,
                    Test = r.Test,
                }).ToList(),
                Errors = codeVersion.Results.Where(r => r.Test == null).Select(r => r.Result).ToList(),
                RunAt = codeVersion.UploadDate
            };

            return test;
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> ShowResults(Guid? id)
        {
            var codeVersions = await _context.ShowResults(id); 



           //var codeVersions2 = _context.CodeVersion.Include(cv => cv.CodeFor.Student).Include(cv => cv.CodeFor.Assignment.ShortDescription).Include(c => c.Results.Select(r => r.Test)).First(c => c.Id == id);
            return View("~/Views/TestResults/Results.cshtml", codeVersions);
        }

        // // GET: TestResults/Details/5
        // [Authorize(Roles = "Admin")]
        // public async Task<IActionResult> Details(Guid? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     var testResult = await _context.TestResult
        //         .FirstOrDefaultAsync(m => m.Id == id);
        //     if (testResult == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     return View(testResult);
        // }
        //
        // // GET: TestResults/Create
        // [Authorize(Roles = "Admin")]
        // public IActionResult Create()
        // {
        //     return View();
        // }
        //
        // // POST: TestResults/Create
        // // To protect from overposting attacks, enable the specific properties you want to bind to.
        // // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // [Authorize(Roles = "Admin")]
        // public async Task<IActionResult> Create([Bind("Id,Result,Output")] TestResult testResult)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         testResult.Id = Guid.NewGuid();
        //         _context.Add(testResult);
        //         await _context.SaveChangesAsync();
        //         return RedirectToAction(nameof(Index));
        //     }
        //     return View(testResult);
        // }
        //
        // // GET: TestResults/Edit/5
        // [Authorize(Roles = "Admin")]
        // public async Task<IActionResult> Edit(Guid? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     var testResult = await _context.TestResult.FindAsync(id);
        //     if (testResult == null)
        //     {
        //         return NotFound();
        //     }
        //     return View(testResult);
        // }
        //
        // // POST: TestResults/Edit/5
        // // To protect from overposting attacks, enable the specific properties you want to bind to.
        // // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // [Authorize(Roles = "Admin")]
        // public async Task<IActionResult> Edit(Guid id, [Bind("Id,Result,Output")] TestResult testResult)
        // {
        //     if (id != testResult.Id)
        //     {
        //         return NotFound();
        //     }
        //
        //     if (ModelState.IsValid)
        //     {
        //         try
        //         {
        //             _context.Update(testResult);
        //             await _context.SaveChangesAsync();
        //         }
        //         catch (DbUpdateConcurrencyException)
        //         {
        //             if (!TestResultExists(testResult.Id))
        //             {
        //                 return NotFound();
        //             }
        //             else
        //             {
        //                 throw;
        //             }
        //         }
        //         return RedirectToAction(nameof(Index));
        //     }
        //     return View(testResult);
        // }
        //
        // // GET: TestResults/Delete/5
        // [Authorize(Roles = "Admin")]
        // public async Task<IActionResult> Delete(Guid? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     var testResult = await _context.TestResult
        //         .FirstOrDefaultAsync(m => m.Id == id);
        //     if (testResult == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     return View(testResult);
        // }
        //
        // // POST: TestResults/Delete/5
        // [HttpPost, ActionName("Delete")]
        // [ValidateAntiForgeryToken]
        // [Authorize(Roles = "Admin")]
        // public async Task<IActionResult> DeleteConfirmed(Guid id)
        // {
        //     var testResult = await _context.TestResult.FindAsync(id);
        //     if (testResult != null)
        //     {
        //         _context.TestResult.Remove(testResult);
        //     }
        //
        //     await _context.SaveChangesAsync();
        //     return RedirectToAction(nameof(Index));
        // }


    }
}
