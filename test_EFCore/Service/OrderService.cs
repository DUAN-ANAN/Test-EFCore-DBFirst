using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using test_EFCore.Interface;
using test_EFCore.Models;

namespace test_EFCore.Service
{
    public class OrderService : IOrderService
    {
        private readonly NorthwindContext _dbContext;

        public OrderService(NorthwindContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<List<Orders>> GetAllOrdersAsync()
        {
            return await _dbContext.Orders.AsNoTracking().ToListAsync();
        }

        /*
         * 如果可能有null回傳, 應該回傳甚麼型別?
         */
        public async Task<Orders> GetOrderByIdAsync(int OrderId)
        {
            return await _dbContext.Orders.AsNoTracking().Where(o => o.OrderId == OrderId).FirstOrDefaultAsync();
        }

        /*
         * 1. 挑選特定欄位, 要回傳什麼型別?
         * 2. 如果要判斷 Order < 0, 需回傳 BadRequest?, 還是 null? 在這裡做還是在Controller做?
         *    -> 想問的是: 會先做檢核, 是在Controller做還是Loigc層?
         *    2-1. 如果是在Logic, 那檢測到非法, 要回傳?
         *      2-1-1. 或是假設前端打API, 期望收到什麼樣的data?
         */
        public async Task<Orders> Test_FilterColumns_Async(int OrderId) //挑選特定欄位要回傳?
        {
            //return await _dbContext.Orders.AsNoTracking().Where(o => o.OrderId == OrderId).Select(o => new { o.OrderId, o.ShipCountry, o.ShipCity, o.ShipName }).FirstOrDefaultAsync();
            return await _dbContext.Orders.AsNoTracking().Where(o => o.OrderId == OrderId).FirstOrDefaultAsync();
        }

        /*
         * 1. 沒有對應實體欄位, 要回傳?
         */
        public async Task<List<Orders>> Test_Include_Async(int OrderId) 
        {

            var res = await _dbContext.Orders
                .AsNoTracking()
                .Include(c => c.OrderDetails)
                .Where(o => o.OrderId == OrderId)
                .ToListAsync();
                //.FirstOrDefaultAsync()
                

            #region T-SQL
            /*
             SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[Freight], [o].[OrderDate], [o].[RequiredDate], [o].[ShipAddress], [o].[ShipCity], [o].[ShipCountry], [o].[ShipName], [o].[ShipPostalCode], [o].[ShipRegion], [o].[ShipVia], [o].[ShippedDate], [o0].[OrderID], [o0].[ProductID], [o0].[Discount], [o0].[Quantity], [o0].[UnitPrice]
             FROM [Orders] AS [o]
             LEFT JOIN [Order Details] AS [o0] ON [o].[OrderID] = [o0].[OrderID]
             WHERE [o].[OrderID] = @__OrderId_0
             ORDER BY [o].[OrderID], [o0].[OrderID]         
             */
            #endregion

            return res;
        }

        /*
         * 1. 沒有對應實體欄位, 要回傳?
         */
        public async Task<IQueryable> Test_IncludeFilter_Async(int OrderId)
        {

            var res = await _dbContext.Orders
                 .AsNoTracking()
                 .Include(c => c.OrderDetails)
                 .Where(o => o.OrderId == OrderId)
                 .Select(o => new
                 {
                     o.OrderId,
                     Enumerable = o.OrderDetails.Select(d => new
                     {
                         d.UnitPrice,
                         d.Discount
                     })
                 })
                 .ToListAsync();

            #region T-SQL
            /*
            SELECT [o].[OrderID], [o0].[UnitPrice], [o0].[Discount], [o0].[OrderID], [o0].[ProductID]
            FROM [Orders] AS [o]
            LEFT JOIN [Order Details] AS [o0] ON [o].[OrderID] = [o0].[OrderID]
            WHERE [o].[OrderID] = @__OrderId_0
            ORDER BY [o].[OrderID], [o0].[OrderID]
           */
            #endregion

            return (IQueryable)res;
        }

        public async Task<IQueryable> Test_ThenInclude_Async(int OrderId)
        {
            /*
             * 三層關係
             * Order (1)---->(多) OrderDetails (1)---> (1)Product
             */
            var res = await _dbContext.Orders
                .AsNoTracking()
                .Include(c => c.OrderDetails)
                    .ThenInclude(c => c.Product)
                .Where(o => o.OrderId == OrderId)
                .ToListAsync();

            #region T-SQL
            /*
             SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[Freight], [o].[OrderDate], [o].[RequiredDate], [o].[ShipAddress], [o].[ShipCity], [o].[ShipCountry], [o].[ShipName], [o].[ShipPostalCode], [o].[ShipRegion], [o].[ShipVia], [o].[ShippedDate], [t].[OrderID], [t].[ProductID], [t].[Discount], [t].[Quantity], [t].[UnitPrice], [t].[ProductID0], [t].[CategoryID], [t].[Discontinued], [t].[ProductName], [t].[QuantityPerUnit], [t].[ReorderLevel], [t].[SupplierID], [t].[UnitPrice0], [t].[UnitsInStock], [t].[UnitsOnOrder]
             FROM [Orders] AS [o]
             LEFT JOIN (
                 SELECT [o0].[OrderID], [o0].[ProductID], [o0].[Discount], [o0].[Quantity], [o0].[UnitPrice], [p].[ProductID] AS [ProductID0], [p].[CategoryID], [p].[Discontinued], [p].[ProductName], [p].[QuantityPerUnit], [p].[ReorderLevel], [p].[SupplierID], [p].[UnitPrice] AS [UnitPrice0], [p].[UnitsInStock], [p].[UnitsOnOrder]
                 FROM [Order Details] AS [o0]
                 INNER JOIN [Products] AS [p] ON [o0].[ProductID] = [p].[ProductID]
             ) AS [t] ON [o].[OrderID] = [t].[OrderID]
             WHERE [o].[OrderID] = 10248
             ORDER BY [o].[OrderID], [t].[OrderID], [t].[ProductID]
            */
            #endregion

            return (IQueryable)res;
        }

        /*
         * 測不出差別= =
         * https://learn.microsoft.com/zh-tw/ef/core/querying/single-split-queries#split-queries
         */
        public async Task<IQueryable> Test_AsSpiltQuery_Async(int OrderId)
        {
            var res = await _dbContext.Orders
                .AsNoTracking()
                .Include(c => c.OrderDetails)
                .Where(o => o.OrderId == OrderId)
                .AsSplitQuery()
                .ToListAsync();

            return (IQueryable)res;
        }

        /*
         * 測不出差別= =
         * 要全域開啟?
         * https://learn.microsoft.com/zh-tw/ef/core/querying/single-split-queries#enabling-split-queries-globally
         */
        public async Task<IQueryable> Test_AsSingleQuery_Async(int OrderId)
        {
            var res = await _dbContext.Orders
                .AsNoTracking()
                .Include(c => c.OrderDetails)
                    .ThenInclude(c => c.Product)
                .AsSingleQuery()
                .Where(o => o.OrderId == OrderId)
                .ToListAsync();
                
            return (IQueryable)res;
        }

        /*
         * 1. 用RawSql, 需要回傳全部欄位? 不然會報錯!
         *    如果我寫: _dbcontext.Table
         *                  .FromRawSql("SELECT * FROM Orders WHERE OrderId = @OrderId")
         *                  .Select(o => new { o.Column1, o.Column2})
         *    所產生的 T-SQL會是:
         *    SELECT Column1, Column2 FROM (
         *      SELECT * FROM Ordes WHERE Order = 12345
         *    )
         *    但我期望的是:
         *    SELECT Column1, Column2 FROM Ordes WHERE Order = 12345
         */
        public async Task<IQueryable> Test_RawSqlScript_Async(int OrderId)
        {
            var res = await _dbContext.Orders
               .FromSqlRaw("SELECT * FROM Orders WHERE OrderId = {0}", OrderId)
               .Select(o => new
               {
                   o.OrderId,
                   o.ShipName,
                   o.ShipCountry,
                   o.ShipCity,
                   o.ShipAddress,
                   o.ShipPostalCode
               })
               .AsNoTracking()
               .ToListAsync();

            return (IQueryable)res;
        }

    }


}
