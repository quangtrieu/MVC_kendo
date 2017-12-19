using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCS.Entity
{
    public class CommonData
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
