using System.ComponentModel.DataAnnotations;

namespace InvestFlowAPI.Models
{
    public class InvestmentPlan
    {
        [Key]
        public int PlanID { get; set; }

        public string PlanName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal ProfitRate { get; set; }

        public decimal MinimumAmount { get; set; }

        // NULL means there is no maximum investment amount
        public decimal? MaximumAmount { get; set; }

        public int DurationDays { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}