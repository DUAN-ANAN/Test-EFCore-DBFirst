using test_EFCore.Models;

namespace test_EFCore.Interface
{
    public interface IOrderService
    {
        Task<List<Orders>> GetAllOrdersAsync();
        Task<Orders> GetOrderByIdAsync(int OrderId);
        Task<Orders> Test_FilterColumns_Async(int OrderId);
        Task<List<Orders>> Test_Include_Async(int OrderId);
        Task<IQueryable> Test_IncludeFilter_Async(int OrderId);
        Task<IQueryable> Test_ThenInclude_Async(int OrderId);
        Task<IQueryable> Test_AsSpiltQuery_Async(int OrderId);
        Task<IQueryable> Test_AsSingleQuery_Async(int OrderId);
        Task<IQueryable> Test_RawSqlScript_Async(int OrderId);
    }
}
