using InvestFlowAPI.Data;
using InvestFlowAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvestFlowAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvestmentPlanController : ControllerBase
    {
        private readonly InvestFlowDbContext _context;

        public InvestmentPlanController(InvestFlowDbContext context)
        {
            _context = context;
        }

        // GET: api/investmentplan
        [HttpGet]
        public async Task<IActionResult> GetInvestmentPlans()
        {
            var plans = await _context.InvestmentPlans
                .Where(p => p.IsActive)
                .OrderBy(p => p.MinimumAmount)
                .ToListAsync();

            return Ok(plans);
        }

        // GET: api/investmentplan/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvestmentPlan(int id)
        {
            var plan = await _context.InvestmentPlans
                .FirstOrDefaultAsync(p => p.PlanID == id);

            if (plan == null)
            {
                return NotFound(new
                {
                    message = "Investment plan not found."
                });
            }

            return Ok(plan);
        }

        // POST: api/investmentplan
        [HttpPost]
        public async Task<IActionResult> CreateInvestmentPlan(
            InvestmentPlan plan)
        {
            if (string.IsNullOrWhiteSpace(plan.PlanName))
            {
                return BadRequest(new
                {
                    message = "Plan name is required."
                });
            }

            if (plan.MinimumAmount <= 0)
            {
                return BadRequest(new
                {
                    message = "Minimum amount must be greater than zero."
                });
            }

            if (plan.MaximumAmount.HasValue &&
                plan.MaximumAmount.Value < plan.MinimumAmount)
            {
                return BadRequest(new
                {
                    message = "Maximum amount cannot be less than minimum amount."
                });
            }

            if (plan.ProfitRate < 0)
            {
                return BadRequest(new
                {
                    message = "Profit rate cannot be negative."
                });
            }

            if (plan.DurationDays <= 0)
            {
                return BadRequest(new
                {
                    message = "Duration must be greater than zero."
                });
            }

            plan.CreatedAt = DateTime.UtcNow;
            plan.IsActive = true;

            _context.InvestmentPlans.Add(plan);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Investment plan created successfully.",
                planID = plan.PlanID
            });
        }
    }
}