using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuickQuestions.Data;
using QuickQuestions.Models;

namespace QuickQuestions.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class UserFilesController : Controller
    {
        private readonly QuickQuestionsContext _context;

        public UserFilesController(QuickQuestionsContext context)
        {
            _context = context;
        }

        // GET: Admin/UserFiles
        public async Task<IActionResult> Index()
        {
            var quickQuestionsContext = _context.QuestionResultFile.Include(q => q.QuestionResult);
            return View(await quickQuestionsContext.ToListAsync());
        }

        public async Task<IActionResult> View(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionResultFile = await _context.QuestionResultFile
                .FirstOrDefaultAsync(m => m.ID == id);

            if (questionResultFile == null)
            {
                return NotFound();
            }

            return File(questionResultFile.Content, questionResultFile.ContentType);
        }

        // GET: Admin/UserFiles/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionResultFile = await _context.QuestionResultFile
                .Include(q => q.QuestionResult)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (questionResultFile == null)
            {
                return NotFound();
            }

            return View(questionResultFile);
        }

        // GET: Admin/UserFiles/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionResultFile = await _context.QuestionResultFile
                .Include(q => q.QuestionResult)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (questionResultFile == null)
            {
                return NotFound();
            }

            return View(questionResultFile);
        }

        // POST: Admin/UserFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var questionResultFile = await _context.QuestionResultFile.FindAsync(id);
            _context.QuestionResultFile.Remove(questionResultFile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
