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
    public class HelpSettingBll : BaseBll, IBusinessLogic<HelpSetting>
    {
        public HelpSetting Get(int id)
        {
            throw new NotImplementedException();
        }

        public IList<HelpSetting> Get(BaseListParam param, out int? totalRecord)
        {
            throw new NotImplementedException();
        }

        public int Save(HelpSetting obj, int userId)
        {
            return SingletonIpl.GetInstance<HelpSettingDal>().Save(obj, userId);
        }

        public bool Delete(int objId, int userID)
        {
            throw new NotImplementedException();
        }

        public HelpSetting GetByUserIdAndHelpSettingDataId(int userId, int helpSettingDataId)
        {
            return SingletonIpl.GetInstance<HelpSettingDal>().GetByUserIdAndHelpSettingDataId(userId, helpSettingDataId);
        }
    }
}
