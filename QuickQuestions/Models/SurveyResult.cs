using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuestions.Models
{
    public class SurveyResult
    {
        public Guid ID { get; set; }

        public string UserID { get; set; }

        public Guid SurveyID { get; set; }
        public Survey Survey { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateCreated { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateUpdated { get; set; }

        public List<QuestionResult> QuestionResults { get; set; }

        public SurveyResult()
        {
            QuestionResults = new List<QuestionResult>();
        }
    }
}
