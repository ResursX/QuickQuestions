using Microsoft.EntityFrameworkCore;
using QuickQuestions.Areas.Identity.Data;
using QuickQuestions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuestions.Areas.Admin.Models
{
    public class IndexUserViewModel
    {
        public IList<QuickQuestionsUser> Users;

        public DbSet<Branch> Branches;
    }
}
