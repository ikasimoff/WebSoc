using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web.Data;
using Web.Models;

namespace Web.Controllers
{
    public class AdminAccountsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _rolesManager;
        private readonly IEmailSender _emailSender;

        public AdminAccountsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> rolesManager,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _rolesManager = rolesManager;
        }



        public IActionResult Index()
        {
            ViewBag.Roles = _context.Roles;
            return View();
        }

        public ActionResult Accounts_Read([DataSourceRequest] DataSourceRequest request)
        {
            var data = _context.Users.ToDataSourceResult(request);
            return Json(data);
        }
               
        public IActionResult Edit(string id)
        {
            ViewBag.Roles = _context.Roles;
            ViewBag.UserRoles = _context.UserRoles.Where(i => i.UserId.Equals(id, StringComparison.CurrentCultureIgnoreCase));

            return View(_context.Users.First(i=>i.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase)));
        }
    }
}