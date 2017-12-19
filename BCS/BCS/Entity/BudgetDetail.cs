using System;

namespace BCS.Entity
{
    public class BudgetTab
    {
        public int BudgetTabId { get; set; }
        public int BudgetId { get; set; }
        public int TabIndex { get; set; }
        public string TabName { get; set; }
        public decimal AnnualSales { get; set; }
        public string HeaderColumn { get; set; }
        public string TargetColumn { get; set; }
        public Nullable<bool> DeletedFlg { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
    }

    public class BudgetItem
    {
        public int BudgetItemId { get; set; }
        public int BudgetTabId { get; set; }
        public int CategorySettingId { get; set; }
        public string CategoryName { get; set; }
        public int SalesCategoryRefId { get; set; }
        public int ParentCategoryId { get; set; }
        public string ParentCategoryName { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<bool> IsSelected { get; set; }
        public Nullable<bool> IsPrimeCost { get; set; }
        public Nullable<bool> IsTaxCost { get; set; }
        public Nullable<bool> IsPercentage { get; set; }
        public string BudgetItemRow { get; set; }
        public Nullable<bool> DeletedFlg { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
    }
}
