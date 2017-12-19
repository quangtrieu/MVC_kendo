using System;
using BCS.Framework.Models;

namespace BCS.Framework.SecurityServices.Entity
{
    public class ModuleInfo:BaseEntity
    {
        public int ModuleID { get; set; }
        public string Name { get; set; }
        public string ModuleCode { get; set; }
        public string Description { get; set; }
        public Nullable<int> Parent { get; set; }
    }
}
