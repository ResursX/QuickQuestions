using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using QuickQuestions.Models;

namespace QuickQuestions.Data
{
    public class QuickQuestionsContext : DbContext
    {
        public QuickQuestionsContext(DbContextOptions<QuickQuestionsContext> options)
            : base(options)
        {
        }

        public DbSet<Survey> Survey { get; set; }

        public DbSet<Question> Question { get; set; }

        public DbSet<Answer> Answer { get; set; }

        public DbSet<QuestionResult> QuestionResult { get; set; }

        public DbSet<SurveyResult> SurveyResult { get; set; }

        public DbSet<Branch> Branch { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            // Survey

            builder.Entity<Survey>()
                .Property(b => b.DateCreated)
                .HasDefaultValueSql("GETUTCDATE()")
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            builder.Entity<Survey>()
                .Property(b => b.DateUpdated)
                .HasDefaultValueSql("GETUTCDATE()")
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            // Question

            builder.Entity<Question>()
                .HasOne(p => p.Survey)
                .WithMany(b => b.Questions)
                .HasForeignKey(p => p.SurveyID);

            builder.Entity<Question>()
                .Property(b => b.DateCreated)
                .HasDefaultValueSql("GETUTCDATE()")
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            builder.Entity<Question>()
                .Property(b => b.DateUpdated)
                .HasDefaultValueSql("GETUTCDATE()")
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            // Answer

            builder.Entity<Answer>()
                .HasOne(p => p.Question)
                .WithMany(b => b.Answers)
                .HasForeignKey(p => p.QuestionID);

            builder.Entity<Answer>()
                .Property(b => b.DateCreated)
                .HasDefaultValueSql("GETUTCDATE()")
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            builder.Entity<Answer>()
                .Property(b => b.DateUpdated)
                .HasDefaultValueSql("GETUTCDATE()")
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            // SurveyResult

            builder.Entity<SurveyResult>()
                .HasOne(p => p.Survey)
                .WithMany(b => b.SurveyResults)
                .HasForeignKey(p => p.SurveyID);

            //builder.Entity<SurveyResult>()
            //    .HasOne(p => p.QuickQuestionsUser)
            //    .WithMany(b => b.SurveyResults)
            //    .HasForeignKey(p => p.QuickQuestionsUserID);

            builder.Entity<SurveyResult>()
                .Property(b => b.DateCreated)
                .HasDefaultValueSql("GETUTCDATE()")
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            builder.Entity<SurveyResult>()
                .Property(b => b.DateUpdated)
                .HasDefaultValueSql("GETUTCDATE()")
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            // QuestionResult

            builder.Entity<QuestionResult>()
                .HasOne(p => p.SurveyResult)
                .WithMany(b => b.QuestionResults)
                .HasForeignKey(p => p.SurveyResultID);

            builder.Entity<QuestionResult>()
                .HasOne(p => p.Answer)
                .WithMany(b => b.QuestionResults)
                .HasForeignKey(p => p.AnswerID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<QuestionResult>()
                .Property(b => b.DateCreated)
                .HasDefaultValueSql("GETUTCDATE()")
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            builder.Entity<QuestionResult>()
                .Property(b => b.DateUpdated)
                .HasDefaultValueSql("GETUTCDATE()")
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            // Branches

            //builder.Entity<Branch>()
            //    .HasMany(p => p.QuickQuestionsUsers)
            //    .WithOne(b => b.Branch)
            //    .HasForeignKey(b => b.BranchID)
            //    .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Branch>()
                .Property(b => b.DateCreated)
                .HasDefaultValueSql("GETUTCDATE()")
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            builder.Entity<Branch>()
                .Property(b => b.DateUpdated)
                .HasDefaultValueSql("GETUTCDATE()")
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        }

        
    }
}
