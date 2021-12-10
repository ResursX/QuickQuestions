using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickQuestions.Areas.Identity.Data;
using QuickQuestions.Data;
using QuickQuestions.Models;
using System;
using System.Collections.Generic;
using System.IO;
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

            SurveyIndexViewModel model = new SurveyIndexViewModel()
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

            foreach(var question in survey.Questions)
            {
                string answer = formCollection[question.ID.ToString()];

                if (answer != "custom")
                {
                    Guid answerID;

                    try
                    {
                        answerID = Guid.Parse(formCollection[question.ID.ToString()]);
                    }
                    catch
                    {
                        return View(survey);
                    }

                    if ((await _context.Answer.FirstOrDefaultAsync(a => a.ID == answerID)) == null)
                    {
                        return View(survey);
                    }

                    QuestionResult questionResult = new QuestionResult()
                    {
                        ID = Guid.NewGuid(),
                        SurveyResult = result,
                        AnswerID = answerID,
                        CustomAnswer = false
                    };
                }
                else
                {
                    switch (question.CustomAnswerType)
                    {
                        case QuestionCustomAnswerType.customText:
                        case QuestionCustomAnswerType.customRichText:
                            {
                                QuestionResult questionResult = new QuestionResult()
                                {
                                    ID = Guid.NewGuid(),
                                    SurveyResult = result,
                                    CustomAnswer = true,
                                    Text = formCollection[$"{question.ID}_custom"]
                                };

                                break;
                            }
                        case QuestionCustomAnswerType.customFile:
                            {
                                break;
                            }
                    }
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

        public async Task<IActionResult> Report(Guid? id)
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

            using (XLWorkbook workbook = new XLWorkbook())
            {
                foreach (Question question in survey.Questions.OrderBy(q => q.Index))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Add(question.Text);

                    worksheet.Cell(1, 1).Value = question.Text;
                    worksheet.Range(1, 1, 1, 3).Merge().FirstCell().Style
                        .Font.SetBold()
                        .Fill.SetBackgroundColor(XLColor.PastelRed)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    worksheet.Cell(2, 1).Value = "Answer";
                    worksheet.Cell(2, 2).Value = "Amount";
                    worksheet.Cell(2, 3).Value = "%";

                    worksheet.Range(2, 1, 2, 3).Style
                        .Font.SetBold()
                        .Fill.SetBackgroundColor(XLColor.PastelRed)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    int row = 3;
                    foreach (Answer answer in question.Answers.OrderBy(a => a.Index))
                    {
                        worksheet.Cell(row, 1).Value = answer.Text;
                        worksheet.Cell(row, 2).Value = answer.QuestionResults.Count;
                        worksheet.Cell(row, 3).Value = (double)answer.QuestionResults.Count / survey.SurveyResults.Count;

                        row++;
                    }

                    worksheet.Range(3, 3, row, 3).Style.NumberFormat.SetNumberFormatId(10);
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    byte[] content = stream.ToArray();

                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"{survey.Name.Replace(' ', '_')}_{DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss")}.xlsx");
                }
            }
        }
    }
}
