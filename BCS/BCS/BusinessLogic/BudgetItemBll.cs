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
    public class BudgetItemBll : BaseBll, IBusinessLogic<BudgetItem>
    {
        public BudgetItem Get(int id)
        {
            return SingletonIpl.GetInstance<BudgetItemDal>().Get(id);
        }

        public IList<BudgetItem> Get(BaseListParam param, out int? totalRecord)
        {
            throw new NotImplementedException();
        }

        public int Save(BudgetItem obj, int userID)
        {
            return SingletonIpl.GetInstance<BudgetItemDal>().Save(obj, userID);
        }

        public bool Delete(int objId, int userID)
        {
            throw new NotImplementedException();
        }

        public IList<BudgetItem> GetBudgetItemByBudgetTabId(int budgetId, int budgetTabId, string budgetItemRow, int userId)
        {
            return SingletonIpl.GetInstance<BudgetItemDal>().GetBudgetItemByBudgetTabId(budgetId, budgetTabId, budgetItemRow, userId);
        }

        public void AddDefaultValueByBudgetTab(int budgetId, int budgetTabId, string budgetItemRow, int userId)
        {
            SingletonIpl.GetInstance<BudgetItemDal>().AddDefaultValueByBudgetTab(budgetId, budgetTabId, budgetItemRow, userId);
        }

        public void DeleteByCategorySettingId(int categorySettingId, int userId)
        {
            SingletonIpl.GetInstance<BudgetItemDal>().DeleteByCategorySettingId(categorySettingId, userId);
        }
    }
}
