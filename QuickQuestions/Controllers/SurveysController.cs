using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickQuestions.Areas.Identity.Data;
using QuickQuestions.Data;
using QuickQuestions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuestions.Controllers
{
    public class SurveysController : Controller
    {
        private readonly QuickQuestionsContext _context;
        private readonly UserManager<QuickQuestionsUser> _userManager;

        public SurveysController(QuickQuestionsContext context, UserManager<QuickQuestionsUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            List<Survey> surveys = await _context.Survey
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Answers)
                .Include(s => s.SurveyResults)
                .Where(s => s.DateStart <= DateTimeOffset.UtcNow && s.DateEnd >= DateTimeOffset.UtcNow)
                .ToListAsync();

            QuickQuestionsUser user = await _userManager.GetUserAsync(User);

            IndexSurveyViewModel model = new IndexSurveyViewModel()
            {
                Surveys = surveys,
                UserID = user.Id
            };

            return View(model);
        }

        public async Task<IActionResult> Answer(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey = await _context.Survey
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Answers)
                .Include(s => s.SurveyResults)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (survey == null)
            {
                return NotFound();
            }

            if (survey.DateStart > DateTimeOffset.UtcNow)
            {
                return Unauthorized($"Survey '{survey.ID}' hasn't started yet.");
            }

            if (survey.DateEnd < DateTimeOffset.UtcNow)
            {
                return Unauthorized($"Survey '{survey.ID}' has already expired.");
            }

            QuickQuestionsUser user = await _userManager.GetUserAsync(User);

            if (survey.SurveyResults.FirstOrDefault(sr => sr.UserID == user.Id) != null)
            {
                return Unauthorized($"User's answer for survey '{survey.ID}' already exists.");
            }

            return View(survey);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Answer(Guid? id, IFormCollection formCollection)
        {
            if (id == null)
            {
                return NotFound();
            }

            Survey survey = await _context.Survey
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Answers)
                .Include(s => s.SurveyResults)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (survey == null)
            {
                return NotFound();
            }

            if (survey.DateStart > DateTimeOffset.UtcNow)
            {
                return Unauthorized($"Survey '{survey.ID}' hasn't started yet.");
            }

            if (survey.DateEnd < DateTimeOffset.UtcNow)
            {
                return Unauthorized($"Survey '{survey.ID}' has already expired.");
            }

            QuickQuestionsUser user = await _userManager.GetUserAsync(User);

            if (survey.SurveyResults.FirstOrDefault(sr => sr.UserID == user.Id) != null)
            {
                return Unauthorized($"User's answer for survey '{survey.ID}' already exists.");
            }

            // Validation of answers
            foreach (var question in survey.Questions)
            {
                string str = formCollection[question.ID.ToString()];

                if (str == null)
                {
                    return View(survey);
                }
            }

            SurveyResult result = new SurveyResult()
            {
                ID = Guid.NewGuid(),
                SurveyID = survey.ID,
                UserID = await _userManager.GetUserIdAsync(user)
            };

            await _context.AddAsync(result);

            foreach(var question in survey.Questions)
            {
                Guid answerID = Guid.Parse(formCollection[question.ID.ToString()]);

                QuestionResult questionResult = new QuestionResult()
                {
                    ID = Guid.NewGuid(),
                    SurveyResult = result,
                    AnswerID = answerID
                };

                await _context.AddAsync(questionResult);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Results(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey = await _context.Survey
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Answers)
                .Include(s => s.SurveyResults)
                    .ThenInclude(sr => sr.QuestionResults)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (survey == null)
            {
                return NotFound();
            }

            if (survey.DateStart > DateTimeOffset.UtcNow)
            {
                return Forbid($"Survey '{survey.ID}' hasn't started yet.");
            }

            if (survey.DateEnd < DateTimeOffset.UtcNow)
            {
                return Forbid($"Survey '{survey.ID}' has already expired.");
            }

            Branch mostActiveBranch = await MostActiveBranch(survey, _context, _userManager);

            List<SurveyResult> branchResults = null;

            if (User.IsInRole(RoleInitializer.RoleDepartmentManager))
            {
                QuickQuestionsUser user = await _userManager.GetUserAsync(User);

                branchResults = survey.SurveyResults.Where(sr => _userManager.FindByIdAsync(sr.UserID).Result.BranchID == user.BranchID).ToList();
            }

            ResultsSurveyViewModel result = new ResultsSurveyViewModel()
            {
                Survey = survey,
                BranchResults = branchResults,
                MostActiveBranch = mostActiveBranch
            };

            return View(result);
        }

        private async Task<Branch> MostActiveBranch(Survey survey, QuickQuestionsContext context, UserManager<QuickQuestionsUser> userManager)
        {
            int NoBranchCount = 0;
            Dictionary<Guid, int> branches = new Dictionary<Guid, int>();

            foreach (var result in survey.SurveyResults)
            {
                var user = await userManager.FindByIdAsync(result.UserID);

                var branch = await context.Branch.FirstOrDefaultAsync(b => b.ID == user.BranchID);

                if (branch != null)
                {
                    if (branches.ContainsKey(branch.ID))
                        branches[branch.ID] += 1;
                    else
                        branches.Add(branch.ID, 1);
                }
                else NoBranchCount++;
            }

            if (branches.Count == 0)
            {
                return null;
            }

            Guid maxID = branches.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

            if (branches[maxID] > NoBranchCount)
            {
                return await _context.Branch.FirstOrDefaultAsync(b => b.ID == maxID);
            }
            else
                return null;
        }
    }
}
