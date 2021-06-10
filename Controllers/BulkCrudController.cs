using DapperTuts.Models;
using DapperTuts.Models.ViewModels;
using DapperTuts.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DapperTuts.Controllers
{
    public class BulkCrudController : Controller
    {
        private readonly IDapperService _dapperService;

        private const string INSERT = "INSERT";
        private const string UPDATE = "UPDATE";
        private const string DELETE = "DELETE";

        public BulkCrudController(IDapperService dapperService)
        {
            _dapperService = dapperService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _dapperService.GetAll());
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentIdent()
        {
            try
            {
                int currentId = await _dapperService.GetCurrentIdent();

                return Json(new
                {
                    currentId = currentId
                });
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<IActionResult> SaveDatabase(List<OperationVM> obj)
        {
            bool result = false;
            try
            {
                List<Book> insertRows = obj.Where(o => o.Operation == INSERT).Select(s => s.Book).ToList();

                List<Book> updateRows = obj.Where(o => o.Operation == UPDATE).Select(s => s.Book).ToList();

                List<Book> deleteRows = obj.Where(o => o.Operation == DELETE).Select(s => s.Book).ToList();

                result = await _dapperService.SaveBooks(insertRows, updateRows, deleteRows);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
         

            return result ? StatusCode(200) : StatusCode(500);
        }
    }
}
