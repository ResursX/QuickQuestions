using System.Collections.Generic;

namespace QuickQuestions.Models
{
    public class ResultsSurveyViewModel
    {
        public Survey Survey { get; set; }

        public List<SurveyResult> BranchResults { get; set; }

        public Branch MostActiveBranch { get; set; }
    }
}
