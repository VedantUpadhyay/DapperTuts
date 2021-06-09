using DapperTuts.Models.ViewModels;
using DapperTuts.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
            try
            {
                foreach (var op in obj)
                {
                    switch (op.Operation)
                    {
                        case INSERT:
                            await _dapperService.AddBook(op.Book);
                            break;

                        case UPDATE:
                            await _dapperService.UpdateBook(op.Book);
                            break;

                        case DELETE:
                            await _dapperService
                                .DeleteBook(op.Book.Id);
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
         

            return Ok();
        }
    }
}
