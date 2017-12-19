using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCS.Framework.Models;

namespace BCS.Framework.SecurityServices.Entity
{
    [Serializable]
    public class GroupInfo : BaseEntity
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; }
    }
}
