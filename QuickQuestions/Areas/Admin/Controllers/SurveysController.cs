using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuickQuestions.Areas.Admin.Models;
using QuickQuestions.Data;
using QuickQuestions.Models;
using QuickQuestions.ViewModels;

namespace QuickQuestions.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class SurveysController : Controller
    {
        private readonly ILogger<SurveysController> _logger;
        private readonly QuickQuestionsContext _context;

        public SurveysController(ILogger<SurveysController> logger, QuickQuestionsContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: Surveys
        public async Task<IActionResult> Index()
        {
            return View(await _context.Survey.ToListAsync());
        }

        // GET: Surveys/Render/5
        public async Task<IActionResult> Render(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey = await _context.Survey
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (survey == null)
            {
                return NotFound();
            }

            //SurveyAnswerModel model = new SurveyAnswerModel(survey);

            return View(survey);
        }

        // GET: Surveys/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Surveys/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Text,DateStart,DateEnd,Questions")] SurveyEditModel surveyModel)
        {
            if (ModelState.IsValid)
            {
                //StringBuilder sb = new StringBuilder();
                //
                //sb.AppendLine($"{survey.Name}");
                //sb.AppendLine($"{survey.Text}");
                //sb.AppendLine($"{survey.DateStart}");
                //sb.AppendLine($"{survey.DateEnd}");
                //
                //foreach(Question question in survey.Questions)
                //{
                //    sb.AppendLine($"{question.Index} {question.Text}");
                //    foreach (Answer answer in question.Answers)
                //    {
                //        sb.AppendLine($"{question.Index} {answer.Index} {answer.Text}");
                //    }
                //}
                //
                //_logger.LogInformation(sb.ToString());

                Survey survey = new Survey()
                {
                    ID = Guid.NewGuid(),
                    Name = surveyModel.Name,
                    Text = surveyModel.Text,
                    DateStart = surveyModel.DateStart,
                    DateEnd = surveyModel.DateEnd
                };

                foreach (QuestionEditModel questionModel in surveyModel.Questions)
                {
                    Question question = new Question()
                    {
                        ID = Guid.NewGuid(),
                        Index = questionModel.Index,
                        Summary = questionModel.Summary,
                        Text = questionModel.Text,
                        CustomAnswerType = questionModel.CustomAnswerType
                    };

                    foreach (AnswerEditModel answerModel in questionModel.Answers)
                    {
                        question.Answers.Add(new Answer()
                        {
                            ID = Guid.NewGuid(),
                            Index = answerModel.Index,
                            Summary = answerModel.Summary,
                            Text = answerModel.Text
                        });
                    }

                    survey.Questions.Add(question);
                }

                _context.Add(survey);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(surveyModel);
        }

        // GET: Surveys/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Survey survey = await _context.Survey.Include(s => s.Questions).ThenInclude(q => q.Answers).FirstOrDefaultAsync(s => s.ID == id);

            if (survey == null)
            {
                return NotFound();
            }

            SurveyEditModel surveyModel = new SurveyEditModel(survey);

            return View(surveyModel);
        }

        // POST: Surveys/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ID,Name,Text,DateStart,DateEnd,Questions")] SurveyEditModel surveyModel)
        {
            if (id != surveyModel.ID)
            {
                return NotFound();
            }

            Survey survey = await _context.Survey.Include(s => s.Questions).ThenInclude(q => q.Answers).FirstOrDefaultAsync(s => s.ID == id);

            if (survey == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    survey.Name = surveyModel.Name;
                    survey.Text = surveyModel.Text;
                    survey.DateStart = surveyModel.DateStart;
                    survey.DateEnd = surveyModel.DateEnd;

                    for (int i = 0; i < survey.Questions.Count; i++)
                    {
                        QuestionEditModel questionModel = surveyModel.Questions.Find(q => q.ID == survey.Questions[i].ID);

                        if (questionModel != null)
                        {
                            survey.Questions[i].Index = questionModel.Index;
                            survey.Questions[i].Summary = questionModel.Summary;
                            survey.Questions[i].Text = questionModel.Text;
                            survey.Questions[i].CustomAnswerType = questionModel.CustomAnswerType;

                            for (int j = 0; j < survey.Questions[i].Answers.Count; j++)
                            {
                                AnswerEditModel answerModel = questionModel.Answers.Find(a => a.ID == survey.Questions[i].Answers[j].ID);

                                if (answerModel != null)
                                {
                                    survey.Questions[i].Answers[j].Index = answerModel.Index;
                                    survey.Questions[i].Answers[j].Summary = answerModel.Summary;
                                    survey.Questions[i].Answers[j].Text = answerModel.Text;
                                }
                                else
                                {
                                    survey.Questions[i].Answers.RemoveAt(j);
                                }

                                questionModel.Answers.Remove(answerModel);
                            }

                            foreach(AnswerEditModel answerModel in questionModel.Answers)
                            {
                                _context.Add(new Answer()
                                {
                                    ID = Guid.NewGuid(),
                                    Question = survey.Questions[i],
                                    Index = answerModel.Index,
                                    Summary = answerModel.Summary,
                                    Text = answerModel.Text,
                                });
                            }

                            surveyModel.Questions.Remove(questionModel);
                        }
                        else
                        {
                            survey.Questions.RemoveAt(i);
                        }
                    }

                    foreach (QuestionEditModel questionModel in surveyModel.Questions)
                    {
                        Question question = new Question()
                        {
                            ID = Guid.NewGuid(),
                            Survey = survey,
                            Index = questionModel.Index,
                            Summary = questionModel.Summary,
                            Text = questionModel.Text,
                            CustomAnswerType = questionModel.CustomAnswerType
                        };

                        _context.Add(question);

                        foreach (AnswerEditModel answerModel in questionModel.Answers)
                        {
                            _context.Add(new Answer()
                            {
                                ID = Guid.NewGuid(),
                                Question = question,
                                Index = answerModel.Index,
                                Summary = answerModel.Summary,
                                Text = answerModel.Text,
                            });
                        }
                    }

                    _context.Update(survey);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SurveyExists(survey.ID))
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
            return View(surveyModel);
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
                    .ThenInclude(a => a.QuestionResults)
                .Include(s => s.SurveyResults)
                    .ThenInclude(sr => sr.QuestionResults)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (survey == null)
            {
                return NotFound();
            }

            SurveyResultsViewModel result = new SurveyResultsViewModel(survey);

            using (XLWorkbook workbook = new XLWorkbook())
            {
                foreach (QuestionResultsViewModel questionResult in result.QuestionResults.Where(qr => !qr.CustomOnly))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Add(questionResult.Text);

                    worksheet.Cell(1, 1).Value = questionResult.Text;
                    worksheet.Range(1, 1, 1, 3).Merge().FirstCell().Style
                        .Font.SetBold()
                        .Fill.SetBackgroundColor(XLColor.PastelRed)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    var table = worksheet.Cell(2, 1).InsertTable(questionResult.AnswerResults);

                    //worksheet.Range(3, 3, row, 3).Style.NumberFormat.SetNumberFormatId(10);
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

        // GET: Surveys/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey = await _context.Survey
                .FirstOrDefaultAsync(m => m.ID == id);
            if (survey == null)
            {
                return NotFound();
            }

            return View(survey);
        }

        // POST: Surveys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var survey = await _context.Survey.FindAsync(id);
            _context.Survey.Remove(survey);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SurveyExists(Guid id)
        {
            return _context.Survey.Any(e => e.ID == id);
        }

        //AJAX Requests

        private void SortModelArray(SurveyEditModel surveyModel)
        {
            surveyModel.Questions.Sort((a, b) => a.Index.CompareTo(b.Index));

            foreach (QuestionEditModel question in surveyModel.Questions)
            {
                question.Answers.Sort((a, b) => a.Index.CompareTo(b.Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddQuestion([Bind("Questions")] SurveyEditModel surveyModel)
        {
            SortModelArray(surveyModel);

            surveyModel.Questions.Add(new QuestionEditModel()
            {
                Index = surveyModel.Questions.Count
            });

            ModelState.Clear(); // Lol, this thing has priority over passed model data

            return PartialView("_Questions", surveyModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddAnswer([Bind("Questions")] SurveyEditModel surveyModel, int index)
        {
            SortModelArray(surveyModel);

            surveyModel.Questions[index].Answers.Add(new AnswerEditModel()
            {
                Index = surveyModel.Questions[index].Answers.Count
            });

            ModelState.Clear(); // Lol, this thing has priority over passed model data

            return PartialView("_Questions", surveyModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MoveAnswer([Bind("Questions")] SurveyEditModel surveyModel, int questionIndexOld, int indexOld, int questionIndexNew, int indexNew)
        {
            SortModelArray(surveyModel);

            surveyModel.Questions[questionIndexNew].Answers.Insert(indexNew, surveyModel.Questions[questionIndexOld].Answers[indexOld]);

            surveyModel.Questions[questionIndexNew].Answers[indexNew].Index = indexNew;

            for(int i = indexNew + 1; i < surveyModel.Questions[questionIndexNew].Answers.Count; i++)
            {
                surveyModel.Questions[questionIndexNew].Answers[i].Index = i;
            }

            surveyModel.Questions[questionIndexOld].Answers.RemoveAt(indexOld);

            ModelState.Clear(); // Lol, this thing has priority over passed model data

            return PartialView("_Questions", surveyModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DebugReload([Bind("Questions")] SurveyEditModel surveyModel)
        {
            SortModelArray(surveyModel);

            ModelState.Clear(); // Lol, this thing has priority over passed model data

            return PartialView("_Questions", surveyModel);
        }
    }
}
