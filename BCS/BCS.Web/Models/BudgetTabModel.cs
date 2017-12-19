using BCS.Commons;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;
using System.Data;

namespace BCS.Web.Models
{
    public class BudgetTabModel
    {
        public int BudgetTabId { get; set; }
        public int BudgetId { get; set; }
        public int TabIndex { get; set; }
        public string TabName { get; set; }
        public decimal AnnualSales { get; set; }
        public string HeaderColumn { get; set; }
        public List<string> HeaderColumnList
        {
            set
            {
                StringBuilder str = new StringBuilder();
                foreach (string item in value)
                {
                    str.AppendFormat(BCSCommonData.BUDGET_DETAIL_HEADER_ITEM_FORMAT, item);
                }
                this.HeaderColumn = str.ToString();
            }
            get
            {
                List<string> headerList = new List<string>();

                // load string to xml format
                XmlDocument reader = new XmlDocument();
                reader.LoadXml(string.Format("<temp>{0}</temp>", HeaderColumn));
                foreach (XmlNode node in reader.DocumentElement.ChildNodes)
                {
                    headerList.Add(node.ChildNodes[0].InnerText);
                }

                return headerList;
            }
        }
        public string TargetColumn { get; set; }
        public List<TargetItemDetail> TargetColumnList
        {
            set
            {
                StringBuilder str = new StringBuilder();
                foreach (TargetItemDetail item in value)
                {
                    str.AppendFormat(BCSCommonData.BUDGET_DETAIL_HEADER_TARGET_FORMAT, item.TargetSales, item.TargetPercent);
                }
                this.TargetColumn = str.ToString();
            }
            get
            {
                List<TargetItemDetail> dataList = new List<TargetItemDetail>();

                // load string to xml format
                XmlDocument reader = new XmlDocument();
                reader.LoadXml(string.Format("<temp>{0}</temp>", TargetColumn));
                foreach (XmlNode node in reader.DocumentElement.ChildNodes)
                {
                    dataList.Add(new TargetItemDetail()
                    {
                        TargetSales = Convert.ToDecimal(node.ChildNodes[0].InnerText),
                        TargetPercent = Convert.ToDecimal(node.ChildNodes[1].InnerText),
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

        public List<BudgetItemModel> BudgetItemModelList { get; set; }

        public List<BudgetItemDetail> TotalRow(int parentCategoryId)
        {
            List<BudgetItemDetail> list = new List<BudgetItemDetail>();
            var categoryList = this.BudgetItemModelList.Where(s => s.ParentCategoryId == parentCategoryId).ToList<BudgetItemModel>();
            for (int i = 0; i < this.HeaderColumnList.Count; i++)
            {
                BudgetItemDetail budgetItem = new BudgetItemDetail();
                foreach (var categoryRow in categoryList)
                {
                    budgetItem.ActualSales += categoryRow.BudgetItemList[i].ActualSales;
                    budgetItem.ProjectionSales += categoryRow.BudgetItemList[i].ProjectionSales;
                    budgetItem.ProjectionPercent += categoryRow.BudgetItemList[i].ProjectionPercent;
                }
                budgetItem.ActualPercent = (budgetItem.ProjectionSales == 0) ? 0 : (budgetItem.ActualSales * 100 / budgetItem.ProjectionSales);
                list.Add(budgetItem);
            }

            return list;
        }

        public List<BudgetItemDetail> TotalRowIsNotTax(int parentCategoryId)
        {
            List<BudgetItemDetail> list = new List<BudgetItemDetail>();
            var categoryList = this.BudgetItemModelList.Where(s => s.ParentCategoryId == parentCategoryId && s.IsTaxCost == false).ToList<BudgetItemModel>();
            for (int i = 0; i < this.HeaderColumnList.Count; i++)
            {
                BudgetItemDetail budgetItem = new BudgetItemDetail();
                foreach (var categoryRow in categoryList)
                {
                    budgetItem.ActualSales += categoryRow.BudgetItemList[i].ActualSales;
                    budgetItem.ProjectionSales += categoryRow.BudgetItemList[i].ProjectionSales;
                    budgetItem.ProjectionPercent += categoryRow.BudgetItemList[i].ProjectionPercent;
                }
                budgetItem.ActualPercent = (budgetItem.ProjectionSales == 0) ? 0 : (budgetItem.ActualSales * 100 / budgetItem.ProjectionSales);
                list.Add(budgetItem);
            }

            return list;
        }

        public List<BudgetItemDetail> TotalRowIsTax(int parentCategoryId)
        {
            List<BudgetItemDetail> list = new List<BudgetItemDetail>();
            var categoryList = this.BudgetItemModelList.Where(s => s.ParentCategoryId == parentCategoryId && s.IsTaxCost == true).ToList<BudgetItemModel>();
            for (int i = 0; i < this.HeaderColumnList.Count; i++)
            {
                BudgetItemDetail budgetItem = new BudgetItemDetail();
                foreach (var categoryRow in categoryList)
                {
                    budgetItem.ActualSales += categoryRow.BudgetItemList[i].ActualSales;
                    budgetItem.ProjectionSales += categoryRow.BudgetItemList[i].ProjectionSales;
                    budgetItem.ProjectionPercent += categoryRow.BudgetItemList[i].ProjectionPercent;
                }
                budgetItem.ActualPercent = (budgetItem.ProjectionSales == 0) ? 0 : (budgetItem.ActualSales * 100 / budgetItem.ProjectionSales);
                list.Add(budgetItem);
            }

            return list;
        }

        public List<BudgetItemDetail> SalesTotal { get; set; }
        public DataTable SalesDataTable
        {
            get
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("BudgetItemId");
                dataTable.Columns.Add("CategorySettingId");
                dataTable.Columns.Add("CategoryName");
                dataTable.Columns.Add("ParentCategoryId");
                dataTable.Columns.Add("ParentCategoryName");

                SalesTotal = new List<BudgetItemDetail>();

                // add column name to table
                foreach (string header in this.HeaderColumnList)
                {
                    dataTable.Columns.Add(header + "_IsPercentage");
                    dataTable.Columns.Add(header + "_ProjectionSales");
                    dataTable.Columns.Add(header + "_ProjectionPercent");
                    dataTable.Columns.Add(header + "_ActualSales");
                    dataTable.Columns.Add(header + "_ActualPercent");
                    dataTable.Columns.Add(header + "_VarianceSales");
                    dataTable.Columns.Add(header + "_VariancePercent");

                    SalesTotal.Add(new BudgetItemDetail());
                }

                // add grand total column to table
                dataTable.Columns.Add("GrandTotal_ProjectionSales");
                dataTable.Columns.Add("GrandTotal_ProjectionPercent");
                dataTable.Columns.Add("GrandTotal_ActualSales");
                dataTable.Columns.Add("GrandTotal_ActualPercent");
                dataTable.Columns.Add("GrandTotal_VarianceSales");
                dataTable.Columns.Add("GrandTotal_VariancePercent");

                // add data to table
                var dataByParentCategoryId = this.BudgetItemModelList.Where(s => s.ParentCategoryName == BCSCommonData.CATEGORY_DEFAULT_SALES_TEXT);
                foreach (BudgetItemModel budgetItem in dataByParentCategoryId)
                {
                    DataRow newRow = dataTable.NewRow();
                    newRow["BudgetItemId"] = budgetItem.BudgetItemId;
                    newRow["CategorySettingId"] = budgetItem.CategorySettingId;
                    newRow["CategoryName"] = budgetItem.CategoryName;
                    newRow["ParentCategoryId"] = budgetItem.ParentCategoryId;
                    newRow["ParentCategoryName"] = budgetItem.ParentCategoryName;

                    decimal totalProjectionSales = 0;
                    decimal totalActualSales = 0;
                    for (int i = 0; i < this.HeaderColumnList.Count; i++)
                    {
                        newRow[this.HeaderColumnList[i] + "_IsPercentage"] = budgetItem.BudgetItemList[i].IsPercentage;

                        decimal sales = 0, percent = 0;
                        if (budgetItem.BudgetItemList[i].IsPercentage)
                        {
                            sales = this.TargetColumnList[i].TargetSales == 0 ? 0 : budgetItem.BudgetItemList[i].ProjectionPercent * this.TargetColumnList[i].TargetSales / 100;
                            percent = budgetItem.BudgetItemList[i].ProjectionPercent;
                        }
                        else
                        {
                            sales = budgetItem.BudgetItemList[i].ProjectionSales;
                            percent = this.TargetColumnList[i].TargetSales == 0 ? 0 : budgetItem.BudgetItemList[i].ProjectionSales * 100 / this.TargetColumnList[i].TargetSales;
                        }
                        newRow[this.HeaderColumnList[i] + "_ProjectionSales"] = sales;
                        newRow[this.HeaderColumnList[i] + "_ProjectionPercent"] = percent;
                        SalesTotal[i].ProjectionSales += sales;
                        SalesTotal[i].ProjectionPercent += percent;
                        totalProjectionSales += sales;

                        decimal actual = budgetItem.BudgetItemList[i].ActualSales;
                        newRow[this.HeaderColumnList[i] + "_ActualSales"] = actual;
                        newRow[this.HeaderColumnList[i] + "_ActualPercent"] = budgetItem.BudgetItemList[i].ActualPercent;
                        SalesTotal[i].ActualSales += actual;
                        totalActualSales += actual;

                        newRow[this.HeaderColumnList[i] + "_VarianceSales"] = actual - sales;
                        newRow[this.HeaderColumnList[i] + "_VariancePercent"] = sales == 0 ? 0 : (actual - sales) * 100 / sales;
                    }

                    var variance = totalActualSales - totalProjectionSales;
                    newRow["GrandTotal_ProjectionSales"] = totalProjectionSales;
                    newRow["GrandTotal_ProjectionPercent"] = 0;
                    newRow["GrandTotal_ActualSales"] = totalActualSales;
                    newRow["GrandTotal_ActualPercent"] = totalProjectionSales == 0 ? 0 : totalActualSales * 100 / totalProjectionSales;
                    newRow["GrandTotal_VarianceSales"] = variance;
                    newRow["GrandTotal_VariancePercent"] = totalProjectionSales == 0 ? 0 : variance * 100 / totalProjectionSales;

                    dataTable.Rows.Add(newRow);
                }

                // reCalculate actual percent
                foreach (var item in SalesTotal)
                {
                    item.ActualPercent = item.ProjectionSales == 0 ? 0 : item.ActualSales * 100 / item.ProjectionSales;
                }
                foreach (DataRow row in dataTable.Rows)
                {
                    for (int i = 0; i < this.HeaderColumnList.Count; i++)
                    {
                        row[this.HeaderColumnList[i] + "_ActualPercent"] = SalesTotal[i].ActualSales == 0 ? 0 : Convert.ToDecimal(row[this.HeaderColumnList[i] + "_ActualSales"]) * 100 / SalesTotal[i].ActualSales;
                    }
                }

                return dataTable;
            }
        }

        public List<decimal> FixCostList { get; set; }
        public List<decimal> VariableCostList { get; set; }
        public List<BudgetItemDetail> CogsTotal { get; set; }
        public DataTable CogsDataTable
        {
            get
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("BudgetItemId");
                dataTable.Columns.Add("CategorySettingId");
                dataTable.Columns.Add("CategoryName");
                dataTable.Columns.Add("ParentCategoryId");
                dataTable.Columns.Add("SalesCategoryRefId");
                dataTable.Columns.Add("ParentCategoryName");

                CogsTotal = new List<BudgetItemDetail>();
                FixCostList = new List<decimal>();
                VariableCostList = new List<decimal>();

                // add column name to table
                foreach (string header in this.HeaderColumnList)
                {
                    dataTable.Columns.Add(header + "_IsPercentage");
                    dataTable.Columns.Add(header + "_ProjectionSales");
                    dataTable.Columns.Add(header + "_ProjectionPercent");
                    dataTable.Columns.Add(header + "_ActualSales");
                    dataTable.Columns.Add(header + "_ActualPercent");
                    dataTable.Columns.Add(header + "_VarianceSales");
                    dataTable.Columns.Add(header + "_VariancePercent");

                    CogsTotal.Add(new BudgetItemDetail());
                    FixCostList.Add(0);
                    VariableCostList.Add(0);
                }

                // add grand total column to table
                dataTable.Columns.Add("GrandTotal_ProjectionSales");
                dataTable.Columns.Add("GrandTotal_ProjectionPercent");
                dataTable.Columns.Add("GrandTotal_ActualSales");
                dataTable.Columns.Add("GrandTotal_ActualPercent");
                dataTable.Columns.Add("GrandTotal_VarianceSales");
                dataTable.Columns.Add("GrandTotal_VariancePercent");

                // data by sales
                var dataBySales = this.SalesDataTable;

                // add data to table
                var dataByParentCategoryId = this.BudgetItemModelList.Where(s => s.ParentCategoryName == BCSCommonData.CATEGORY_DEFAULT_COGS_TEXT);
                foreach (BudgetItemModel budgetItem in dataByParentCategoryId)
                {
                    DataRow newRow = dataTable.NewRow();
                    newRow["BudgetItemId"] = budgetItem.BudgetItemId;
                    newRow["CategorySettingId"] = budgetItem.CategorySettingId;
                    newRow["CategoryName"] = budgetItem.CategoryName;
                    newRow["ParentCategoryId"] = budgetItem.ParentCategoryId;
                    newRow["SalesCategoryRefId"] = budgetItem.SalesCategoryRefId;
                    newRow["ParentCategoryName"] = budgetItem.ParentCategoryName;

                    // get data mapping from sales section
                    var dataMappingFromSales = dataBySales.Select("CategorySettingId = " + budgetItem.SalesCategoryRefId).FirstOrDefault();

                    decimal totalProjectionSales = 0;
                    decimal totalActualSales = 0;
                    for (int i = 0; i < this.HeaderColumnList.Count; i++)
                    {
                        // get actual sales of sales section reference
                        decimal actualRefSalesSection = dataMappingFromSales == null || budgetItem.SalesCategoryRefId == 0 ? this.SalesTotal[i].ActualSales : Convert.ToDecimal(dataMappingFromSales[this.HeaderColumnList[i] + "_ActualSales"]);

                        newRow[this.HeaderColumnList[i] + "_IsPercentage"] = budgetItem.BudgetItemList[i].IsPercentage;

                        decimal sales = 0, percent = 0;
                        if (budgetItem.BudgetItemList[i].IsPercentage)
                        {
                            percent = budgetItem.BudgetItemList[i].ProjectionPercent;
                            sales = actualRefSalesSection * percent / 100;
                            VariableCostList[i] += sales;
                        }
                        else
                        {
                            sales = budgetItem.BudgetItemList[i].ProjectionSales;
                            percent = actualRefSalesSection == 0 ? 0 : sales * 100 / actualRefSalesSection;
                            FixCostList[i] += sales;
                        }
                        newRow[this.HeaderColumnList[i] + "_ProjectionSales"] = sales;
                        newRow[this.HeaderColumnList[i] + "_ProjectionPercent"] = percent;
                        CogsTotal[i].ProjectionSales += sales;
                        totalProjectionSales += sales;

                        decimal actual = budgetItem.BudgetItemList[i].ActualSales;
                        newRow[this.HeaderColumnList[i] + "_ActualSales"] = actual;
                        newRow[this.HeaderColumnList[i] + "_ActualPercent"] = actualRefSalesSection == 0 ? 0 : budgetItem.BudgetItemList[i].ActualSales * 100 / actualRefSalesSection;
                        CogsTotal[i].ActualSales += actual;
                        totalActualSales += actual;

                        newRow[this.HeaderColumnList[i] + "_VarianceSales"] = sales - actual;
                        newRow[this.HeaderColumnList[i] + "_VariancePercent"] = actualRefSalesSection == 0 ? 0 : (sales - actual) * 100 / actualRefSalesSection;
                    }

                    // set grand total by row
                    newRow["GrandTotal_ProjectionSales"] = totalProjectionSales;
                    var totalSalesMapping = dataMappingFromSales == null || budgetItem.SalesCategoryRefId == 0 ? this.SalesTotal.Sum(s => s.ActualSales) : Convert.ToDecimal(dataMappingFromSales["GrandTotal_ActualSales"]);
                    newRow["GrandTotal_ProjectionPercent"] = totalSalesMapping == 0 ? 0 : totalProjectionSales * 100 / totalSalesMapping;
                    newRow["GrandTotal_ActualSales"] = totalActualSales;
                    newRow["GrandTotal_ActualPercent"] = totalSalesMapping == 0 ? 0 : totalActualSales * 100 / totalSalesMapping;
                    var variance = totalProjectionSales - totalActualSales;
                    newRow["GrandTotal_VarianceSales"] = variance;
                    newRow["GrandTotal_VariancePercent"] = totalSalesMapping == 0 ? 0 : variance * 100 / totalSalesMapping;

                    dataTable.Rows.Add(newRow);
                }

                // reCalculate actual percent
                foreach (var item in CogsTotal)
                {
                    var index = CogsTotal.IndexOf(item);
                    var actualSales = this.SalesTotal[index].ActualSales;
                    item.ActualPercent = actualSales == 0 ? 0 : item.ActualSales * 100 / actualSales;
                    item.ProjectionPercent = actualSales == 0 ? 0 : item.ProjectionSales * 100 / actualSales;
                }

                return dataTable;
            }
        }

        public List<BudgetItemDetail> PrimeCostTotal { get; set; }
        public List<BudgetItemDetail> PayrollTotal { get; set; }
        public DataTable PayrollDataTable
        {
            get
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("BudgetItemId");
                dataTable.Columns.Add("CategorySettingId");
                dataTable.Columns.Add("CategoryName");
                dataTable.Columns.Add("ParentCategoryId");
                dataTable.Columns.Add("ParentCategoryName");

                PayrollTotal = new List<BudgetItemDetail>();
                PrimeCostTotal = new List<BudgetItemDetail>();

                // add column name to table
                foreach (string header in this.HeaderColumnList)
                {
                    dataTable.Columns.Add(header + "_IsPercentage");
                    dataTable.Columns.Add(header + "_ProjectionSales");
                    dataTable.Columns.Add(header + "_ProjectionPercent");
                    dataTable.Columns.Add(header + "_ActualSales");
                    dataTable.Columns.Add(header + "_ActualPercent");
                    dataTable.Columns.Add(header + "_VarianceSales");
                    dataTable.Columns.Add(header + "_VariancePercent");

                    PayrollTotal.Add(new BudgetItemDetail());
                    PrimeCostTotal.Add(new BudgetItemDetail());
                }

                // add grand total column to table
                dataTable.Columns.Add("GrandTotal_ProjectionSales");
                dataTable.Columns.Add("GrandTotal_ProjectionPercent");
                dataTable.Columns.Add("GrandTotal_ActualSales");
                dataTable.Columns.Add("GrandTotal_ActualPercent");
                dataTable.Columns.Add("GrandTotal_VarianceSales");
                dataTable.Columns.Add("GrandTotal_VariancePercent");

                // add data to table: is not tax
                var dataByParentCategoryId = this.BudgetItemModelList.Where(s => s.ParentCategoryName == BCSCommonData.CATEGORY_DEFAULT_PAYROLL_EXPENSES_TEXT && s.IsTaxCost == false);
                foreach (BudgetItemModel budgetItem in dataByParentCategoryId)
                {
                    DataRow newRow = dataTable.NewRow();
                    newRow["BudgetItemId"] = budgetItem.BudgetItemId;
                    newRow["CategorySettingId"] = budgetItem.CategorySettingId;
                    newRow["CategoryName"] = budgetItem.CategoryName;
                    newRow["ParentCategoryId"] = budgetItem.ParentCategoryId;
                    newRow["ParentCategoryName"] = budgetItem.ParentCategoryName;

                    decimal totalProjectionSales = 0;
                    decimal totalActualSales = 0;
                    for (int i = 0; i < this.HeaderColumnList.Count; i++)
                    {
                        newRow[this.HeaderColumnList[i] + "_IsPercentage"] = budgetItem.BudgetItemList[i].IsPercentage;

                        decimal sales = 0, percent = 0;
                        if (budgetItem.BudgetItemList[i].IsPercentage)
                        {
                            sales = this.TargetColumnList[i].TargetSales * budgetItem.BudgetItemList[i].ProjectionPercent / 100;
                            percent = budgetItem.BudgetItemList[i].ProjectionPercent;
                            VariableCostList[i] += sales;
                        }
                        else
                        {
                            sales = budgetItem.BudgetItemList[i].ProjectionSales;
                            percent = this.TargetColumnList[i].TargetSales == 0 ? 0 : budgetItem.BudgetItemList[i].ProjectionSales * 100 / this.TargetColumnList[i].TargetSales;
                            FixCostList[i] += sales;
                        }
                        newRow[this.HeaderColumnList[i] + "_ProjectionSales"] = sales;
                        newRow[this.HeaderColumnList[i] + "_ProjectionPercent"] = percent;
                        PrimeCostTotal[i].ProjectionSales += budgetItem.IsPrimeCost ? sales : 0;
                        PayrollTotal[i].ProjectionSales += sales;
                        totalProjectionSales += sales;

                        var actual = budgetItem.BudgetItemList[i].ActualSales;
                        newRow[this.HeaderColumnList[i] + "_ActualSales"] = actual;
                        newRow[this.HeaderColumnList[i] + "_ActualPercent"] = sales == 0 ? 0 : budgetItem.BudgetItemList[i].ActualSales * 100 / sales;
                        PrimeCostTotal[i].ActualSales += budgetItem.IsPrimeCost ? actual : 0;
                        PayrollTotal[i].ActualSales += actual;
                        totalActualSales += actual;

                        newRow[this.HeaderColumnList[i] + "_VarianceSales"] = sales - actual;
                        newRow[this.HeaderColumnList[i] + "_VariancePercent"] = sales == 0 ? 0 : (sales - actual) * 100 / sales;
                    }

                    var variance = totalProjectionSales - totalActualSales;
                    newRow["GrandTotal_ProjectionSales"] = totalProjectionSales;
                    newRow["GrandTotal_ProjectionPercent"] = 0;
                    newRow["GrandTotal_ActualSales"] = totalActualSales;
                    newRow["GrandTotal_ActualPercent"] = totalProjectionSales == 0 ? 0 : totalActualSales * 100 / totalProjectionSales;
                    newRow["GrandTotal_VarianceSales"] = variance;
                    newRow["GrandTotal_VariancePercent"] = totalProjectionSales == 0 ? 0 : variance * 100 / totalProjectionSales;

                    dataTable.Rows.Add(newRow);
                }

                // reCalculate actual percent
                foreach (var item in PayrollTotal)
                {
                    var index = PayrollTotal.IndexOf(item);
                    item.ActualPercent = item.ProjectionSales == 0 ? 0 : item.ActualSales * 100 / item.ProjectionSales;
                    item.ProjectionPercent = this.TargetColumnList[index].TargetSales == 0 ? 0 : item.ProjectionSales * 100 / this.TargetColumnList[index].TargetSales;
                }

                return dataTable;
            }
        }

        public List<BudgetItemDetail> PayrollIsTaxTotal { get; set; }
        public DataTable PayrollIsTaxDataTable
        {
            get
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("BudgetItemId");
                dataTable.Columns.Add("CategorySettingId");
                dataTable.Columns.Add("CategoryName");
                dataTable.Columns.Add("ParentCategoryId");
                dataTable.Columns.Add("ParentCategoryName");

                PayrollIsTaxTotal = new List<BudgetItemDetail>();

                // add column name to table
                foreach (string header in this.HeaderColumnList)
                {
                    dataTable.Columns.Add(header + "_IsPercentage");
                    dataTable.Columns.Add(header + "_ProjectionSales");
                    dataTable.Columns.Add(header + "_ProjectionPercent");
                    dataTable.Columns.Add(header + "_ActualSales");
                    dataTable.Columns.Add(header + "_ActualPercent");
                    dataTable.Columns.Add(header + "_VarianceSales");
                    dataTable.Columns.Add(header + "_VariancePercent");

                    PayrollIsTaxTotal.Add(new BudgetItemDetail());
                }

                // add grand total column to table
                dataTable.Columns.Add("GrandTotal_ProjectionSales");
                dataTable.Columns.Add("GrandTotal_ProjectionPercent");
                dataTable.Columns.Add("GrandTotal_ActualSales");
                dataTable.Columns.Add("GrandTotal_ActualPercent");
                dataTable.Columns.Add("GrandTotal_VarianceSales");
                dataTable.Columns.Add("GrandTotal_VariancePercent");

                // add data to table: is not tax
                var dataByParentCategoryId = this.BudgetItemModelList.Where(s => s.ParentCategoryName == BCSCommonData.CATEGORY_DEFAULT_PAYROLL_EXPENSES_TEXT && s.IsTaxCost == true);
                foreach (BudgetItemModel budgetItem in dataByParentCategoryId)
                {
                    DataRow newRow = dataTable.NewRow();
                    newRow["BudgetItemId"] = budgetItem.BudgetItemId;
                    newRow["CategorySettingId"] = budgetItem.CategorySettingId;
                    newRow["CategoryName"] = budgetItem.CategoryName;
                    //newRow["IsTaxCost"] = budgetItem.IsTaxCost;
                    newRow["ParentCategoryId"] = budgetItem.ParentCategoryId;
                    newRow["ParentCategoryName"] = budgetItem.ParentCategoryName;

                    decimal totalProjectionSales = 0;
                    decimal totalActualSales = 0;
                    for (int i = 0; i < this.HeaderColumnList.Count; i++)
                    {
                        newRow[this.HeaderColumnList[i] + "_IsPercentage"] = budgetItem.BudgetItemList[i].IsPercentage;

                        decimal sales = 0, percent = 0;
                        if (budgetItem.BudgetItemList[i].IsPercentage)
                        {
                            percent = budgetItem.BudgetItemList[i].ProjectionPercent;
                            sales = this.PayrollTotal[i].ProjectionSales * budgetItem.BudgetItemList[i].ProjectionPercent / 100;
                            VariableCostList[i] += sales;
                        }
                        else
                        {
                            sales = budgetItem.BudgetItemList[i].ProjectionSales;
                            percent = this.TargetColumnList[i].TargetSales == 0 ? 0 : budgetItem.BudgetItemList[i].ProjectionSales * 100 / this.TargetColumnList[i].TargetSales;
                            FixCostList[i] += sales;
                        }
                        newRow[this.HeaderColumnList[i] + "_ProjectionSales"] = sales;
                        newRow[this.HeaderColumnList[i] + "_ProjectionPercent"] = percent;
                        PrimeCostTotal[i].ProjectionSales += budgetItem.IsPrimeCost ? sales : 0;
                        PayrollIsTaxTotal[i].ProjectionSales += sales;
                        totalProjectionSales += sales;

                        var actual = budgetItem.BudgetItemList[i].ActualSales;
                        newRow[this.HeaderColumnList[i] + "_ActualSales"] = actual;
                        newRow[this.HeaderColumnList[i] + "_ActualPercent"] = this.PayrollTotal[i].ActualSales == 0 ? 0 : budgetItem.BudgetItemList[i].ActualSales * 100 / this.PayrollTotal[i].ActualSales;
                        PrimeCostTotal[i].ActualSales += budgetItem.IsPrimeCost ? actual : 0;
                        PayrollIsTaxTotal[i].ActualSales += actual;
                        totalActualSales += actual;

                        newRow[this.HeaderColumnList[i] + "_VarianceSales"] = sales - actual;
                        newRow[this.HeaderColumnList[i] + "_VariancePercent"] = sales == 0 ? 0 : (sales - actual) * 100 / sales;

                        // set payroll is tax percent total row
                        PrimeCostTotal[i].ActualPercent = sales == 0 ? 0 : actual * 100 / sales;
                        var projectionSales = this.PayrollTotal[i].ProjectionSales;
                        PrimeCostTotal[i].ProjectionPercent = projectionSales == 0 ? 0 : sales * 100 / projectionSales;

                    }

                    var variance = totalProjectionSales - totalActualSales;
                    newRow["GrandTotal_ProjectionSales"] = totalProjectionSales;
                    newRow["GrandTotal_ProjectionPercent"] = totalProjectionSales == 0 ? 0 : totalProjectionSales * 100 / totalProjectionSales;
                    newRow["GrandTotal_ActualSales"] = totalActualSales;
                    newRow["GrandTotal_ActualPercent"] = totalProjectionSales == 0 ? 0 : totalActualSales * 100 / totalProjectionSales;
                    newRow["GrandTotal_VarianceSales"] = variance;
                    newRow["GrandTotal_VariancePercent"] = totalProjectionSales == 0 ? 0 : variance * 100 / totalProjectionSales;

                    dataTable.Rows.Add(newRow);
                }

                // reCalculate actual percent
                foreach (var item in PayrollIsTaxTotal)
                {
                    var index = PayrollIsTaxTotal.IndexOf(item);
                    var projectionSales = this.PayrollTotal[index].ProjectionSales;
                    item.ProjectionPercent = projectionSales == 0 ? 0 : item.ProjectionSales * 100 / projectionSales;
                    item.ActualPercent = item.ProjectionSales == 0 ? 0 : item.ActualSales * 100 / item.ProjectionSales;
                }

                return dataTable;
            }
        }

        public List<BudgetItemDetail> PayrollAllTotal
        {
            get
            {
                List<BudgetItemDetail> list = new List<BudgetItemDetail>();
                for (int i = 0; i < PayrollTotal.Count; i++)
                {
                    BudgetItemDetail newItem = new BudgetItemDetail();

                    var sales = this.PayrollTotal[i].ProjectionSales + this.PayrollIsTaxTotal[i].ProjectionSales;
                    newItem.ProjectionSales = sales;
                    newItem.ProjectionPercent = this.TargetColumnList[i].TargetSales == 0 ? 0 : sales * 100 / this.TargetColumnList[i].TargetSales;

                    var actual = this.PayrollTotal[i].ActualSales + this.PayrollIsTaxTotal[i].ActualSales;
                    newItem.ActualSales = actual;
                    newItem.ActualPercent = sales == 0 ? 0 : actual * 100 / sales;

                    list.Add(newItem);
                }

                return list;
            }
        }

        public List<BudgetItemDetail> OperationTotal { get; set; }
        public DataTable OperationDataTable
        {
            get
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("BudgetItemId");
                dataTable.Columns.Add("CategorySettingId");
                dataTable.Columns.Add("CategoryName");
                dataTable.Columns.Add("ParentCategoryId");
                dataTable.Columns.Add("ParentCategoryName");

                OperationTotal = new List<BudgetItemDetail>();

                // add column name to table
                foreach (string header in this.HeaderColumnList)
                {
                    dataTable.Columns.Add(header + "_IsPercentage");
                    dataTable.Columns.Add(header + "_ProjectionSales");
                    dataTable.Columns.Add(header + "_ProjectionPercent");
                    dataTable.Columns.Add(header + "_ActualSales");
                    dataTable.Columns.Add(header + "_ActualPercent");
                    dataTable.Columns.Add(header + "_VarianceSales");
                    dataTable.Columns.Add(header + "_VariancePercent");

                    OperationTotal.Add(new BudgetItemDetail());
                }

                // add grand total column to table
                dataTable.Columns.Add("GrandTotal_ProjectionSales");
                dataTable.Columns.Add("GrandTotal_ProjectionPercent");
                dataTable.Columns.Add("GrandTotal_ActualSales");
                dataTable.Columns.Add("GrandTotal_ActualPercent");
                dataTable.Columns.Add("GrandTotal_VarianceSales");
                dataTable.Columns.Add("GrandTotal_VariancePercent");

                // add data to table
                var dataByParentCategoryId = this.BudgetItemModelList.Where(s => s.ParentCategoryName == BCSCommonData.CATEGORY_DEFAULT_OPERATION_EXPENSES_TEXT);
                foreach (BudgetItemModel budgetItem in dataByParentCategoryId)
                {
                    DataRow newRow = dataTable.NewRow();
                    newRow["BudgetItemId"] = budgetItem.BudgetItemId;
                    newRow["CategorySettingId"] = budgetItem.CategorySettingId;
                    newRow["CategoryName"] = budgetItem.CategoryName;
                    newRow["ParentCategoryId"] = budgetItem.ParentCategoryId;
                    newRow["ParentCategoryName"] = budgetItem.ParentCategoryName;

                    decimal totalProjectionSales = 0;
                    decimal totalActualSales = 0;
                    for (int i = 0; i < this.HeaderColumnList.Count; i++)
                    {
                        newRow[this.HeaderColumnList[i] + "_IsPercentage"] = budgetItem.BudgetItemList[i].IsPercentage;

                        decimal sales = 0, percent = 0;
                        if (budgetItem.BudgetItemList[i].IsPercentage)
                        {
                            sales = this.TargetColumnList[i].TargetSales * budgetItem.BudgetItemList[i].ProjectionPercent / 100;
                            percent = budgetItem.BudgetItemList[i].ProjectionPercent;
                            VariableCostList[i] += sales;
                        }
                        else
                        {
                            sales = budgetItem.BudgetItemList[i].ProjectionSales;
                            percent = this.TargetColumnList[i].TargetSales == 0 ? 0 : budgetItem.BudgetItemList[i].ProjectionSales * 100 / this.TargetColumnList[i].TargetSales;
                            FixCostList[i] += sales;
                        }
                        newRow[this.HeaderColumnList[i] + "_ProjectionSales"] = sales;
                        newRow[this.HeaderColumnList[i] + "_ProjectionPercent"] = percent;
                        OperationTotal[i].ProjectionSales += sales;
                        totalProjectionSales += sales;

                        var actual = budgetItem.BudgetItemList[i].ActualSales;
                        newRow[this.HeaderColumnList[i] + "_ActualSales"] = actual;
                        newRow[this.HeaderColumnList[i] + "_ActualPercent"] = SalesTotal[i].ActualSales == 0 ? 0 : budgetItem.BudgetItemList[i].ActualSales * 100 / SalesTotal[i].ActualSales;
                        OperationTotal[i].ActualSales += actual;
                        totalActualSales += actual;

                        newRow[this.HeaderColumnList[i] + "_VarianceSales"] = sales - actual;
                        newRow[this.HeaderColumnList[i] + "_VariancePercent"] = sales == 0 ? 0 : (sales - actual) * 100 / sales;
                    }

                    var variance = totalProjectionSales - totalActualSales;
                    newRow["GrandTotal_ProjectionSales"] = totalProjectionSales;
                    newRow["GrandTotal_ProjectionPercent"] = 0;
                    newRow["GrandTotal_ActualSales"] = totalActualSales;
                    newRow["GrandTotal_ActualPercent"] = totalProjectionSales == 0 ? 0 : totalActualSales * 100 / totalProjectionSales;
                    newRow["GrandTotal_VarianceSales"] = variance;
                    newRow["GrandTotal_VariancePercent"] = totalProjectionSales == 0 ? 0 : variance * 100 / totalProjectionSales;

                    dataTable.Rows.Add(newRow);
                }

                // reCalculate actual percent
                foreach (var item in OperationTotal)
                {
                    item.ActualPercent = item.ProjectionSales == 0 ? 0 : item.ActualSales * 100 / item.ProjectionSales;

                    var index = OperationTotal.IndexOf(item);
                    item.ProjectionPercent = this.TargetColumnList[index].TargetSales == 0 ? 0 : item.ProjectionSales * 100 / this.TargetColumnList[index].TargetSales;
                }

                return dataTable;
            }
        }

        public List<BudgetItemDetail> ProfitLossTotal { get; set; }
        public DataTable ProfitLossDataTable
        {
            get
            {
                if (this.SalesTotal == null) { var x = this.SalesDataTable; }
                if (this.CogsTotal == null) { var x = this.CogsDataTable; }
                if (this.PayrollTotal == null) { var x = this.PayrollDataTable; x = this.PayrollIsTaxDataTable; }
                if (this.OperationTotal == null) { var x = this.OperationDataTable; }

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("BudgetItemId");
                dataTable.Columns.Add("CategorySettingId");
                dataTable.Columns.Add("CategoryName");
                dataTable.Columns.Add("ParentCategoryId");
                dataTable.Columns.Add("ParentCategoryName");

                ProfitLossTotal = new List<BudgetItemDetail>();

                // add column name to table
                foreach (string header in this.HeaderColumnList)
                {
                    dataTable.Columns.Add(header + "_IsPercentage");
                    dataTable.Columns.Add(header + "_ProjectionSales");
                    dataTable.Columns.Add(header + "_ProjectionPercent");
                    dataTable.Columns.Add(header + "_ActualSales");
                    dataTable.Columns.Add(header + "_ActualPercent");
                    dataTable.Columns.Add(header + "_VarianceSales");
                    dataTable.Columns.Add(header + "_VariancePercent");

                    ProfitLossTotal.Add(new BudgetItemDetail());
                }

                // add grand total column to table
                dataTable.Columns.Add("GrandTotal_ProjectionSales");
                dataTable.Columns.Add("GrandTotal_ProjectionPercent");
                dataTable.Columns.Add("GrandTotal_ActualSales");
                dataTable.Columns.Add("GrandTotal_ActualPercent");
                dataTable.Columns.Add("GrandTotal_VarianceSales");
                dataTable.Columns.Add("GrandTotal_VariancePercent");

                // add data to table
                var dataByParentCategoryId = this.BudgetItemModelList.Where(s => s.ParentCategoryName == BCSCommonData.CATEGORY_DEFAULT_PROFIT_LOSS_TEXT);
                foreach (BudgetItemModel budgetItem in dataByParentCategoryId)
                {
                    DataRow newRow = dataTable.NewRow();
                    newRow["BudgetItemId"] = budgetItem.BudgetItemId;
                    newRow["CategorySettingId"] = budgetItem.CategorySettingId;
                    newRow["CategoryName"] = budgetItem.CategoryName;
                    newRow["ParentCategoryId"] = budgetItem.ParentCategoryId;
                    newRow["ParentCategoryName"] = budgetItem.ParentCategoryName;

                    for (int i = 0; i < this.HeaderColumnList.Count; i++)
                    {
                        newRow[this.HeaderColumnList[i] + "_IsPercentage"] = budgetItem.BudgetItemList[i].IsPercentage;

                        decimal sales = 0, percent = 0;
                        if (budgetItem.BudgetItemList[i].IsPercentage)
                        {
                            sales = this.TargetColumnList[i].TargetSales * budgetItem.BudgetItemList[i].ProjectionPercent / 100;
                            percent = budgetItem.BudgetItemList[i].ProjectionPercent;
                        }
                        else
                        {
                            sales = budgetItem.BudgetItemList[i].ProjectionSales;
                            percent = this.TargetColumnList[i].TargetSales == 0 ? 0 : budgetItem.BudgetItemList[i].ProjectionSales * 100 / this.TargetColumnList[i].TargetSales;
                        }
                        newRow[this.HeaderColumnList[i] + "_ProjectionSales"] = sales;
                        newRow[this.HeaderColumnList[i] + "_ProjectionPercent"] = percent;
                        ProfitLossTotal[i].ProjectionSales += sales;

                        var actual = budgetItem.BudgetItemList[i].ActualSales;
                        newRow[this.HeaderColumnList[i] + "_ActualSales"] = actual;
                        newRow[this.HeaderColumnList[i] + "_ActualPercent"] = SalesTotal[i].ActualSales == 0 ? 0 : budgetItem.BudgetItemList[i].ActualSales * 100 / SalesTotal[i].ActualSales;
                        ProfitLossTotal[i].ActualSales += actual;

                        newRow[this.HeaderColumnList[i] + "_VarianceSales"] = sales - actual;
                        newRow[this.HeaderColumnList[i] + "_VariancePercent"] = sales == 0 ? 0 : (sales - actual) * 100 / sales;
                    }

                    newRow["GrandTotal_ProjectionSales"] = 0;
                    newRow["GrandTotal_ProjectionPercent"] = 0;
                    newRow["GrandTotal_ActualSales"] = 0;
                    newRow["GrandTotal_ActualPercent"] = 0;
                    newRow["GrandTotal_VarianceSales"] = 0;
                    newRow["GrandTotal_VariancePercent"] = 0;

                    dataTable.Rows.Add(newRow);
                }

                // set data to Sales
                if (dataTable.Rows.Count == 0)
                {
                    string[] categoryList = { "Sales", "COGS", "Gross Profit", "Payroll Expenses", "Operating Profit", "Prime Cost", "Operating Expenses", "Net Profit/Loss", "Net Profit Running Total", "Breakeven Point" };
                    foreach (string item in categoryList)
                    {
                        DataRow newRow = dataTable.NewRow();
                        newRow["CategoryName"] = item;
                        dataTable.Rows.Add(newRow);
                    }
                }

                DataRow salesRow = dataTable.Rows[0];
                DataRow cogsRow = dataTable.Rows[1];
                DataRow grossProfitRow = dataTable.Rows[2];
                DataRow payrollRow = dataTable.Rows[3];
                DataRow operatingProfitRow = dataTable.Rows[4];
                DataRow primeCostRow = dataTable.Rows[5];
                DataRow operationRow = dataTable.Rows[6];
                DataRow netProfitRow = dataTable.Rows[7];
                DataRow netProfitRuningRow = dataTable.Rows[8];
                DataRow bepRow = dataTable.Rows[9];
                decimal netProfitRuningSales = 0, netProfitRuningActual = 0, netProfitRuningVariance = 0, bepGrandTotalSales = 0;
                for (int i = 0; i < this.HeaderColumnList.Count; i++)
                {
                    // set sales value to row
                    salesRow[this.HeaderColumnList[i] + "_ProjectionSales"] = this.SalesTotal[i].ProjectionSales;
                    salesRow[this.HeaderColumnList[i] + "_ProjectionPercent"] = this.SalesTotal[i].ProjectionPercent;
                    salesRow[this.HeaderColumnList[i] + "_ActualSales"] = this.SalesTotal[i].ActualSales;
                    salesRow[this.HeaderColumnList[i] + "_ActualPercent"] = this.SalesTotal[i].ActualSales == 0 ? 0 : 100;
                    salesRow[this.HeaderColumnList[i] + "_VarianceSales"] = this.SalesTotal[i].VarianceSales;
                    salesRow[this.HeaderColumnList[i] + "_VariancePercent"] = this.SalesTotal[i].VariancePercent;

                    // set cogs value to row
                    cogsRow[this.HeaderColumnList[i] + "_ProjectionSales"] = this.CogsTotal[i].ProjectionSales;
                    cogsRow[this.HeaderColumnList[i] + "_ProjectionPercent"] = this.CogsTotal[i].ProjectionPercent;
                    cogsRow[this.HeaderColumnList[i] + "_ActualSales"] = this.CogsTotal[i].ActualSales;
                    cogsRow[this.HeaderColumnList[i] + "_ActualPercent"] = this.CogsTotal[i].ActualPercent;
                    cogsRow[this.HeaderColumnList[i] + "_VarianceSales"] = this.CogsTotal[i].VarianceSales;
                    cogsRow[this.HeaderColumnList[i] + "_VariancePercent"] = this.CogsTotal[i].VariancePercent;

                    // set gross profit to row
                    var grossSales = this.SalesTotal[i].ProjectionSales - this.CogsTotal[i].ProjectionSales;
                    var grossActual = this.SalesTotal[i].ActualSales - this.CogsTotal[i].ActualSales;
                    var grossVariance = grossActual - grossSales;
                    grossProfitRow[this.HeaderColumnList[i] + "_ProjectionSales"] = grossSales;
                    grossProfitRow[this.HeaderColumnList[i] + "_ProjectionPercent"] = this.TargetColumnList[i].TargetSales == 0 ? 0 : grossSales * 100 / this.TargetColumnList[i].TargetSales;
                    grossProfitRow[this.HeaderColumnList[i] + "_ActualSales"] = grossActual;
                    grossProfitRow[this.HeaderColumnList[i] + "_ActualPercent"] = this.SalesTotal[i].ActualSales == 0 ? 0 : grossActual * 100 / this.SalesTotal[i].ActualSales;
                    grossProfitRow[this.HeaderColumnList[i] + "_VarianceSales"] = grossVariance;
                    grossProfitRow[this.HeaderColumnList[i] + "_VariancePercent"] = grossSales == 0 ? 0 : grossVariance * 100 / grossSales;

                    // set payroll value to row index is 4
                    payrollRow[this.HeaderColumnList[i] + "_ProjectionSales"] = this.PayrollAllTotal[i].ProjectionSales;
                    payrollRow[this.HeaderColumnList[i] + "_ProjectionPercent"] = this.PayrollAllTotal[i].ProjectionPercent;
                    payrollRow[this.HeaderColumnList[i] + "_ActualSales"] = this.PayrollAllTotal[i].ActualSales;
                    payrollRow[this.HeaderColumnList[i] + "_ActualPercent"] = this.SalesTotal[i].ActualSales == 0 ? 0 : this.PayrollAllTotal[i].ActualSales * 100 / this.SalesTotal[i].ActualSales;
                    payrollRow[this.HeaderColumnList[i] + "_VarianceSales"] = this.PayrollAllTotal[i].VarianceSales;
                    payrollRow[this.HeaderColumnList[i] + "_VariancePercent"] = this.PayrollAllTotal[i].VariancePercent;

                    // set operating profit to row index is 5
                    var operatingSales = grossSales - this.PayrollAllTotal[i].ProjectionSales;
                    var operatingActual = grossActual - this.PayrollAllTotal[i].ActualSales;
                    var operatingVariance = operatingActual - operatingSales;
                    operatingProfitRow[this.HeaderColumnList[i] + "_ProjectionSales"] = operatingSales;
                    operatingProfitRow[this.HeaderColumnList[i] + "_ProjectionPercent"] = this.TargetColumnList[i].TargetSales == 0 ? 0 : operatingSales * 100 / this.TargetColumnList[i].TargetSales;
                    operatingProfitRow[this.HeaderColumnList[i] + "_ActualSales"] = operatingActual;
                    operatingProfitRow[this.HeaderColumnList[i] + "_ActualPercent"] = this.SalesTotal[i].ActualSales == 0 ? 0 : operatingActual * 100 / this.SalesTotal[i].ActualSales;
                    operatingProfitRow[this.HeaderColumnList[i] + "_VarianceSales"] = operatingVariance;
                    operatingProfitRow[this.HeaderColumnList[i] + "_VariancePercent"] = operatingSales == 0 ? 0 : operatingVariance * 100 / operatingSales;

                    // set prime cost to row index is 6
                    var primeCostSales = this.CogsTotal[i].ProjectionSales + this.PrimeCostTotal[i].ProjectionSales;
                    var primeCostActual = this.CogsTotal[i].ActualSales + this.PrimeCostTotal[i].ActualSales;
                    var primeCostVariance = primeCostActual - primeCostSales;
                    primeCostRow[this.HeaderColumnList[i] + "_ProjectionSales"] = primeCostSales;
                    primeCostRow[this.HeaderColumnList[i] + "_ProjectionPercent"] = this.TargetColumnList[i].TargetSales == 0 ? 0 : primeCostSales * 100 / this.TargetColumnList[i].TargetSales;
                    primeCostRow[this.HeaderColumnList[i] + "_ActualSales"] = primeCostActual;
                    primeCostRow[this.HeaderColumnList[i] + "_ActualPercent"] = this.SalesTotal[i].ActualSales == 0 ? 0 : primeCostActual * 100 / this.SalesTotal[i].ActualSales;
                    primeCostRow[this.HeaderColumnList[i] + "_VarianceSales"] = primeCostVariance;
                    primeCostRow[this.HeaderColumnList[i] + "_VariancePercent"] = primeCostSales == 0 ? 0 : primeCostVariance * 100 / primeCostSales;

                    // set operation value to row index is 7
                    operationRow[this.HeaderColumnList[i] + "_ProjectionSales"] = this.OperationTotal[i].ProjectionSales;
                    operationRow[this.HeaderColumnList[i] + "_ProjectionPercent"] = this.OperationTotal[i].ProjectionPercent;
                    operationRow[this.HeaderColumnList[i] + "_ActualSales"] = this.OperationTotal[i].ActualSales;
                    operationRow[this.HeaderColumnList[i] + "_ActualPercent"] = this.SalesTotal[i].ActualSales == 0 ? 0 : this.OperationTotal[i].ActualSales * 100 / this.SalesTotal[i].ActualSales;
                    operationRow[this.HeaderColumnList[i] + "_VarianceSales"] = this.OperationTotal[i].VarianceSales;
                    operationRow[this.HeaderColumnList[i] + "_VariancePercent"] = this.OperationTotal[i].VariancePercent;

                    // set net profit to row index is 8
                    var netProfitSales = operatingSales - this.OperationTotal[i].ProjectionSales;
                    var netProfitActual = operatingActual - this.OperationTotal[i].ActualSales;
                    var netProfitVariance = netProfitActual - netProfitSales;
                    netProfitRow[this.HeaderColumnList[i] + "_ProjectionSales"] = netProfitSales;
                    netProfitRow[this.HeaderColumnList[i] + "_ProjectionPercent"] = this.TargetColumnList[i].TargetSales == 0 ? 0 : netProfitSales * 100 / this.TargetColumnList[i].TargetSales;
                    netProfitRow[this.HeaderColumnList[i] + "_ActualSales"] = netProfitActual;
                    netProfitRow[this.HeaderColumnList[i] + "_ActualPercent"] = this.SalesTotal[i].ActualSales == 0 ? 0 : netProfitActual * 100 / this.SalesTotal[i].ActualSales;
                    netProfitRow[this.HeaderColumnList[i] + "_VarianceSales"] = netProfitVariance;
                    netProfitRow[this.HeaderColumnList[i] + "_VariancePercent"] = netProfitSales == 0 ? 0 : netProfitVariance * 100 / netProfitSales;

                    // set profit loss total by net profit row
                    ProfitLossTotal[i].ProjectionSales = netProfitSales;
                    ProfitLossTotal[i].ProjectionPercent = this.TargetColumnList[i].TargetSales == 0 ? 0 : netProfitSales * 100 / this.TargetColumnList[i].TargetSales;
                    ProfitLossTotal[i].ActualSales = netProfitActual;
                    ProfitLossTotal[i].ActualPercent = netProfitSales == 0 ? 0 : netProfitActual * 100 / netProfitSales;

                    // set net profit runing total to row index is 9
                    netProfitRuningSales += netProfitSales;
                    netProfitRuningActual += netProfitActual;
                    netProfitRuningVariance = netProfitRuningActual - netProfitRuningSales;
                    netProfitRuningRow[this.HeaderColumnList[i] + "_ProjectionSales"] = netProfitRuningSales;
                    netProfitRuningRow[this.HeaderColumnList[i] + "_ProjectionPercent"] = this.TargetColumnList[i].TargetSales == 0 ? 0 : netProfitRuningSales * 100 / this.TargetColumnList[i].TargetSales;
                    netProfitRuningRow[this.HeaderColumnList[i] + "_ActualSales"] = netProfitRuningActual;
                    netProfitRuningRow[this.HeaderColumnList[i] + "_ActualPercent"] = this.SalesTotal[i].ActualSales == 0 ? 0 : netProfitRuningActual * 100 / this.SalesTotal[i].ActualSales;
                    netProfitRuningRow[this.HeaderColumnList[i] + "_VarianceSales"] = netProfitRuningVariance;
                    netProfitRuningRow[this.HeaderColumnList[i] + "_VariancePercent"] = netProfitRuningSales == 0 ? 0 : netProfitRuningVariance * 100 / netProfitRuningSales;

                    // set Breakeven Point value to row index is 10
                    var bepSales = this.TargetColumnList[i].TargetSales == 0 ? 0 : this.FixCostList[i] / (1 - this.VariableCostList[i] / this.TargetColumnList[i].TargetSales);
                    bepGrandTotalSales += bepSales;
                    bepRow[this.HeaderColumnList[i] + "_ProjectionSales"] = bepSales;
                    bepRow[this.HeaderColumnList[i] + "_ProjectionPercent"] = this.TargetColumnList[i].TargetSales == 0 ? 0 : bepSales * 100 / this.TargetColumnList[i].TargetSales;
                    bepRow[this.HeaderColumnList[i] + "_ActualSales"] = 0;
                    bepRow[this.HeaderColumnList[i] + "_ActualPercent"] = 0;
                    bepRow[this.HeaderColumnList[i] + "_VarianceSales"] = 0;
                    bepRow[this.HeaderColumnList[i] + "_VariancePercent"] = 0;

                }

                // set data to Sales
                var grandTotalSales = this.SalesTotal.Sum(s => s.ProjectionSales);
                var grandTotalActual = this.SalesTotal.Sum(s => s.ActualSales);
                var variance = grandTotalActual - grandTotalSales;
                salesRow["GrandTotal_ProjectionSales"] = grandTotalSales;
                salesRow["GrandTotal_ProjectionPercent"] = grandTotalSales == 0 ? 0 : 100;
                salesRow["GrandTotal_ActualSales"] = grandTotalActual;
                salesRow["GrandTotal_ActualPercent"] = grandTotalSales == 0 ? 0 : grandTotalActual * 100 / grandTotalSales;
                salesRow["GrandTotal_VarianceSales"] = variance;
                salesRow["GrandTotal_VariancePercent"] = grandTotalSales == 0 ? 0 : variance * 100 / grandTotalSales;

                // set data to COGS
                var cogsGrandTotalSales = this.CogsTotal.Sum(s => s.ProjectionSales);
                var cogsGrandTotalActual = this.CogsTotal.Sum(s => s.ActualSales);
                var cogsGrandTotalVariance = cogsGrandTotalActual - cogsGrandTotalSales;
                cogsRow["GrandTotal_ProjectionSales"] = cogsGrandTotalSales;
                cogsRow["GrandTotal_ProjectionPercent"] = grandTotalSales == 0 ? 0 : cogsGrandTotalSales * 100 / grandTotalSales;
                cogsRow["GrandTotal_ActualSales"] = cogsGrandTotalActual;
                cogsRow["GrandTotal_ActualPercent"] = cogsGrandTotalSales == 0 ? 0 : cogsGrandTotalActual * 100 / cogsGrandTotalSales;
                cogsRow["GrandTotal_VarianceSales"] = cogsGrandTotalVariance;
                cogsRow["GrandTotal_VariancePercent"] = cogsGrandTotalSales == 0 ? 0 : cogsGrandTotalVariance * 100 / cogsGrandTotalSales;

                // set data to gross profit
                var grossGrandTotalSales = grandTotalSales - cogsGrandTotalSales;
                var grossGrandTotalActual = grandTotalActual - cogsGrandTotalActual;
                var grossGrandTotalVariance = grossGrandTotalActual - grossGrandTotalSales;
                grossProfitRow["GrandTotal_ProjectionSales"] = grossGrandTotalSales;
                grossProfitRow["GrandTotal_ProjectionPercent"] = grandTotalSales == 0 ? 0 : grossGrandTotalSales * 100 / grandTotalSales;
                grossProfitRow["GrandTotal_ActualSales"] = grossGrandTotalActual;
                grossProfitRow["GrandTotal_ActualPercent"] = grossGrandTotalSales == 0 ? 0 : grossGrandTotalActual * 100 / grossGrandTotalSales;
                grossProfitRow["GrandTotal_VarianceSales"] = grossGrandTotalVariance;
                grossProfitRow["GrandTotal_VariancePercent"] = grossGrandTotalSales == 0 ? 0 : grossGrandTotalVariance * 100 / grossGrandTotalSales;

                // set data to Payroll
                var payrollGrandTotalSales = this.PayrollAllTotal.Sum(s => s.ProjectionSales);
                var payrollGrandTotalActual = this.PayrollAllTotal.Sum(s => s.ActualSales);
                var payrollGrandTtoalVariance = payrollGrandTotalActual - payrollGrandTotalSales;
                payrollRow["GrandTotal_ProjectionSales"] = payrollGrandTotalSales;
                payrollRow["GrandTotal_ProjectionPercent"] = grandTotalSales == 0 ? 0 : payrollGrandTotalSales * 100 / grandTotalSales;
                payrollRow["GrandTotal_ActualSales"] = payrollGrandTotalActual;
                payrollRow["GrandTotal_ActualPercent"] = payrollGrandTotalSales == 0 ? 0 : payrollGrandTotalActual * 100 / payrollGrandTotalSales;
                payrollRow["GrandTotal_VarianceSales"] = payrollGrandTtoalVariance;
                payrollRow["GrandTotal_VariancePercent"] = payrollGrandTotalSales == 0 ? 0 : payrollGrandTtoalVariance * 100 / payrollGrandTotalSales;

                // set data to operating profit
                var operatingGrandTotalSales = grandTotalSales - cogsGrandTotalSales - payrollGrandTotalSales;
                var operatingGrandTotalActual = grandTotalActual - cogsGrandTotalActual - payrollGrandTotalActual;
                var operatingGrandTotalVariance = operatingGrandTotalActual - operatingGrandTotalSales;
                operatingProfitRow["GrandTotal_ProjectionSales"] = operatingGrandTotalSales;
                operatingProfitRow["GrandTotal_ProjectionPercent"] = grandTotalSales == 0 ? 0 : operatingGrandTotalSales * 100 / grandTotalSales;
                operatingProfitRow["GrandTotal_ActualSales"] = operatingGrandTotalActual;
                operatingProfitRow["GrandTotal_ActualPercent"] = operatingGrandTotalSales == 0 ? 0 : operatingGrandTotalActual * 100 / operatingGrandTotalSales;
                operatingProfitRow["GrandTotal_VarianceSales"] = operatingGrandTotalVariance;
                operatingProfitRow["GrandTotal_VariancePercent"] = operatingGrandTotalSales == 0 ? 0 : operatingGrandTotalVariance * 100 / operatingGrandTotalSales;

                // set data to Prime Cost
                var primeCostGrandTotalSales = cogsGrandTotalSales + this.PrimeCostTotal.Sum(s => s.ProjectionSales);
                var primeCostGrandTotalActual = cogsGrandTotalActual + this.PrimeCostTotal.Sum(s => s.ActualSales);
                var primeCostGrandTtoalVariance = primeCostGrandTotalActual - primeCostGrandTotalSales;
                primeCostRow["GrandTotal_ProjectionSales"] = primeCostGrandTotalSales;
                primeCostRow["GrandTotal_ProjectionPercent"] = grandTotalSales == 0 ? 0 : primeCostGrandTotalSales * 100 / grandTotalSales;
                primeCostRow["GrandTotal_ActualSales"] = primeCostGrandTotalActual;
                primeCostRow["GrandTotal_ActualPercent"] = primeCostGrandTotalSales == 0 ? 0 : primeCostGrandTotalActual * 100 / primeCostGrandTotalSales;
                primeCostRow["GrandTotal_VarianceSales"] = primeCostGrandTtoalVariance;
                primeCostRow["GrandTotal_VariancePercent"] = primeCostGrandTotalSales == 0 ? 0 : primeCostGrandTtoalVariance * 100 / primeCostGrandTotalSales;

                // set data to Operation
                var operationGrandTotalSales = this.OperationTotal.Sum(s => s.ProjectionSales);
                var operationGrandTotalActual = this.OperationTotal.Sum(s => s.ActualSales);
                var operationGrandTtoalVariance = operationGrandTotalActual - operationGrandTotalSales;
                operationRow["GrandTotal_ProjectionSales"] = operationGrandTotalSales;
                operationRow["GrandTotal_ProjectionPercent"] = grandTotalSales == 0 ? 0 : operationGrandTotalSales * 100 / grandTotalSales;
                operationRow["GrandTotal_ActualSales"] = operationGrandTotalActual;
                operationRow["GrandTotal_ActualPercent"] = grandTotalActual == 0 ? 0 : operationGrandTotalActual * 100 / grandTotalActual;
                operationRow["GrandTotal_VarianceSales"] = operationGrandTtoalVariance;
                operationRow["GrandTotal_VariancePercent"] = operationGrandTotalSales == 0 ? 0 : operationGrandTtoalVariance * 100 / operationGrandTotalSales;

                // set data to Net Profit
                var netProfitGrandTotalSales = operatingGrandTotalSales - operationGrandTotalSales;
                var netProfitGrandTotalActual = operatingGrandTotalActual - operationGrandTotalActual;
                var netProfitGrandTtoalVariance = netProfitGrandTotalActual - netProfitGrandTotalSales;
                netProfitRow["GrandTotal_ProjectionSales"] = netProfitGrandTotalSales;
                netProfitRow["GrandTotal_ProjectionPercent"] = grandTotalSales == 0 ? 0 : netProfitGrandTotalSales * 100 / grandTotalSales;
                netProfitRow["GrandTotal_ActualSales"] = netProfitGrandTotalActual;
                netProfitRow["GrandTotal_ActualPercent"] = netProfitGrandTotalSales == 0 ? 0 : netProfitGrandTotalActual * 100 / netProfitGrandTotalSales;
                netProfitRow["GrandTotal_VarianceSales"] = netProfitGrandTtoalVariance;
                netProfitRow["GrandTotal_VariancePercent"] = netProfitGrandTotalSales == 0 ? 0 : netProfitGrandTtoalVariance * 100 / netProfitGrandTotalSales;

                // set data to Net Profit Runing Total
                netProfitRuningRow["GrandTotal_ProjectionSales"] = netProfitRuningSales;
                netProfitRuningRow["GrandTotal_ProjectionPercent"] = grandTotalSales == 0 ? 0 : netProfitRuningSales * 100 / grandTotalSales;
                netProfitRuningRow["GrandTotal_ActualSales"] = netProfitRuningActual;
                netProfitRuningRow["GrandTotal_ActualPercent"] = netProfitRuningSales == 0 ? 0 : netProfitRuningActual * 100 / netProfitRuningSales;
                netProfitRuningRow["GrandTotal_VarianceSales"] = netProfitRuningVariance;
                netProfitRuningRow["GrandTotal_VariancePercent"] = netProfitRuningSales == 0 ? 0 : netProfitRuningVariance * 100 / netProfitRuningSales;

                // set data to Net Profit Runing Total
                bepRow["GrandTotal_ProjectionSales"] = bepGrandTotalSales;
                bepRow["GrandTotal_ProjectionPercent"] = grandTotalSales == 0 ? 0 : bepGrandTotalSales * 100 / grandTotalSales;
                bepRow["GrandTotal_ActualSales"] = 0;
                bepRow["GrandTotal_ActualPercent"] = 0;
                bepRow["GrandTotal_VarianceSales"] = 0;
                bepRow["GrandTotal_VariancePercent"] = 0;

                return dataTable;
            }
        }
    }

    public class TargetItemDetail
    {
        public decimal TargetSales { get; set; }
        public decimal TargetPercent { get; set; }
    }
}