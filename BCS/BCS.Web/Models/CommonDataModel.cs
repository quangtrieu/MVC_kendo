using System;

namespace BCS.Web.Models
{
    public class CommonDataModel
    {
        public int Id { get; set; }
        public string DataType { get; set; }
        public string DataCode { get; set; }
        public Nullable<int> DataValue { get; set; }
        public string DataText { get; set; }
        public string Description { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<bool> DeletedFlg { get; set; }
    }
}