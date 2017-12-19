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
    public class CategorySettingBll : BaseBll, IBusinessLogic<CategorySetting>
    {
        public CategorySetting Get(int id)
        {
            return SingletonIpl.GetInstance<CategorySettingDal>().Get(id);
        }

        public IList<CategorySetting> Get(BaseListParam param, out int? totalRecord)
        {
            throw new NotImplementedException();
        }

        public int Save(CategorySetting obj, int userID)
        {
            return SingletonIpl.GetInstance<CategorySettingDal>().Save(obj, userID);
        }

        public bool Delete(int objId, int userID)
        {
            throw new NotImplementedException();
        }

        public IList<CategorySetting> GetCategorySettingByBudgetId(int id, int userID)
        {
            return SingletonIpl.GetInstance<CategorySettingDal>().GetCategorySettingByBudgetId(id, userID);
        }

        public IList<CategorySetting> GetCategorySettingByParentId(int id, int budgetId, int userID)
        {
            return SingletonIpl.GetInstance<CategorySettingDal>().GetCategorySettingByParentId(id, budgetId, userID);
        }

        public bool CheckDuplicateCategorySetting(CategorySetting categorySetting, int userID)
        {
            return SingletonIpl.GetInstance<CategorySettingDal>().CheckDuplicateCategorySetting(categorySetting, userID);
        }

        public IList<CategorySetting> GetCategorySettingByParentName(string categoryName, int budgetId, int userID)
        {
            return SingletonIpl.GetInstance<CategorySettingDal>().GetCategorySettingByParentName(categoryName, budgetId, userID);
        }

        public bool DeleteByBudgetId(int budgetId, int userID)
        {
            return SingletonIpl.GetInstance<CategorySettingDal>().DeleteByBudgetId(budgetId, userID);
        }

        public bool DeleteByParentCategoryId(int budgetId, int parentCategoryId, int userID)
        {
            return SingletonIpl.GetInstance<CategorySettingDal>().DeleteByParentCategoryId(budgetId, parentCategoryId, userID);
        }
    }
}
