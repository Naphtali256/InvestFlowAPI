namespace InvestFlowAPI.Models
{
    public class InvestmentRequest
    {
        public int UserID { get; set; }
        public int PlanID { get; set; }
        public decimal Amount { get; set; }
    }
}