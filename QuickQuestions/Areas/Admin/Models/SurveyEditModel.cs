using QuickQuestions.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuestions.Areas.Admin.Models
{
    public class SurveyEditModel
    {
        public Guid? ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Text { get; set; }

        [Required]
        public DateTimeOffset DateStart { get; set; }
        [Required]
        public DateTimeOffset DateEnd { get; set; }

        public List<QuestionEditModel> Questions { get; set; }

        public SurveyEditModel()
        {
            Questions = new List<QuestionEditModel>();
        }

        public SurveyEditModel(Survey survey)
        {
            ID = survey.ID;
            Name = survey.Name;
            Text = survey.Text;
            DateStart = survey.DateStart;
            DateEnd = survey.DateEnd;

            Questions = new List<QuestionEditModel>();

            foreach (Question question in survey.Questions.OrderBy(q => q.Index))
            {
                Questions.Add(new QuestionEditModel(question));
            }
        }
    }
}
