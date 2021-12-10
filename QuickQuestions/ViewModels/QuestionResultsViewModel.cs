using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuestions.Models
{
    public class QuestionResultsViewModel
    {
        public string Summary { get; set; }
        public string Text { get; set; }

        public List<AnswerResultsViewModel> AnswerResults { get; set; }

        public QuestionResultsViewModel()
        {
            AnswerResults = new List<AnswerResultsViewModel>();
        }

        public QuestionResultsViewModel(Survey survey, Question question)
        {
            Summary = question.Summary;
            Text = question.Text;
            AnswerResults = question.Answers.OrderBy(a => a.Index).Select(a => new AnswerResultsViewModel(survey, a)).ToList();
        }

        public QuestionResultsViewModel(Question question, List<SurveyResult> surveyResults)
        {
            Summary = question.Summary;
            Text = question.Text;
            AnswerResults = question.Answers.OrderBy(a => a.Index).Select(a => new AnswerResultsViewModel(a, surveyResults)).ToList();
        }
    }
}
