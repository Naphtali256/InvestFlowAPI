using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace InvestFlowAPI.Models
{
    public class Investment
    {
        [Key]
        public int InvestmentID { get; set; }
        // ==========================================
        // USER
        // ==========================================
        public int UserID { get; set; }
        [ForeignKey(nameof(UserID))]
        public User? User { get; set; }
        // ==========================================
        // INVESTMENT PLAN
        // ==========================================
        public int PlanID { get; set; }
        [ForeignKey(nameof(PlanID))]
        public InvestmentPlan? InvestmentPlan { get; set; }
        // ==========================================
        // INVESTMENT INFORMATION
        // ==========================================
        public decimal Amount { get; set; }
        public decimal ProfitAmount { get; set; }
        public decimal TotalReturn { get; set; }
        // ==========================================
        // DATES
        // ==========================================
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        // ==========================================
        // STATUS
        // ==========================================
        public string Status { get; set; } = "Active";
        // ==========================================
        // CREATED DATE
        // ==========================================
        public DateTime CreatedAt { get; set; }
    }
}