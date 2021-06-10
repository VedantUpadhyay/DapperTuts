using DapperTuts.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using System.Reflection;

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

        public async Task<int> GetCurrentIdent()
        {
            using var conn = new SqlConnection(connString);

            string sqlCommand = @"select IDENT_CURRENT('Book')";

            return await conn.ExecuteScalarAsync<int>(sqlCommand);
        }

        public async Task<Book> GetById(int? id)
        {
            using var conn = new SqlConnection(connString);

            string sqlCommand = @"select * from Book
                                  where Id = @Id";

            return await conn.QueryFirstAsync<Book>(sqlCommand, new { Id = id });
        }

        public async Task<bool> SaveBooks(List<Book> insertRows, List<Book> updateRows, List<Book> deleteRows)
        {
            using var conn = new SqlConnection(connString);

            await conn.OpenAsync();
            var trans = await conn.BeginTransactionAsync();

            string insertSql = @"insert into Book 
                              (BookName, AuthorName, ISBN) 
                               values(@BookName,@AuthorName,@ISBN)";

            string updateSql = @"update Book set BookName =                          @BookName,AuthorName = @AuthorName,ISBN = @ISBN
                          where Id = @Id";

            string deleteSql = @"delete from Book where Id = @Id";

            int rows = 0;
            try
            {

                rows = await conn.ExecuteAsync(insertSql, insertRows,trans);

                rows += await conn.ExecuteAsync(updateSql, updateRows, trans);

                rows += await conn.ExecuteAsync(deleteSql, deleteRows, trans);

                //commit all operations only when all ops executes successfully.
                await trans.CommitAsync();
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                await conn.CloseAsync();
            }

            return rows > 0;
            
        }  

        public async Task<bool> BullkCrudUsingSP(IEnumerable<Book> books)
        {
            using var conn = new SqlConnection(connString);

            string sp_name = "[dbo].[bulkdcrud_old]";
            //string sp_name = "[dbo].[bulkcrud]";


            int rows = await conn.ExecuteAsync(sp_name, new {
                bookView = books.ToList().ToDataTable()
            }, commandType: CommandType.StoredProcedure);
            return rows > 0;
        }

       
    }
    static class DataTableExtension
    {
        public static DataTable ToDataTable<T>(this List<T> items)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                tb.Columns.Add(prop.Name, prop.PropertyType);
            }

            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                tb.Rows.Add(values);
            }

            return tb;
        }
    }
}
