using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuestions.Models
{
    public class AnswerResultsViewModel
    {
        public string Text { get; set; }

        public int Count { get; set; }

        public double Percent { get; set; }

        public AnswerResultsViewModel() {}

        public AnswerResultsViewModel(Survey survey, Answer answer)
        {
            Text = answer.Text;
            Count = answer.QuestionResults.Count;
            Percent = (double)answer.QuestionResults.Count / survey.SurveyResults.Count;
        }

        public AnswerResultsViewModel(Answer answer, List<SurveyResult> surveyResults)
        {
            Text = answer.Text;
            Count = surveyResults.Count(sr => sr.QuestionResults.Exists(qr => qr.AnswerID == answer.ID));
            Percent = (double)answer.QuestionResults.Count / surveyResults.Count;
        }
    }
}
