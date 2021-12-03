using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickQuestions.Areas.Identity.Data;
using QuickQuestions.Data;

[assembly: HostingStartup(typeof(QuickQuestions.Areas.Identity.IdentityHostingStartup))]
namespace QuickQuestions.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((Action<WebHostBuilderContext, IServiceCollection>)((context, services) => {

                services.AddDbContext<QuickQuestionsIdentityContext>(options =>
                    options.UseSqlServer(context.Configuration.GetConnectionString("QuickQuestionsIdentityContext")));

                IdentityEntityFrameworkBuilderExtensions
                    .AddEntityFrameworkStores<QuickQuestionsIdentityContext>(
                            services.AddIdentity<QuickQuestionsUser, IdentityRole>((Action<IdentityOptions>)(options => options.SignIn.RequireConfirmedAccount = true))
                        )
                    .AddDefaultTokenProviders();

                services.ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = $"/Identity/Account/Login";
                    options.LogoutPath = $"/Identity/Account/Logout";
                    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
                });

            }));
        }
    }
}