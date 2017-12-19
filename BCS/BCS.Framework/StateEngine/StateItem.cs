using System;
using System.Collections.Generic;

namespace BCS.Framework.StateEngine
{
    /// <summary>
    /// Not implement on UAT 1
    /// </summary>
    public class StateItem
    {
        Dictionary<string, string> _itemData = new Dictionary<string, string>();
        IList<Guid> owners = new List<Guid>();

        public int Id { get; set; }

        public Guid Originator { get; set; }

        public DateTime CreateTime { get; set; }

        public Dictionary<string, string> ItemData
        {
            get { return _itemData; }
        }
        
        public IList<Guid> Owners
        {
            get { return owners; }
        }

        public void SetOwner(IList<Guid> owners)
        {
            this.owners = owners;
        }

        internal void SetItemData(Dictionary<string, string> itemData)
        {
            _itemData = itemData;
        }
    }
}