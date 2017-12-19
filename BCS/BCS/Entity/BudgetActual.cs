
namespace BCS.Entity
{
    public class BudgetActual
    {
        public string CategoryName { get; set; }
        public string RelatedTo { get; set; }
        public string LastINVDate { get; set; }
        public decimal ActualSales { get; set; }
        public decimal Inventory { get; set; }
        public decimal EstimatedUsage { get; set; }
        public decimal Purchases { get; set; }
        public decimal ActualCogs { get; set; }
    }
}
