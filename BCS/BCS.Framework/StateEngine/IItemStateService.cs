using System;
using System.Collections.Generic;
using BCS.Framework.Commons;
using BCS.Framework.Commons;

namespace BCS.Framework.StateEngine
{
    /// <summary>
    /// Not implement on UAT 1
    /// </summary>
    public interface IItemStateService
    {
        StateItem Create(Guid originator);

        void Save(StateItem stateItem);

        StateItem GetById(int id);

        IList<StateItem> GetByOwner(Guid owner, PagingInfo paging);

        IList<StateItem> GetByOriginator(Guid originator, PagingInfo paging);
    }
}
