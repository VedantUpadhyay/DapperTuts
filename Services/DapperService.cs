using DapperTuts.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace DapperTuts.Services
{
    public class DapperService : IDapperService
    {
        private readonly IConfiguration _config;
        private readonly string connString;

        public DapperService(IConfiguration config)
        {
            this._config = config;
            this.connString = this._config.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> AddBook(Book book)
        {
            string sqlCommand = @"insert into Book 
                                  (BookName, AuthorName, ISBN) 
                                   values(@BookName,@AuthorName,@ISBN)";
            using var conn = new SqlConnection(connString);

            int rows = await conn.ExecuteAsync(sqlCommand,book);

            return rows > 0;

        }

        public async Task<bool> DeleteBook(int id)
        {
            using var conn = new SqlConnection(connString);

            DynamicParameters dynamicParameters = new();

            dynamicParameters.Add("id", id);

            var sqlCommand = @"delete from Book where Id = @id";

            int rows = await conn.ExecuteAsync(sqlCommand, dynamicParameters);

            return rows > 0;
        }

        public async Task<IEnumerable<Book>> GetAll()
        {
            string sqlCommand = @"select * from Book";

            using var conn = new SqlConnection(connString);

            return await conn.QueryAsync<Book>(sqlCommand);
        }

        public async Task<IEnumerable<Book>> GetByName(string bookName)
        {
            using var conn = new SqlConnection(connString);

            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("BookName", "%" + bookName + "%");

            string sqlCommand = @"SELECT [Id],
                                  [BookName]
                                  ,[AuthorName]
                                  ,[ISBN]
                              FROM [Book]
                              where BookName like @bookName";

            return await conn.QueryAsync<Book>(sqlCommand, dynamicParams);
        }

        public async Task<bool> UpdateBook(Book book)
        {
            using var conn = new SqlConnection(connString);
            string sqlCommand = @"update Book set BookName = @BookName,AuthorName = @AuthorName,ISBN = @ISBN
               where Id = @Id";

            int rows = await conn.ExecuteAsync(sqlCommand, book);

            return rows > 0;
        }

        public async Task<Book> GetById(int? id)
        {
            using var conn = new SqlConnection(connString);

            string sqlCommand = @"select * from Book
                                  where Id = @Id";

            return await conn.QueryFirstAsync<Book>(sqlCommand, new { Id = id });
        }
    }
}
