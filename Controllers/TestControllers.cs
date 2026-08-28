using InvestFlowAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvestFlowAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly InvestFlowDbContext _context;

        public TestController(InvestFlowDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                bool databaseConnected = await _context.Database.CanConnectAsync();

                if (databaseConnected)
                {
                    return Ok(new
                    {
                        message = "InvestFlow API and InvestFlowDB are connected successfully!"
                    });
                }

                return StatusCode(500, new
                {
                    message = "API is running, but cannot connect to InvestFlowDB."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Database connection failed.",
                    error = ex.Message
                });
            }
        }
    }
}