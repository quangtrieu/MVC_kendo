using BCS.DataAccess;
using BCS.Entity;
using BCS.Framework.Commons;
using BCS.Framework.Singleton;
using System;
using System.Collections.Generic;

namespace BCS.BusinessLogic
{
    public class CategoryBll : BaseBll, IBusinessLogic<Category>
    {
        public Category Get(int id)
        {
            return SingletonIpl.GetInstance<CategoryDal>().Get(id);
        }

        public IList<Category> Get(BaseListParam param, out int? totalRecord)
        {
            throw new NotImplementedException();
        }

        public int Save(Category obj, int userId)
        {
            return SingletonIpl.GetInstance<CategoryDal>().Save(obj, userId);
        }

        public bool Delete(int objId, int userID)
        {
            throw new NotImplementedException();
        }

        public IList<Category> GetCategoryByUserId(int userId)
        {
            return SingletonIpl.GetInstance<CategoryDal>().GetCategoryByUserId(userId);
        }

        public IList<Category> GetCategoryByParentId(int userId, int id)
        {
            return SingletonIpl.GetInstance<CategoryDal>().GetCategoryByParentId(userId, id);
        }

        public IList<Category> GetCategoryByParentName(string parentName, int userID)
        {
            return SingletonIpl.GetInstance<CategoryDal>().GetCategoryByParentName(parentName, userID);
        }

        public IList<Category> GetDefaultSectionByUser(int userId, string sectionCode)
        {
            return SingletonIpl.GetInstance<CategoryDal>().GetDefaultSectionByUser(userId, sectionCode);
        }

        public bool DeleteAllCategoryByUser(int userId)
        {
            return SingletonIpl.GetInstance<CategoryDal>().DeleteAllCategoryByUser(userId);
        }

        public bool DeleteAllCategoryChildrenByParentCategoryId(int parentCategoryId, int userId)
        {
            return SingletonIpl.GetInstance<CategoryDal>().DeleteAllCategoryChildrenByParentId(parentCategoryId, userId);
        }
    }
}
