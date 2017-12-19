using System.Collections.Generic;

namespace BCS.Framework.Commons
{
    /// <summary>
    /// Data Access Interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataAccess<T>
    {
        T Get(int id);
        IList<T> Get(BaseListParam param, out int? totalRecord);
        int Save(T obj, int userID);
        bool Delete(int objId, int userID);
    }
}
