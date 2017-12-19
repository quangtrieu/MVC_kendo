using System.Collections.Generic;

namespace BCS.Framework.Commons
{
    interface IBusinessLogic
    {
    }
    /// <summary>
    /// Business Logic Interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBusinessLogic<T>
    {
        T Get(int id);
        IList<T> Get(BaseListParam param, out int? totalRecord);
        int Save(T obj, int userID);
        bool Delete(int objId, int userID);
    }
}
