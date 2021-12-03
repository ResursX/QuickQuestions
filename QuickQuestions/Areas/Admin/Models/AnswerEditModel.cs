using QuickQuestions.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuestions.Areas.Admin.Models
{
    public class AnswerEditModel
    {
        public Guid? ID { get; set; }

        public Guid QuestionID { get; set; }

        public int Index { get; set; }

        [Required]
        [StringLength(1000)]
        public string Text { get; set; }

        public AnswerEditModel() { }

        public AnswerEditModel(Answer answer)
        {
            ID = answer.ID;
            QuestionID = answer.QuestionID;
            Index = answer.Index;
            Text = answer.Text;
        }
    }
}
