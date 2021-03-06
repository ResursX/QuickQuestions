// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuickQuestions.Data;

namespace QuickQuestions.Migrations.QuickQuestions
{
    [DbContext(typeof(QuickQuestionsContext))]
    [Migration("20211208194238_Update1")]
    partial class Update1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.12")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("QuickQuestions.Models.Answer", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateCreated")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<DateTime>("DateUpdated")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<int>("Index")
                        .HasColumnType("int");

                    b.Property<Guid>("QuestionID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Summary")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("QuestionID");

                    b.ToTable("Answer");
                });

            modelBuilder.Entity("QuickQuestions.Models.Branch", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("DateCreated")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<DateTimeOffset>("DateUpdated")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("ID");

                    b.ToTable("Branch");
                });

            modelBuilder.Entity("QuickQuestions.Models.Question", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("CustomAnswerType")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateCreated")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<DateTime>("DateUpdated")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<int>("Index")
                        .HasColumnType("int");

                    b.Property<string>("Summary")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("SurveyID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("SurveyID");

                    b.ToTable("Question");
                });

            modelBuilder.Entity("QuickQuestions.Models.QuestionResult", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AnswerID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateCreated")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<DateTime>("DateUpdated")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<Guid?>("QuestionID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SurveyResultID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("AnswerID");

                    b.HasIndex("QuestionID");

                    b.HasIndex("SurveyResultID");

                    b.ToTable("QuestionResult");
                });

            modelBuilder.Entity("QuickQuestions.Models.QuestionResultFile", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte[]>("Content")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("ContentType")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTimeOffset>("DateCreated")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<DateTimeOffset>("DateUpdated")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<string>("FileName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("QuestionResultID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ID");

                    b.HasIndex("QuestionResultID");

                    b.ToTable("QuestionResultFile");
                });

            modelBuilder.Entity("QuickQuestions.Models.Survey", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("DateCreated")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<DateTimeOffset>("DateEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("DateStart")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("DateUpdated")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Survey");
                });

            modelBuilder.Entity("QuickQuestions.Models.SurveyResult", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateCreated")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<DateTime>("DateUpdated")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<Guid>("SurveyID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UserID")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("SurveyID");

                    b.ToTable("SurveyResult");
                });

            modelBuilder.Entity("QuickQuestions.Models.Answer", b =>
                {
                    b.HasOne("QuickQuestions.Models.Question", "Question")
                        .WithMany("Answers")
                        .HasForeignKey("QuestionID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Question");
                });

            modelBuilder.Entity("QuickQuestions.Models.Question", b =>
                {
                    b.HasOne("QuickQuestions.Models.Survey", "Survey")
                        .WithMany("Questions")
                        .HasForeignKey("SurveyID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Survey");
                });

            modelBuilder.Entity("QuickQuestions.Models.QuestionResult", b =>
                {
                    b.HasOne("QuickQuestions.Models.Answer", "Answer")
                        .WithMany("QuestionResults")
                        .HasForeignKey("AnswerID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("QuickQuestions.Models.Question", "Question")
                        .WithMany("QuestionResults")
                        .HasForeignKey("QuestionID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("QuickQuestions.Models.SurveyResult", "SurveyResult")
                        .WithMany("QuestionResults")
                        .HasForeignKey("SurveyResultID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Answer");

                    b.Navigation("Question");

                    b.Navigation("SurveyResult");
                });

            modelBuilder.Entity("QuickQuestions.Models.QuestionResultFile", b =>
                {
                    b.HasOne("QuickQuestions.Models.QuestionResult", "QuestionResult")
                        .WithMany("QuestionResultFiles")
                        .HasForeignKey("QuestionResultID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("QuestionResult");
                });

            modelBuilder.Entity("QuickQuestions.Models.SurveyResult", b =>
                {
                    b.HasOne("QuickQuestions.Models.Survey", "Survey")
                        .WithMany("SurveyResults")
                        .HasForeignKey("SurveyID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Survey");
                });

            modelBuilder.Entity("QuickQuestions.Models.Answer", b =>
                {
                    b.Navigation("QuestionResults");
                });

            modelBuilder.Entity("QuickQuestions.Models.Question", b =>
                {
                    b.Navigation("Answers");

                    b.Navigation("QuestionResults");
                });

            modelBuilder.Entity("QuickQuestions.Models.QuestionResult", b =>
                {
                    b.Navigation("QuestionResultFiles");
                });

            modelBuilder.Entity("QuickQuestions.Models.Survey", b =>
                {
                    b.Navigation("Questions");

                    b.Navigation("SurveyResults");
                });

            modelBuilder.Entity("QuickQuestions.Models.SurveyResult", b =>
                {
                    b.Navigation("QuestionResults");
                });
#pragma warning restore 612, 618
        }
    }
}
