using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuickQuestions.Areas.Admin.Models;
using QuickQuestions.Data;
using QuickQuestions.Models;

namespace QuickQuestions.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class QuestionsController : Controller
    {
        private readonly ILogger<QuestionsController> _logger;
        private readonly QuickQuestionsContext _context;

        public QuestionsController(ILogger<QuestionsController> logger, QuickQuestionsContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: Questions
        public async Task<IActionResult> Index()
        {
            var quickQuestionsContext = _context.Question.Include(q => q.Survey);
            return View(await quickQuestionsContext.ToListAsync());
        }

        // GET: Questions/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Question
                .Include(q => q.Survey)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // GET: Questions/Create
        public IActionResult Create()
        {
            ViewData["SurveyID"] = new SelectList(_context.Survey, "ID", "Name");
            return View();
        }

        // POST: Questions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SurveyID,Index,Text,Answers")] QuestionEditModel question)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Question result = new Question()
                    {
                        ID = Guid.NewGuid(),
                        SurveyID = question.SurveyID,
                        Text = question.Text,
                        Index = question.Index
                    };

                    _context.Add(result);

                    foreach (var answer in question.Answers)
                    {
                        Answer resultAnswer = new Answer()
                        {
                            ID = Guid.NewGuid(),
                            Question = result,
                            Text = answer.Text,
                            Index = answer.Index
                        };

                        _context.Add(resultAnswer);
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Error happeded while updating database.");
            }
            
            ViewData["SurveyID"] = new SelectList(_context.Survey, "ID", "Name", question.SurveyID);
            return View(question);
        }

        // GET: Questions/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Question question = await _context.Question.Include(q => q.Answers).FirstOrDefaultAsync(q => q.ID == id);
            if (question == null)
            {
                return NotFound();
            }

            QuestionEditModel model = new QuestionEditModel(question);

            ViewData["SurveyID"] = new SelectList(_context.Survey, "ID", "Name", question.SurveyID);
            return View(model);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ID,SurveyID,Index,Text,Answers")] QuestionEditModel model)
        {
            if (id != model.ID)
            {
                return NotFound();
            }

            Question question = await _context.Question.Include(q => q.Answers).FirstOrDefaultAsync(q => q.ID == id);

            if (question == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    question.SurveyID = model.SurveyID;
                    question.Index = model.Index;
                    question.Text = model.Text;

                    List<Answer> answers = new List<Answer>(question.Answers);

                    foreach (AnswerEditModel answerModel in model.Answers)
                    {
                        Answer answer;

                        //_logger.LogInformation($"{answerModel.ID}");

                        if (answerModel.ID != null)
                        {
                            int i = answers.FindIndex(a => a.ID == answerModel.ID);

                            if (i < 0)
                            {
                                return NotFound();
                            }

                            answer = answers[i];
                            answers.RemoveAt(i);

                            answer.Index = answerModel.Index;
                            answer.Text = answerModel.Text;
                        }
                        else
                        {
                            answer = new Answer()
                            {
                                ID = Guid.NewGuid(),
                                QuestionID = id,
                                Index = answerModel.Index,
                                Text = answerModel.Text
                            };

                            _context.Add(answer);
                        }
                    }

                    foreach (Answer answer in answers)
                    {
                        //_logger.LogInformation($"Removing {answer.ID}");

                        _context.Answer.Remove(answer);
                    }

                    _context.Update(question);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(question.ID))
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
            ViewData["SurveyID"] = new SelectList(_context.Survey, "ID", "Name", question.SurveyID);
            return View(model);
        }

        // GET: Questions/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Question
                .Include(q => q.Survey)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var question = await _context.Question.FindAsync(id);
            _context.Question.Remove(question);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionExists(Guid? id)
        {
            if (id == null)
                return false;

            return _context.Question.Any(e => e.ID == id);
        }

        // Answers

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddAnswer([Bind("Answers")] QuestionEditModel question)
        {
            question.Answers.Sort((a, b) => a.Index.CompareTo(b.Index));

            question.Answers.Add(new AnswerEditModel() {
                Index = question.Answers.Count
            });

            ModelState.Clear(); // Lol, this thing has priority over passed model data

            return PartialView("_Answers", question);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DebugReload([Bind("Answers")] QuestionEditModel question)
        {
            question.Answers.Sort((a, b) => a.Index.CompareTo(b.Index));

            ModelState.Clear(); // Lol, this thing has priority over passed model data

            return PartialView("_Answers", question);
        }
    }
}
