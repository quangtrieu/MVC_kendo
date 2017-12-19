using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Web.Models
{
    public class BudgetTabHeaderModel
    {
        public int TabId { get; set; }
        public List<int> HeaderIndex { get; set; }
    }
}