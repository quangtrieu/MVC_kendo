using BCS.DataAccess;
using BCS.Entity;
using BCS.Framework.Commons;
using BCS.Framework.Singleton;
using System;
using System.Collections.Generic;

namespace BCS.BusinessLogic
{
    public class RestActiveCodeBll : BaseBll, IBusinessLogic<RestActiveCode>
    {
        public RestActiveCode Get(int id)
        {
            return SingletonIpl.GetInstance<RestActiveCodeDal>().Get(id);
        }

        public RestActiveCode GetByToken(string tokenId)
        {
            return SingletonIpl.GetInstance<RestActiveCodeDal>().GetByToken(tokenId);
        }

        public IList<RestActiveCode> Get(BaseListParam param, out int? totalRecord)
        {
            throw new NotImplementedException();
        }

        public int Save(RestActiveCode obj, int userId)
        {
            return SingletonIpl.GetInstance<RestActiveCodeDal>().Save(obj, userId);
        }

        public bool Delete(int objId, int userID)
        {
            throw new NotImplementedException();
        }

        public IList<RestActiveCode> GetRestCodeByUserId(int userId)
        {
            return SingletonIpl.GetInstance<RestActiveCodeDal>().GetRestCodeByUserId(userId);
        }

        public RestActiveCode GetByUserIdAndRestCode(int userId, string restCode)
        {
            return SingletonIpl.GetInstance<RestActiveCodeDal>().GetByUserIdAndRestCode(userId, restCode);
        }

        public bool UpdateDefaultRestCodeByUser(int userId, string restCode)
        {
            return SingletonIpl.GetInstance<RestActiveCodeDal>().UpdateDefaultRestCodeByUser(userId, restCode);
        }

        public bool ResetNotDefaultRestCodeByUserId(int userId)
        {
            return SingletonIpl.GetInstance<RestActiveCodeDal>().ResetNotDefaultRestCodeByUserId(userId);
        }
    }
}
