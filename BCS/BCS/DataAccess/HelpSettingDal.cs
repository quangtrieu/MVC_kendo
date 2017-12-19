using BCS.Entity;
using BCS.Framework.Commons;
using BCS.Framework.Data;
using BCS.Framework.Helper;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCS.DataAccess
{
    public class HelpSettingDal : BaseDal<ADOProvider>, IDataAccess<HelpSetting>
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
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserId", userId);
                param.Add("@XML", XmlHelper.SerializeXML<HelpSetting>(obj));
                param.Add("@HelpSettingId", obj.HelpSettingId, DbType.Int32, ParameterDirection.InputOutput);

                // set write connection string
                UnitOfWork.SetWriteConnectionString(true);

                // call store save rest active code
                UnitOfWork.ProcedureExecute("set_HelpSetting_Save", param);

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return param.Get<int>("@HelpSettingId");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0;
            }
        }

        public bool Delete(int objId, int userID)
        {
            throw new NotImplementedException();
        }

        public HelpSetting GetByUserIdAndHelpSettingDataId(int userId, int helpSettingDataId)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store get budget by id
                var result = UnitOfWork.Procedure<HelpSetting>("set_HelpSetting_GetByUserIdAndHelpSettingDataId", new { UserId = userId, HelpSettingDataId = helpSettingDataId }).FirstOrDefault();

                // clear connection string
                UnitOfWork.ConnectionString = null;

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }
    }
}
