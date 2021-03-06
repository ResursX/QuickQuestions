using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuestions.Models
{
    public class QuestionResult
    {
        public Guid ID { get; set; }

        public Guid SurveyResultID { get; set; }
        public SurveyResult SurveyResult { get; set; }

        public Guid? QuestionID { get; set; }
        public Question Question { get; set; }

        public Guid? AnswerID { get; set; }
        public Answer Answer { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateCreated { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateUpdated { get; set; }

        public bool CustomAnswer { get; set; }

        public string Text { get; set; }

        public List<QuestionResultFile> QuestionResultFiles { get; set; }

        public QuestionResult()
        {
            QuestionResultFiles = new List<QuestionResultFile>();
        }
    }
}
