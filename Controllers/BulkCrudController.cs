using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DapperTuts.Controllers
{
    public class BulkCrudController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
