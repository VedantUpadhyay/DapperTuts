using DapperTuts.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DapperTuts.Services
{
    public interface IDapperService
    {

        Task<IEnumerable<Book>> GetByName(string bookName);

        Task<IEnumerable<Book>> GetAll();

        Task<bool> DeleteBook(int id);

        Task<bool> AddBook(Book book);

        Task<bool> UpdateBook(Book book);

        Task<Book> GetById(int? id);

        Task<int> GetCurrentIdent();

        Task<bool> SaveBooks(List<Book> insertRows, List<Book> updateRows, List<Book> deleteRows);
    }
}
