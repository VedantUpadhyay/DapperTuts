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
        private readonly string connString;


        public HomeController(ILogger<HomeController> logger,
            IConfiguration config)
        {
            _config = config;
            _logger = logger;
            connString = _config.GetConnectionString("DefaultConnection");
        }

        public async Task<IActionResult> DeleteBook([FromRoute]int Id)
        {
            using var conn = new SqlConnection(connString);

            DynamicParameters dynamicParameters = new();

            dynamicParameters.Add("id", Id);

            var sqlCommand = @"delete from Book where Id = @id";

            int rowsAffected = await conn.ExecuteAsync(sqlCommand, dynamicParameters);

            if (rowsAffected > 0)
            {
                return Ok(rowsAffected);
            }
            return BadRequest("sorry");
        }

        [HttpPost]
        public async Task<IActionResult> IndexPost([FromForm]string bookName)
        {
            using var conn = new SqlConnection(connString);

            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("BookName", "%" + bookName + "%");

            string sqlCommand = @"SELECT [Id],
                                  [BookName]
                                  ,[AuthorName]
                                  ,[ISBN]
                              FROM[myDb].[dbo].[Book]
                              where BookName like @bookName";

            IEnumerable<Book> books = await conn.QueryAsync<Book>(sqlCommand,dynamicParams);

            return View("Index", books);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            string connString = _config.GetConnectionString("DefaultConnection");

            using var conn = new SqlConnection(connString);

            string sqlCommand = @"SELECT [Id],
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
