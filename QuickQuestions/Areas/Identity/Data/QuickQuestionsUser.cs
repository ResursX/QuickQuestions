using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace QuickQuestions.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the QuickQuestionsUser class
    public class QuickQuestionsUser : IdentityUser
    {
        [PersonalData]
        public string Name { get; set; }
        [PersonalData]
        public string Surname { get; set; }

        [PersonalData]
        public Guid? BranchID { get; set; }

        [NotMapped]
        public string FullName { get => $"{Name} {Surname}";}
    }
}
