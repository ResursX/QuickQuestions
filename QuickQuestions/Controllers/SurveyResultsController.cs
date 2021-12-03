using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuickQuestions.Data;
using QuickQuestions.Models;

namespace QuickQuestions.Controllers
{
    public class SurveyResultsController : Controller
    {
        private readonly QuickQuestionsContext _context;

        public SurveyResultsController(QuickQuestionsContext context)
        {
            _context = context;
        }

        // GET: SurveyResults
        public async Task<IActionResult> Index()
        {
            var quickQuestionsContext = _context.SurveyResult.Include(s => s.Survey);
            return View(await quickQuestionsContext.ToListAsync());
        }

        // GET: SurveyResults/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyResult = await _context.SurveyResult
                .Include(s => s.Survey)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (surveyResult == null)
            {
                return NotFound();
            }

            return View(surveyResult);
        }

        // GET: SurveyResults/Create
        public IActionResult Create()
        {
            ViewData["SurveyID"] = new SelectList(_context.Survey, "ID", "Name");
            return View();
        }

        // POST: SurveyResults/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,UserID,SurveyID,DateCreated,DateUpdated,ConcurrencyStamp")] SurveyResult surveyResult)
        {
            if (ModelState.IsValid)
            {
                surveyResult.ID = Guid.NewGuid();
                _context.Add(surveyResult);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SurveyID"] = new SelectList(_context.Survey, "ID", "Name", surveyResult.SurveyID);
            return View(surveyResult);
        }

        // GET: SurveyResults/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyResult = await _context.SurveyResult.FindAsync(id);
            if (surveyResult == null)
            {
                return NotFound();
            }
            ViewData["SurveyID"] = new SelectList(_context.Survey, "ID", "Name", surveyResult.SurveyID);
            return View(surveyResult);
        }

        // POST: SurveyResults/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ID,UserID,SurveyID,DateCreated,DateUpdated,ConcurrencyStamp")] SurveyResult surveyResult)
        {
            if (id != surveyResult.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(surveyResult);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SurveyResultExists(surveyResult.ID))
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
            ViewData["SurveyID"] = new SelectList(_context.Survey, "ID", "Name", surveyResult.SurveyID);
            return View(surveyResult);
        }

        // GET: SurveyResults/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyResult = await _context.SurveyResult
                .Include(s => s.Survey)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (surveyResult == null)
            {
                return NotFound();
            }

            return View(surveyResult);
        }

        // POST: SurveyResults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var surveyResult = await _context.SurveyResult.FindAsync(id);
            _context.SurveyResult.Remove(surveyResult);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SurveyResultExists(Guid id)
        {
            return _context.SurveyResult.Any(e => e.ID == id);
        }
    }
}
