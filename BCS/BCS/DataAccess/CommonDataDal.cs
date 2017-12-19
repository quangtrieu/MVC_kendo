using BCS.Entity;
using BCS.Framework.Commons;
using BCS.Framework.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BCS.DataAccess
{
    public class CommonDataDal : BaseDal<ADOProvider>, IDataAccess<CommonData>
    {
        public CommonData Get(int id)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store get category by id
                var result = UnitOfWork.Procedure<CommonData>("cmm_commonData_GetById", new { Id = id }).FirstOrDefault();

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

        public IList<CommonData> GetCommonDataByCode(string code)
        {
            try
            {
                // set write connection string
                UnitOfWork.SetWriteConnectionString(false);

                // call store get category by id
                var result = UnitOfWork.Procedure<CommonData>("cmm_CommonData_GetByCode", new { DataCode = code }).ToList();

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

        public IList<CommonData> Get(BaseListParam param, out int? totalRecord)
        {
            throw new NotImplementedException();
        }

        public int Save(CommonData obj, int userID)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int objId, int userID)
        {
            throw new NotImplementedException();
        }
    }
}
