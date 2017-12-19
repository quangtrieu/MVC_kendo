using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BCS.Web.Models
{
    public class BaseEntity
    {
        public DateTime? CreatedDate { get; set; }
        public long? CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }
        public long? UpdatedUserID { get; set; }
    }
}
