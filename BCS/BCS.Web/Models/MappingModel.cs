using System.Data;

namespace BCS.Web.Models
{
    public class MappingModel
    {
        public int BudgetId { get; set; }
        public int BudgetTabId { get; set; }
        public string BudgetName { get; set; }
        public string HeaderName { get; set; }

        public string FileName { get; set; }
        public int SheetIndex { get; set; }
        public string FirstValue { get; set; }
        public string SecondValue { get; set; }
        public string FuncCalculate { get; set; }
        public string RedirectPage { get; set; }

        public DataTable DataTableBySheet { get; set; }


        
    }
}