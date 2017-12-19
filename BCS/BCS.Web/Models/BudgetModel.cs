
namespace BCS.Web.Models
{
    public class BudgetModel
    {
        public int BudgetId { get; set; }
        public string BudgetName { get; set; }
        public string RestCode { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public int BudgetLengthType { get; set; }
        public System.DateTime BudgetLengthStart { get; set; }
        public System.DateTime FiscalYearStartOn { get; set; }
        public int BudgetLength { get; set; }
        public bool ActualNumbersFlg { get; set; }
        public bool TargetFlg { get; set; }
        public bool VarianceFlg { get; set; }
        public int InputMethodId { get; set; }
        public decimal Sales { get; set; }
        public decimal SalesPercent { get; set; }
        public decimal COGS { get; set; }
        public decimal COGSPercent { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal GrossProfitPercent { get; set; }
        public decimal PayrollExpenses { get; set; }
        public decimal PayrollExpensesPercent { get; set; }
        public decimal OperatingProfit { get; set; }
        public decimal OperatingProfitPercent { get; set; }
        public decimal PrimeCost { get; set; }
        public decimal PrimeCostPercent { get; set; }
        public decimal OperatingExpenses { get; set; }
        public decimal OperatingExpensesPercent { get; set; }
        public decimal NetProfitLoss { get; set; }
        public decimal NetProfitLossPercent { get; set; }
        public decimal BreakEvenPoint { get; set; }
        public decimal BreakEvenPointPercent { get; set; }
        public bool DeletedFlg { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int CreatedUserId { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public int UpdatedUserId { get; set; }

        // Edit flag
        public bool EditFlg { get; set; }
        public string CreatedUserName { get; set; }
        public string UpdatedUserName { get; set; }
        public string CommonDataText { get; set; }
        public string CommonDataCode { get; set; }

        public string BudgetNameDisplay {
            get { return this.BudgetName.Length > 35 ? this.BudgetName.Substring(0, 35) + " ..." : this.BudgetName; }
        }

        public System.Collections.Generic.List<CategorySettingModel> CategorySettingModelList { get; set; }
        public System.Collections.Generic.List<BudgetTabModel> BudgetTabModelList { get; set; }

        public string Section { get; set; }
        public string BudgetTabIndex { get; set; }
    }
}