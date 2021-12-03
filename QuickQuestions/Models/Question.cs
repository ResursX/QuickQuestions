using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuestions.Models
{
    public class Question
    {
        public Guid ID { get; set; }

        public Guid SurveyID { get; set; }
        public Survey Survey { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateCreated { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateUpdated { get; set; }

        public int Index { get; set; }

        [Required]
        [StringLength(1000)]
        public string Text { get; set; }

        public List<Answer> Answers { get; set; }

        public List<QuestionResult> QuestionResults { get; set; }

        public Question()
        {
            Answers = new List<Answer>();
            QuestionResults = new List<QuestionResult>();
        }
    }
}
