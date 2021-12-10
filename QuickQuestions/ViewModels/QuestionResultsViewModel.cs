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

        public bool CustomOnly { get; set; }

        public List<AnswerResultsViewModel> AnswerResults { get; set; }

        public QuestionResultsViewModel()
        {
            AnswerResults = new List<AnswerResultsViewModel>();
        }

        public QuestionResultsViewModel(Survey survey, Question question)
        {
            Summary = question.Summary;
            Text = question.Text;
            CustomOnly = (question.CustomAnswerType != QuestionCustomAnswerType.noCustom && question.Answers.Count == 0);

            if (!CustomOnly)
            {
                AnswerResults = question.Answers.OrderBy(a => a.Index).Select(a => new AnswerResultsViewModel(survey, a)).ToList();

                if (question.CustomAnswerType != QuestionCustomAnswerType.noCustom)
                {
                    AnswerResults.Add(AnswerResultsViewModel.AnswerResultsViewModel_Custom(question));
                }

                AnswerResults.Add(AnswerResultsViewModel.AnswerResultsViewModel_None(question));
            }
            else
            {
                AnswerResults = new List<AnswerResultsViewModel>();
            }
        }

        public QuestionResultsViewModel(Question question, List<SurveyResult> surveyResults)
        {
            Summary = question.Summary;
            Text = question.Text;
            CustomOnly = (question.CustomAnswerType != QuestionCustomAnswerType.noCustom && question.Answers.Count == 0);

            if (!CustomOnly)
            {
                AnswerResults = question.Answers.OrderBy(a => a.Index).Select(a => new AnswerResultsViewModel(a, surveyResults)).ToList();

                if (question.CustomAnswerType != QuestionCustomAnswerType.noCustom)
                {
                    AnswerResults.Add(AnswerResultsViewModel.AnswerResultsViewModel_Custom(question, surveyResults));
                }

                AnswerResults.Add(AnswerResultsViewModel.AnswerResultsViewModel_None(question, surveyResults));
            }
            else
            {
                AnswerResults = new List<AnswerResultsViewModel>();
            }
        }
    }
}
