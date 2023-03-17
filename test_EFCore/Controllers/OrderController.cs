using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using test_EFCore.Interface;
using test_EFCore.Models;
using test_EFCore.Service;

namespace test_EFCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<Orders>>> GetAllOrdersAsync()
        {
            var res = await _orderService.GetAllOrdersAsync();
            return Ok(res);
        }

        [HttpGet("[action]/{OrderID}")]
        public async Task<ActionResult<Orders>> GetOrderByIdAsync(int OrderId)
        {
            var res = await _orderService.GetOrderByIdAsync(OrderId);
            return Ok(res);
        }

        [HttpGet("[action]/{OrderID}")]
        public async Task<ActionResult> Test_FilterColumns_Async(int OrderId)
        {
            if (OrderId < 1) { return BadRequest(); } // 在這裡做?
            var res = await _orderService.Test_FilterColumns_Async(OrderId);
            return Ok(res);
        }

        [HttpGet("[action]/{OrderId}")]
        public async Task<ActionResult> Test_Include_Async(int OrderId)
        {
            var res = await _orderService.Test_Include_Async(OrderId);
            return Ok(res);
        }

        [HttpGet("[action]/{OrderId}")]
        public async Task<ActionResult> Test_IncludeFilter_Async(int OrderId)
        {
            var res = await _orderService.Test_IncludeFilter_Async(OrderId);
            return Ok(res);
        }

        [HttpGet("[action]/{OrderId}")]
        public async Task<ActionResult> Test_ThenInclude_Async(int OrderId)
        {
            var res = await _orderService.Test_ThenInclude_Async(OrderId);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> Test_AsSpiltQuery_Async(int OrderId)
        {
            var res = await _orderService.Test_AsSpiltQuery_Async(OrderId);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> Test_AsSingleQuery_Async(int OrderId)
        {
            var res = await _orderService.Test_AsSingleQuery_Async(OrderId);
            return Ok(res);
        }

        [HttpGet("[action]/{OrderId}")]
        public async Task<ActionResult> Test_RawSqlScript_Async(int OrderId)
        {
            var res = await _orderService.Test_RawSqlScript_Async(OrderId);
            return Ok(res);
        }
    }
}
