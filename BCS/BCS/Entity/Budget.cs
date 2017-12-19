using System;

namespace BCS.Entity
{
    public partial class Budget
    {
        public int BudgetId { get; set; }
        public string BudgetName { get; set; }
        public string RestCode { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> BudgetLengthType { get; set; }
        public Nullable<System.DateTime> BudgetLengthStart { get; set; }
        public Nullable<System.DateTime> FiscalYearStartOn { get; set; }
        public Nullable<int> BudgetLength { get; set; }
        public Nullable<bool> ActualNumbersFlg { get; set; }
        public Nullable<bool> TargetFlg { get; set; }
        public Nullable<bool> VarianceFlg { get; set; }
        public Nullable<int> InputMethodId { get; set; }
        public Nullable<decimal> Sales { get; set; }
        public Nullable<decimal> SalesPercent { get; set; }
        public Nullable<decimal> COGS { get; set; }
        public Nullable<decimal> COGSPercent { get; set; }
        public Nullable<decimal> GrossProfit { get; set; }
        public Nullable<decimal> GrossProfitPercent { get; set; }
        public Nullable<decimal> PayrollExpenses { get; set; }
        public Nullable<decimal> PayrollExpensesPercent { get; set; }
        public Nullable<decimal> OperatingProfit { get; set; }
        public Nullable<decimal> OperatingProfitPercent { get; set; }
        public Nullable<decimal> PrimeCost { get; set; }
        public Nullable<decimal> PrimeCostPercent { get; set; }
        public Nullable<decimal> OperatingExpenses { get; set; }
        public Nullable<decimal> OperatingExpensesPercent { get; set; }
        public Nullable<decimal> NetProfitLoss { get; set; }
        public Nullable<decimal> NetProfitLossPercent { get; set; }
        public Nullable<decimal> BreakEvenPoint { get; set; }
        public Nullable<decimal> BreakEvenPointPercent { get; set; }
        public Nullable<bool> DeletedFlg { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        // Edit flag
        public bool EditFlg { get; set; }
        public string CreatedUserName { get; set; }
        public string UpdatedUserName { get; set; }
        public string CommonDataText { get; set; }
        public string CommonDataCode { get; set; }
    }
}
