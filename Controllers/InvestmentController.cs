using InvestFlowAPI.Data;
using InvestFlowAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvestFlowAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvestmentController : ControllerBase
    {
        private readonly InvestFlowDbContext _context;

        public InvestmentController(InvestFlowDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // GET ALL INVESTMENTS
        // GET: api/Investment
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> GetInvestments()
        {
            var investments = await _context.Investments
                .Include(i => i.User)
                .Include(i => i.InvestmentPlan)
                .ToListAsync();

            var result = investments.Select(i => new
            {
                investmentID = i.InvestmentID,
                userID = i.UserID,
                planID = i.PlanID,

                planName = i.InvestmentPlan != null
                    ? i.InvestmentPlan.PlanName
                    : "Unknown",

                amount = i.Amount,
                profitAmount = i.ProfitAmount,
                totalReturn = i.TotalReturn,

                startDate = i.StartDate,
                endDate = i.EndDate,

                status = i.Status,
                createdAt = i.CreatedAt
            });

            return Ok(result);
        }


        // =========================================================
        // GET INVESTMENTS FOR ONE USER
        // GET: api/Investment/user/1
        // =========================================================
        [HttpGet("user/{userID}")]
        public async Task<IActionResult> GetUserInvestments(int userID)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserID == userID);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            var investments = await _context.Investments
                .Where(i => i.UserID == userID)
                .Include(i => i.InvestmentPlan)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            var result = investments.Select(i => new
            {
                investmentID = i.InvestmentID,

                userID = i.UserID,

                planID = i.PlanID,

                planName = i.InvestmentPlan != null
                    ? i.InvestmentPlan.PlanName
                    : "Unknown",

                amount = i.Amount,

                profitAmount = i.ProfitAmount,

                totalReturn = i.TotalReturn,

                startDate = i.StartDate,

                endDate = i.EndDate,

                status = i.Status,

                createdAt = i.CreatedAt
            });

            return Ok(result);
        }


        // =========================================================
        // GET ONE INVESTMENT
        // GET: api/Investment/1
        // =========================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvestment(int id)
        {
            var investment = await _context.Investments
                .Include(i => i.InvestmentPlan)
                .FirstOrDefaultAsync(i =>
                    i.InvestmentID == id);

            if (investment == null)
            {
                return NotFound(new
                {
                    message = "Investment not found."
                });
            }

            return Ok(new
            {
                investmentID = investment.InvestmentID,

                userID = investment.UserID,

                planID = investment.PlanID,

                planName = investment.InvestmentPlan != null
                    ? investment.InvestmentPlan.PlanName
                    : "Unknown",

                amount = investment.Amount,

                profitAmount = investment.ProfitAmount,

                totalReturn = investment.TotalReturn,

                startDate = investment.StartDate,

                endDate = investment.EndDate,

                status = investment.Status,

                createdAt = investment.CreatedAt
            });
        }


        // =========================================================
        // CREATE INVESTMENT
        // POST: api/Investment
        // =========================================================
        [HttpPost]
        public async Task<IActionResult> CreateInvestment(
            [FromBody] InvestmentRequest request)
        {
            // =====================================================
            // VALIDATE REQUEST
            // =====================================================

            if (request == null)
            {
                return BadRequest(new
                {
                    message = "Investment request is required."
                });
            }


            // =====================================================
            // CHECK USER
            // =====================================================

            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.UserID == request.UserID);

            if (user == null)
            {
                return BadRequest(new
                {
                    message = "User not found."
                });
            }


            // =====================================================
            // CHECK INVESTMENT PLAN
            // =====================================================

            var plan = await _context.InvestmentPlans
                .FirstOrDefaultAsync(p =>
                    p.PlanID == request.PlanID &&
                    p.IsActive);

            if (plan == null)
            {
                return BadRequest(new
                {
                    message =
                        "Investment plan not found or inactive."
                });
            }


            // =====================================================
            // CHECK INVESTMENT AMOUNT
            // =====================================================

            if (request.Amount <= 0)
            {
                return BadRequest(new
                {
                    message =
                        "Investment amount must be greater than zero."
                });
            }


            // =====================================================
            // CHECK MINIMUM AMOUNT
            // =====================================================

            if (request.Amount < plan.MinimumAmount)
            {
                return BadRequest(new
                {
                    message =
                        $"Minimum investment for {plan.PlanName} is ${plan.MinimumAmount:N2}."
                });
            }


            // =====================================================
            // CHECK MAXIMUM AMOUNT
            // =====================================================

            if (
                plan.MaximumAmount.HasValue &&
                request.Amount > plan.MaximumAmount.Value
            )
            {
                return BadRequest(new
                {
                    message =
                        $"Maximum investment for {plan.PlanName} is ${plan.MaximumAmount.Value:N2}."
                });
            }


            // =====================================================
            // CHECK ACCOUNT BALANCE
            // =====================================================

            if (user.AccountBalance < request.Amount)
            {
                return BadRequest(new
                {
                    message =
                        $"Insufficient account balance. Your current balance is ${user.AccountBalance:N2}."
                });
            }


            // =====================================================
            // CALCULATE PROFIT
            // =====================================================

            decimal profitAmount =
                request.Amount *
                (plan.ProfitRate / 100m);


            // =====================================================
            // CALCULATE TOTAL RETURN
            // =====================================================

            decimal totalReturn =
                request.Amount +
                profitAmount;


            // =====================================================
            // CALCULATE DATES
            // =====================================================

            DateTime startDate =
                DateTime.UtcNow;

            DateTime endDate =
                startDate.AddDays(
                    plan.DurationDays);


            // =====================================================
            // CREATE INVESTMENT
            // =====================================================

            var investment = new Investment
            {
                UserID = request.UserID,

                PlanID = request.PlanID,

                Amount = request.Amount,

                ProfitAmount = profitAmount,

                TotalReturn = totalReturn,

                StartDate = startDate,

                EndDate = endDate,

                Status = "Active",

                CreatedAt = DateTime.UtcNow
            };


            // =====================================================
            // DEDUCT INVESTMENT FROM ACCOUNT BALANCE
            // =====================================================

            user.AccountBalance -= request.Amount;


            // =====================================================
            // SAVE BOTH CHANGES
            // =====================================================

            _context.Investments.Add(investment);

            await _context.SaveChangesAsync();


            // =====================================================
            // RETURN SUCCESS RESPONSE
            // =====================================================

            return Ok(new
            {
                message =
                    "Investment created successfully.",

                investment = new
                {
                    investmentID =
                        investment.InvestmentID,

                    userID =
                        investment.UserID,

                    planID =
                        investment.PlanID,

                    planName =
                        plan.PlanName,

                    amount =
                        investment.Amount,

                    profitAmount =
                        investment.ProfitAmount,

                    totalReturn =
                        investment.TotalReturn,

                    startDate =
                        investment.StartDate,

                    endDate =
                        investment.EndDate,

                    status =
                        investment.Status,

                    createdAt =
                        investment.CreatedAt
                },

                remainingBalance =
                    user.AccountBalance
            });
        }
    }
}