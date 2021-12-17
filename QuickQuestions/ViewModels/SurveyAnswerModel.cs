using Microsoft.AspNetCore.Http;
using QuickQuestions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuestions.ViewModels
{
    public class SurveyAnswerModel
    {
        public Survey Survey { get; set; }

        public Dictionary<string, string> Answers { get; set; }

        public Dictionary<string, string> CustomAnswersText { get; set; }

        public Dictionary<string, IFormFile> CustomAnswersFiles { get; set; }

        public SurveyAnswerModel()
        {
            Answers = new Dictionary<string, string>();
            CustomAnswersText = new Dictionary<string, string>();
            CustomAnswersFiles = new Dictionary<string, IFormFile>();
        }

        public SurveyAnswerModel(Survey survey)
        {
            Survey = survey;

            Answers = new Dictionary<string, string>();
            CustomAnswersText = new Dictionary<string, string>();
            CustomAnswersFiles = new Dictionary<string, IFormFile>();

            foreach (Question question in survey.Questions)
            {
                Answers.Add(question.ID.ToString(), null);
                
                if(question.CustomAnswerType != QuestionCustomAnswerType.noCustom)
                {
                    switch (question.CustomAnswerType)
                    {
                        case QuestionCustomAnswerType.customText:
                        case QuestionCustomAnswerType.customRichText:
                            {
                                CustomAnswersText.Add(question.ID.ToString(), null);
                                break;
                            }
                        case QuestionCustomAnswerType.customFile:
                            {
                                CustomAnswersFiles.Add(question.ID.ToString(), null);
                                break;
                            }
                    }
                }
            }
        }
    }
}
