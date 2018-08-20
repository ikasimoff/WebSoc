using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Web.Data;

namespace Web.Controllers
{
    public class AdminInstaLoginsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminInstaLoginsController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }


        public ActionResult Logins_Read([DataSourceRequest] DataSourceRequest request)
        {
            var data = _context.Logins.ToDataSourceResult(request);
            return Json(data);
        }
    }
}