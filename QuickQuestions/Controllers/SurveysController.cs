using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuickQuestions.Areas.Identity.Data;
using QuickQuestions.Data;
using QuickQuestions.Models;
using QuickQuestions.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuestions.Controllers
{
    [Authorize(Roles = "user,manager,admin")]
    public class SurveysController : Controller
    {
        private readonly ILogger<SurveysController> _logger;
        private readonly QuickQuestionsContext _context;
        private readonly UserManager<QuickQuestionsUser> _userManager;

        public SurveysController(QuickQuestionsContext context, UserManager<QuickQuestionsUser> userManager, ILogger<SurveysController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
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

            SurveyIndexViewModel model = new SurveyIndexViewModel()
            {
                Surveys = surveys,
                UserID = user.Id
            };

            return View(model);
        }

        [Authorize(Roles = "user")]
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

            //SurveyAnswerModel model = new SurveyAnswerModel(survey);

            return View(survey);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Answer(Guid? id, [FromForm] Dictionary<string, string> Answers, [FromForm] Dictionary<string, string> CustomAnswersText, IFormCollection formCollection)
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
            //foreach (var question in survey.Questions)
            //{
            //    string str = formCollection[question.ID.ToString()];
            //
            //    if (str == null)
            //    {
            //        return View(survey);
            //    }
            //}

            SurveyResult result = new SurveyResult()
            {
                ID = Guid.NewGuid(),
                SurveyID = survey.ID,
                UserID = await _userManager.GetUserIdAsync(user)
            };

            foreach (Question question in survey.Questions)
            {
                if(Answers.ContainsKey(question.ID.ToString()))
                {
                    string ans = Answers[question.ID.ToString()];

                    switch (ans)
                    {
                        case "false":
                        case null:
                            {
                                result.QuestionResults.Add(new QuestionResult()
                                {
                                    ID = Guid.NewGuid(),
                                    QuestionID = question.ID,
                                    SurveyResult = result,
                                    CustomAnswer = false,
                                });

                                break;
                            }
                        case "custom":
                            {
                                switch (question.CustomAnswerType)
                                {
                                    case QuestionCustomAnswerType.customText:
                                    case QuestionCustomAnswerType.customRichText:
                                        {
                                            result.QuestionResults.Add(new QuestionResult()
                                            {
                                                ID = Guid.NewGuid(),
                                                QuestionID = question.ID,
                                                SurveyResult = result,
                                                CustomAnswer = true,
                                                Text = CustomAnswersText[question.ID.ToString()]
                                            });

                                            break;
                                        }
                                    case QuestionCustomAnswerType.customFile:
                                        {
                                            QuestionResult questionResult = new QuestionResult()
                                            {
                                                ID = Guid.NewGuid(),
                                                QuestionID = question.ID,
                                                SurveyResult = result,
                                                CustomAnswer = true
                                            };

                                            foreach(IFormFile file in formCollection.Files.Where(f => f.Name == question.ID.ToString()))
                                            {
                                                using (BinaryReader binaryReader = new BinaryReader(file.OpenReadStream()))
                                                {
                                                    questionResult.QuestionResultFiles.Add(new QuestionResultFile()
                                                    {
                                                        ID = Guid.NewGuid(),
                                                        FileName = file.FileName,
                                                        ContentType = file.ContentType,
                                                        Content = binaryReader.ReadBytes((int)file.Length)
                                                    });
                                                }
                                            }

                                            result.QuestionResults.Add(questionResult);

                                            break;
                                        }
                                    default:
                                        {
                                            _logger.LogError("0");

                                            return View(survey);
                                        }
                                }

                                break;
                            }
                        default:
                            {
                                Guid answerID;

                                if (Guid.TryParse(ans, out answerID))
                                {
                                    var answer = await _context.Answer.FirstOrDefaultAsync(a => a.ID == answerID);

                                    if (answer == null)
                                    {
                                        _logger.LogError("1");

                                        return View(survey);
                                    }

                                    result.QuestionResults.Add(new QuestionResult()
                                    {
                                        ID = Guid.NewGuid(),
                                        QuestionID = question.ID,
                                        SurveyResult = result,
                                        AnswerID = answerID,
                                        CustomAnswer = false
                                    });
                                }
                                else
                                {
                                    _logger.LogError("2");

                                    return View(survey);
                                }

                                break;
                            }
                    }
                }
                else
                {
                    result.QuestionResults.Add(new QuestionResult()
                    {
                        ID = Guid.NewGuid(),
                        QuestionID = question.ID,
                        SurveyResult = result,
                        CustomAnswer = false,
                    });
                }
            }

            _context.Add(result);
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
                    .ThenInclude(a => a.QuestionResults)
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

            if (survey.DateEnd < DateTimeOffset.UtcNow && !User.IsInRole(RoleInitializer.RoleAdministrator))
            {
                return Forbid($"Survey '{survey.ID}' has already expired.");
            }

            SurveyResultsViewModel result;

            if (!User.IsInRole(RoleInitializer.RoleDepartmentManager))
            {
                result = new SurveyResultsViewModel(survey);
            }
            else
            {
                QuickQuestionsUser user = await _userManager.GetUserAsync(User);

                List<SurveyResult> branchResults = survey.SurveyResults.Where(sr => _userManager.FindByIdAsync(sr.UserID).Result.BranchID == user.BranchID).ToList();

                result = new SurveyResultsViewModel(survey, branchResults);
            }

            result.MostActiveBranch = await MostActiveBranch(survey, _context, _userManager);

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
