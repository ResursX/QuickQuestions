using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuestions.Models
{
    public class Survey
    {
        public Guid ID { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset DateCreated { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset DateUpdated { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Text { get; set; }

        [Required]
        public DateTimeOffset DateStart { get; set; }
        [Required]
        public DateTimeOffset DateEnd { get; set; }

        public List<Question> Questions { get; set; }

        public List<SurveyResult> SurveyResults { get; set; }

        public Survey()
        {
            Questions = new List<Question>();
            SurveyResults = new List<SurveyResult>();
        }
    }
}
