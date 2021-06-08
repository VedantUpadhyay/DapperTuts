using Dapper;
using DapperTuts.Models;
using DapperTuts.Services;
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
        private readonly IDapperService _dapperService;


        public HomeController(ILogger<HomeController> logger,
            IDapperService dapperService)
        {
            _logger = logger;
            _dapperService = dapperService;
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            Book book;
            if (id != null)
            {
                
                book = await _dapperService.GetById(id);
            }
            else
            {
                book = new();
            }
            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(Book book)
        {
            if (ModelState.IsValid)
            {
                //Update
                if (book.Id != 0)
                {
                    
                    bool done = await _dapperService.UpdateBook(book);
                    return done ? RedirectToAction(nameof(Index)) : View(book);
                }

                bool success = await _dapperService.AddBook(book);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            ModelState.AddModelError("error", "Invalid Input");
            return View(book);
        }

        public async Task<IActionResult> DeleteBook([FromRoute] int Id)
        {
            bool rowsAffected = await _dapperService.DeleteBook(Id);

            if (rowsAffected)
            {
                return RedirectToAction(nameof(Index));
            }
            return BadRequest("sorry");
        }

        [HttpPost]
        public async Task<IActionResult> GetByName([FromForm] string bookName)
        {
            IEnumerable<Book> books = await _dapperService.GetByName(bookName);

            return View("Index", books);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Book> books = await _dapperService.GetAll();

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
