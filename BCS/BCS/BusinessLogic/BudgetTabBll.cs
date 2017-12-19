using BCS.DataAccess;
using BCS.Entity;
using BCS.Framework.Commons;
using BCS.Framework.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCS.BusinessLogic
{
    public class BudgetTabBll : BaseBll, IBusinessLogic<BudgetTab>
    {
        public BudgetTab Get(int id)
        {
            return SingletonIpl.GetInstance<BudgetTabDal>().Get(id);
        }

        public IList<BudgetTab> Get(BaseListParam param, out int? totalRecord)
        {
            throw new NotImplementedException();
        }

        public int Save(BudgetTab obj, int userID)
        {
            return SingletonIpl.GetInstance<BudgetTabDal>().Save(obj, userID);
        }

        public bool Delete(int objId, int userID)
        {
            throw new NotImplementedException();
        }

        public IList<BudgetTab> GetBudgetTabByBudgetId(int budgetId, int userId)
        {
            return SingletonIpl.GetInstance<BudgetTabDal>().GetBudgetTabByBudgetId(budgetId, userId);
        }

        public bool DeleteBudgetTabByBudgetId(int budgetId, int userId)
        {
            return SingletonIpl.GetInstance<BudgetTabDal>().DeleteBudgetTabByBudgetId(budgetId, userId);
        }
    }
}
