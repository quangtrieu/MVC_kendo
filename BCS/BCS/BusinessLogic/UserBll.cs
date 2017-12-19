using System;
using System.Collections.Generic;
using BCS.DataAccess;
using BCS.Entity;
using BCS.Framework.Commons;
using BCS.Framework.Singleton;

namespace BCS.BusinessLogic
{
    public class UserBll : BaseBll, IBusinessLogic<User>
    {
        public User Get(int id)
        {
            return SingletonIpl.GetInstance<UserDal>().Get(id);
        }

        public IList<User> Get(BaseListParam param, out int? totalRecord)
        {
            throw new NotImplementedException();
        }

        public int Save(User obj, int userID)
        {
            return SingletonIpl.GetInstance<UserDal>().Save(obj, userID);
        }

        public bool Delete(int objId, int userID)
        {
            throw new NotImplementedException();
        }

        public User GetUserByTokenId(string id)
        {
            return SingletonIpl.GetInstance<UserDal>().GetUserByTokenId(id);
        }

        public IList<User> GetUserByRestCode(int userID, string restCode)
        {
            return SingletonIpl.GetInstance<UserDal>().GetUserByRestCode(userID, restCode);
        }

        public IList<User> GetUserByRole(int userID, int systemId, int roleId, int systemAdminUserId)
        {
            return SingletonIpl.GetInstance<UserDal>().GetUserByRole(userID, systemId, roleId, systemAdminUserId);
        }

        public bool CheckDuplicateUserName(int userId, string userName)
        {
            return SingletonIpl.GetInstance<UserDal>().CheckDuplicateUserName(userId, userName);
        }

        public bool CheckDuplicateEmail(int userId, string email)
        {
            return SingletonIpl.GetInstance<UserDal>().CheckDuplicateEmail(userId, email);
        }

        public IList<User> GetSSPMember(int userID, int systemId, int roleId)
        {
            return SingletonIpl.GetInstance<UserDal>().GetSSPMember(userID, systemId, roleId);
        }
    }
}
