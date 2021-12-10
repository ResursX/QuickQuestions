using System.Collections.Generic;
using System.Linq;

namespace QuickQuestions.Models
{
    public class SurveyResultsViewModel
    {
        public Survey Survey { get; set; }

        public List<QuestionResultsViewModel> QuestionResults { get; set; }

        public List<QuestionResultsViewModel> QuestionBranchResults { get; set; }

        public Branch MostActiveBranch { get; set; }

        public SurveyResultsViewModel()
        {
            QuestionResults = new List<QuestionResultsViewModel>();
            QuestionBranchResults = new List<QuestionResultsViewModel>();
        }

        public SurveyResultsViewModel(Survey survey)
        {
            Survey = survey;
            QuestionResults = Survey.Questions.OrderBy(q => q.Index).Select(q => new QuestionResultsViewModel(survey, q)).ToList();
            QuestionBranchResults = null;
        }

        public SurveyResultsViewModel(Survey survey, List<SurveyResult> branchResults)
        {
            Survey = survey;
            QuestionResults = Survey.Questions.OrderBy(q => q.Index).Select(q => new QuestionResultsViewModel(Survey, q)).ToList();
            QuestionBranchResults = Survey.Questions.OrderBy(q => q.Index).Select(q => new QuestionResultsViewModel(q, branchResults)).ToList();
        }
    }
}
