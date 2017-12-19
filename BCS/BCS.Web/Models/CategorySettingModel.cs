using System.ComponentModel.DataAnnotations;

namespace BCS.Web.Models
{
    public class CategorySettingModel
    {
        public int CategorySettingId { get; set; }
        public int BudgetId { get; set; }
        public int ParentCategoryId { get; set; }
        public int SalesCategoryRefId { get; set; }
        [StringLength(255)]
        [Required(ErrorMessage = "The Category Name field is required.")]
        public string CategoryName { get; set; }
        public int SortOrder { get; set; }
        public bool IsSelected { get; set; }
        public bool IsPrimeCost { get; set; }
        public bool IsTaxCost { get; set; }
        public bool IsPercentage { get; set; }
        public bool DeletedFlg { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int CreatedUserId { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public int UpdatedUserId { get; set; }
    }
}