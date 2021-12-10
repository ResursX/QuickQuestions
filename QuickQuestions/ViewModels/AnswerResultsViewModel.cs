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
            Text = answer.Summary;
            Count = answer.QuestionResults.Count;
            Percent = (double)answer.QuestionResults.Count / survey.SurveyResults.Count;
        }

        public AnswerResultsViewModel(Answer answer, List<SurveyResult> surveyResults)
        {
            Text = answer.Summary;
            Count = surveyResults.Count(sr => sr.QuestionResults.Exists(qr => qr.AnswerID == answer.ID));
            Percent = (double)answer.QuestionResults.Count / surveyResults.Count;
        }

        public static AnswerResultsViewModel AnswerResultsViewModel_Custom(Question question)
        {
            AnswerResultsViewModel result = new AnswerResultsViewModel();

            result.Text = "Свой ответ";
            result.Count = question.QuestionResults.Count(qr => qr.CustomAnswer);
            result.Percent = (double)result.Count / question.QuestionResults.Count;

            return result;
        }

        public static AnswerResultsViewModel AnswerResultsViewModel_Custom(Question question, List<SurveyResult> surveyResults)
        {
            AnswerResultsViewModel result = new AnswerResultsViewModel();

            result.Text = "Свой ответ";
            result.Count = surveyResults.Count(sr => sr.QuestionResults.Exists(qr => qr.QuestionID == question.ID && qr.CustomAnswer));
            result.Percent = (double)result.Count / question.QuestionResults.Count;

            return result;
        }

        public static AnswerResultsViewModel AnswerResultsViewModel_None(Question question)
        {
            AnswerResultsViewModel result = new AnswerResultsViewModel();

            result.Text = "Нет ответа";
            result.Count = question.QuestionResults.Count(qr => qr.AnswerID == null && !qr.CustomAnswer);
            result.Percent = (double)result.Count / question.QuestionResults.Count;

            return result;
        }

        public static AnswerResultsViewModel AnswerResultsViewModel_None(Question question, List<SurveyResult> surveyResults)
        {
            AnswerResultsViewModel result = new AnswerResultsViewModel();

            result.Text = "Нет ответа";
            result.Count = surveyResults.Count(sr => sr.QuestionResults.Exists(qr => qr.QuestionID == question.ID && qr.AnswerID == null && !qr.CustomAnswer));
            result.Percent = (double)result.Count / question.QuestionResults.Count;

            return result;
        }
    }
}
