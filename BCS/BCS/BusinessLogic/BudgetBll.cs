using BCS.DataAccess;
using BCS.Entity;
using BCS.Framework.Commons;
using BCS.Framework.Singleton;
using System;
using System.Collections.Generic;

namespace BCS.BusinessLogic
{
    public class BudgetBll : BaseBll, IBusinessLogic<Budget>
    {
        public Budget Get(int id)
        {
            return SingletonIpl.GetInstance<BudgetDal>().Get(id);
        }

        public IList<Budget> Get(BaseListParam param, out int? totalRecord)
        {
            throw new NotImplementedException();
        }

        public int Save(Budget obj, int userID)
        {
            return SingletonIpl.GetInstance<BudgetDal>().Save(obj, userID);
        }

        public bool Delete(int objId, int userID)
        {
            return SingletonIpl.GetInstance<BudgetDal>().Delete(objId, userID);
        }

        public IList<Budget> GetBudgetByUserId(int userId)
        {
            return SingletonIpl.GetInstance<BudgetDal>().GetBudgetByUserId(userId);
        }

        public int CloneBudget(int id, string budgetName, int userID)
        {
            return SingletonIpl.GetInstance<BudgetDal>().CloneBudget(id, budgetName, userID);
        }

        public IList<Budget> GetBudgetDeletedByUserId(int userId)
        {
            return SingletonIpl.GetInstance<BudgetDal>().GetBudgetDeletedByUserId(userId);
        }
    }
}
