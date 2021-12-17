using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuestions.Areas.Admin.Models
{
    public class EditUserViewModel
    {
        public string UserId { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [EmailAddress]
        public string UserEmail { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string UserSurname { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string UserPatronymic { get; set; }

        public Guid? UserBranchID { get; set; }

        public List<IdentityRole> AllRoles { get; set; }
        public IList<string> UserRoles { get; set; }

        public EditUserViewModel()
        {
            AllRoles = new List<IdentityRole>();
            UserRoles = new List<string>();
        }
    }
}
