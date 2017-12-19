namespace BCS.Framework.Commons
{
    public class BaseListParam {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string Keyword { get; set; }
        public string OrderByField { get; set; }
        public string FilterField { get; set; }
        public int ParentsID { get; set; }
    }
}
