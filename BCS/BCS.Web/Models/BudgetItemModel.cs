using BCS.Commons;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BCS.Web.Models
{
    public class BudgetItemModel
    {
        public int BudgetItemId { get; set; }
        public int BudgetTabId { get; set; }
        public int CategorySettingId { get; set; }
        public int SalesCategoryRefId { get; set; }
        public string CategoryName { get; set; }
        public int ParentCategoryId { get; set; }
        public string ParentCategoryName { get; set; }
        public int SortOrder { get; set; }
        public bool IsSelected { get; set; }
        public bool IsPrimeCost { get; set; }
        public bool IsTaxCost { get; set; }
        public bool IsPercentage { get; set; }
        public string BudgetItemRow { get; set; }
        public List<BudgetItemDetail> BudgetItemList
        {
            set
            {
                StringBuilder str = new StringBuilder();
                foreach (BudgetItemDetail item in value)
                {
                    str.AppendFormat(BCSCommonData.BUDGET_DETAIL_DATA_ITEM_FORMAT, item.ActualSales, item.ActualPercent, item.ProjectionSales, item.ProjectionPercent, item.IsPercentage);
                }
                this.BudgetItemRow = str.ToString();
            }
            get
            {
                List<BudgetItemDetail> dataList = new List<BudgetItemDetail>();

                // load string to xml format
                XmlDocument reader = new XmlDocument();
                reader.LoadXml(string.Format("<temp>{0}</temp>", BudgetItemRow));
                foreach (XmlNode node in reader.DocumentElement.ChildNodes)
                {
                    dataList.Add(new BudgetItemDetail()
                    {
                        ActualSales = Convert.ToDecimal(node.ChildNodes[0].InnerText),
                        ActualPercent = Convert.ToDecimal(node.ChildNodes[1].InnerText),
                        ProjectionSales = Convert.ToDecimal(node.ChildNodes[2].InnerText),
                        ProjectionPercent = Convert.ToDecimal(node.ChildNodes[3].InnerText),
                        IsPercentage = node.ChildNodes[4].InnerText.Equals("0") ? this.IsPercentage : Convert.ToBoolean(node.ChildNodes[4].InnerText),
                    });
                }

                return dataList;
            }
        }

        public bool DeletedFlg { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int CreatedUserId { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public int UpdatedUserId { get; set; }
    }

    public class BudgetItemDetail
    {
        public decimal ActualSales { get; set; }
        public decimal ActualPercent { get; set; }
        public decimal ProjectionSales { get; set; }
        public decimal ProjectionPercent { get; set; }
        public bool IsPercentage { get; set; }
        public decimal VarianceSales
        {
            get
            {
                return ActualSales - ProjectionSales;
            }
        }
        public decimal VariancePercent
        {
            get
            {
                return (ProjectionSales == 0) ? 0 : VarianceSales * 100 / ProjectionSales;
            }
        }

        public decimal VarianceOtherSales
        {
            get
            {
                return ProjectionSales - ActualSales;
            }
        }
        public decimal VarianceOtherPercent
        {
            get
            {
                return (ProjectionSales == 0) ? 0 : VarianceOtherSales * 100 / ProjectionSales;
            }
        }
    }
}