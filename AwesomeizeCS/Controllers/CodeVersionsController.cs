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
using AwesomeizeCS.Services.Interfaces;
using AwesomeizeCS.Services;

namespace AwesomeizeCS.Controllers
{
    public class CodeVersionsController : Controller
    {
        private readonly ICodeVersionsService _service;

        public CodeVersionsController(ICodeVersionsService service)
        {
            _service = service;
        }

        // GET: CodeVersions
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAllCodeVersionsAsync());
        }

        // GET: CodeVersions/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var codeVersion = await _service.GetCodeVersionByIdAsync(id.Value);
                
            if (codeVersion == null)
            {
                return NotFound();
            }

            return View(codeVersion);
        }

        // GET: CodeVersions/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: CodeVersions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,UploadDate,Location")] CodeVersion codeVersion)
        {
            if (ModelState.IsValid)
            {
                await _service.CreateCodeVersionAsync(codeVersion);
                return RedirectToAction(nameof(Index));
            }
            return View(codeVersion);
        }

        // GET: CodeVersions/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var codeVersion = await _service.GetCodeVersionByIdAsync(id.Value);

            if (codeVersion == null)
            {
                return NotFound();
            }
            return View(codeVersion);
        }

        // POST: CodeVersions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UploadDate,Location")] CodeVersion codeVersion)
        {
            if (id != codeVersion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateCodeVersionAsync(codeVersion);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_service.CodeVersionExists(codeVersion.Id))
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
            return View(codeVersion);
        }

        // GET: CodeVersions/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var codeVersion = await _service.GetCodeVersionByIdAsync(id.Value);
            if (codeVersion == null)
            {
                return NotFound();
            }

            return View(codeVersion);
        }

        // POST: CodeVersions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {


            var codeVersion = await _service.GetCodeVersionByIdAsync(id);
            if (codeVersion != null)
            {
                await _service.DeleteCodeVersionAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
