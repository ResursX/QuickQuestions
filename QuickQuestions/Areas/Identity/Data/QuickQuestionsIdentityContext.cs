using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using QuickQuestions.Areas.Identity.Data;
using QuickQuestions.Models;

namespace QuickQuestions.Data
{
    public class QuickQuestionsIdentityContext : IdentityDbContext<QuickQuestionsUser>
    {
        public QuickQuestionsIdentityContext(DbContextOptions<QuickQuestionsIdentityContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        
    }
}
