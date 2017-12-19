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
    public class CommonBll : BaseBll, IBusinessLogic<CommonData>
    {
        public CommonData Get(int id)
        {
            return SingletonIpl.GetInstance<CommonDataDal>().Get(id);
        }

        public IList<CommonData> GetCommonDataByCode(string code)
        {
            return SingletonIpl.GetInstance<CommonDataDal>().GetCommonDataByCode(code);
        }

        public IList<CommonData> Get(BaseListParam param, out int? totalRecord)
        {
            throw new NotImplementedException();
        }

        public int Save(CommonData obj, int userID)
        {
            return SingletonIpl.GetInstance<CommonDataDal>().Save(obj, userID);
        }

        public bool Delete(int objId, int userID)
        {
            throw new NotImplementedException();
        }
    }
}
