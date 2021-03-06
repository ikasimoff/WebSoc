﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Web.Data;

namespace Web.Controllers
{
    public class AdminInstaTasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminInstaTasksController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }


        public ActionResult Tasks_Read([DataSourceRequest] DataSourceRequest request)
        {
            var data = _context.Tasks.ToDataSourceResult(request);
            return Json(data);
        }
    }
}