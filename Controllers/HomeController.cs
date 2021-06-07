using Dapper;
using DapperTuts.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DapperTuts.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger,
            IConfiguration config)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {

            string connString = _config.GetConnectionString("DefaultConnection");

            using var conn = new SqlConnection(connString);

            string sqlCommand = @"SELECT 
                                  [BookName]
                                  ,[AuthorName]
                                  ,[ISBN]
                              FROM[myDb].[dbo].[Book]";

            IEnumerable<Book> books = await conn.QueryAsync<Book>(sqlCommand);

            return View(books);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
