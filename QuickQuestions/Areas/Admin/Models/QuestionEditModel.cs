﻿using QuickQuestions.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuestions.Areas.Admin.Models
{
    public class QuestionEditModel
    {
        public Guid? ID { get; set; }

        public Guid SurveyID { get; set; }

        public int Index { get; set; }

        [Required]
        [StringLength(1000)]
        public string Text { get; set; }

        public List<AnswerEditModel> Answers { get; set; }

        public QuestionEditModel()
        {
            Answers = new List<AnswerEditModel>();
        }

        public QuestionEditModel(Question question)
        {
            ID = question.ID;
            SurveyID = question.SurveyID;
            Index = question.Index;
            Text = question.Text;

            Answers = new List<AnswerEditModel>();

            foreach(Answer answer in question.Answers)
            {
                Answers.Add(new AnswerEditModel(answer));
            }

            Answers.Sort((a, b) => a.Index.CompareTo(b.Index));
        }
    }
}