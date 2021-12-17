using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuickQuestions.Data;
using QuickQuestions.Models;

namespace QuickQuestions.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class FilesController : Controller
    {
        private readonly QuickQuestionsContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public FilesController(QuickQuestionsContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        // GET: Admin/Files
        public async Task<IActionResult> Index()
        {
            return View(await _context.File.ToListAsync());
        }

        // GET: Admin/Files/Create
        public IActionResult Create()
        {
            return View();
        }

        // GET: Admin/Files/View/5
        public async Task<IActionResult> View(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var file = await _context.File
                .FirstOrDefaultAsync(m => m.ID == id);

            if (file == null)
            {
                return NotFound();
            }

            return LocalRedirect($"~/files/{file.ID}{file.FileExtension}"); //File(new FileStream(_appEnvironment.WebRootPath + $"/files/{id}", FileMode.Open), file.ContentType);
        }

        // POST: Admin/Files/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                QuickQuestions.Models.File file = new QuickQuestions.Models.File()
                {
                    ID = Guid.NewGuid(),
                    FileName = uploadedFile.FileName,
                    ContentType = uploadedFile.ContentType,
                    FileExtension = Path.GetExtension(uploadedFile.FileName)
                };

                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + $"/files/{file.ID}{file.FileExtension}", FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                _context.Add(file);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: Admin/Files/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var file = await _context.File
                .FirstOrDefaultAsync(m => m.ID == id);

            if (file == null)
            {
                return NotFound();
            }

            return View(file);
        }

        // POST: Admin/Files/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var file = await _context.File.FindAsync(id);

            if(file != null)
            {
                System.IO.File.Delete(_appEnvironment.WebRootPath + $"/files/{file.ID}{file.FileExtension}");

                _context.File.Remove(file);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool FileExists(Guid id)
        {
            return _context.File.Any(e => e.ID == id);
        }
    }
}
