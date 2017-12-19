﻿using System;

namespace BCS.Entity
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<bool> IsSelected { get; set; }
        public Nullable<bool> IsPrimeCost { get; set; }
        public Nullable<bool> IsTaxCost { get; set; }
        public Nullable<bool> IsPercentage { get; set; }
        public Nullable<int> ParentCategoryId { get; set; }
        public Nullable<bool> DeletedFlg { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
    }
}
