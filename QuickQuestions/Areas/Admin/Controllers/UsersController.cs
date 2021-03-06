using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuickQuestions.Areas.Admin.Models;
using QuickQuestions.Areas.Identity.Data;
using QuickQuestions.Data;
using QuickQuestions.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuestions.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly QuickQuestionsContext _context;
        private readonly UserManager<QuickQuestionsUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(ILogger<UsersController> logger, QuickQuestionsContext context, UserManager<QuickQuestionsUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var branches = _context.Branch;

            IndexUserViewModel model = new IndexUserViewModel()
            {
                Users = users,
                Branches = branches
            };

            return View(model);
        }

        public async Task<IActionResult> Edit(string UserId)
        {
            if (UserId == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(UserId);

            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.ToListAsync();

            EditUserViewModel model = new EditUserViewModel()
            {
                UserId = UserId,
                UserEmail = user.Email,
                UserName = user.Name,
                UserSurname = user.Surname,
                UserPatronymic = user.Patronymic,
                UserBranchID = user.BranchID,

                AllRoles = allRoles,
                UserRoles = userRoles
            };

            ViewData["BranchID"] = new SelectList(_context.Branch, "ID", "Name", model.UserBranchID);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string UserId, [Bind("UserId,UserEmail,UserName,UserSurname,UserPatronymic,UserBranchID,UserRoles")] EditUserViewModel model)
        {
            if (UserId != model.UserId)
            {
                _logger.LogError("Error while editing user data: {0} != {1}", UserId, model.UserId);
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(UserId);

            if (user == null)
            {
                _logger.LogError("Error while editing user data: user = null");
                return NotFound();
            }

            var allRoles = await _roleManager.Roles.ToListAsync();

            if (ModelState.IsValid)
            {
                //Обновление ролей

                var userRoles = await _userManager.GetRolesAsync(user);

                var addedRoles = model.UserRoles.Except(userRoles);
                var removedRoles = userRoles.Except(model.UserRoles);

                await _userManager.AddToRolesAsync(user, addedRoles);
                await _userManager.RemoveFromRolesAsync(user, removedRoles);

                //Обновление данных профиля

                bool hasChanged = false;

                //if(model.UserEmail != user.Email)
                //{
                //    hasChanged = true;
                //    await _userManager.SetEmailAsync(user, model.UserEmail);
                //}

                if(model.UserName != user.Name)
                {
                    hasChanged = true;
                    user.Name = model.UserName;
                }

                if (model.UserSurname != user.Surname)
                {
                    hasChanged = true;
                    user.Surname = model.UserSurname;
                }

                if (model.UserPatronymic != user.Patronymic)
                {
                    hasChanged = true;
                    user.Patronymic = model.UserPatronymic;
                }

                if (model.UserBranchID != user.BranchID)
                {
                    hasChanged = true;
                    user.BranchID = model.UserBranchID;
                }

                if (hasChanged)
                {
                    try
                    {
                        await _userManager.UpdateAsync(user);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if((await _userManager.FindByIdAsync(UserId)) == null)
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            model.AllRoles = allRoles;

            ViewData["BranchID"] = new SelectList(_context.Branch, "ID", "Name", model.UserBranchID);
            return View(model);
        }

        public async Task<IActionResult> Answers(string UserId)
        {
            if (UserId == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(UserId);

            if (user == null)
            {
                return NotFound();
            }

            var surveyResults = _context.SurveyResult.Include(s => s.Survey).Where(sr => sr.UserID == UserId);

            ViewData["UserName"] = user.FullName;

            return View(surveyResults);
        }

        public async Task<IActionResult> SurveyAnswer(Guid? SurveyResultID)
        {
            if (SurveyResultID == null)
            {
                return NotFound();
            }

            SurveyResult surveyResult = await _context.SurveyResult
                .Include(sr => sr.Survey)
                .Include(sr => sr.QuestionResults)
                    .ThenInclude(qr => qr.Question)
                    .ThenInclude(q => q.Answers)
                .Include(sr => sr.QuestionResults)
                    .ThenInclude(qr => qr.QuestionResultFiles)
                .FirstOrDefaultAsync(sr => sr.ID == SurveyResultID);

            if (surveyResult == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(surveyResult.UserID);

            if (user == null)
            {
                return NotFound();
            }

            ViewData["UserName"] = user.FullName;

            return View(surveyResult);
        }

        public async Task<IActionResult> Files(string UserId)
        {
            if (UserId == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(UserId);

            if (user == null)
            {
                return NotFound();
            }

            var files = _context.QuestionResultFile.Include(f => f.QuestionResult).ThenInclude(q => q.SurveyResult).Where(f => f.QuestionResult.SurveyResult.UserID == UserId);

            ViewData["UserName"] = user.FullName;

            return View(files);
        }
    }
}
