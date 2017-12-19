using System;
using System.Data;
using System.Data.SqlClient;
using BCS.Framework.Configuration;

namespace BCS.Framework.Data
{
    /// <summary>
    /// help to gets connection string in .config file
    /// </summary>
    public class DataHelper
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string GetConnectionString(string key = "BCSRead")
        {
            return Config.GetConfigByKey(key);
        }
        
        /// <summary>
        /// Get Location Upload
        /// </summary>
        /// <returns></returns>
        public static string GetLocationUpload(string key = "LocationUpload")
        {
            return Config.GetConfigByKey(key);
        }

       public static string GetProviderName(string key = "BCSProviderName")
        {
            return Config.GetConfigByKey(key);
        }

        public static string GetProviderManifest(string key = "BCSProviderManifest")
        {
            return Config.GetConfigByKey(key);
        }
        
        /// <summary>
        /// Converts the type.
        /// </summary>
        /// <param name="dbType">Type of the db.</param>
        /// <returns></returns>
        public static Type ConvertType(DbType? dbType)
        {
            Type toReturn = typeof(DBNull);

            switch (dbType)
            {
                case DbType.String:
                    toReturn = typeof(string);
                    break;

                case DbType.UInt64:
                    toReturn = typeof(UInt64);
                    break;

                case DbType.Int64:
                    toReturn = typeof(Int64);
                    break;

                case DbType.Int32:
                    toReturn = typeof(Int32);
                    break;

                case DbType.UInt32:
                    toReturn = typeof(UInt32);
                    break;

                case DbType.Single:
                    toReturn = typeof(float);
                    break;

                case DbType.Date:
                    toReturn = typeof(DateTime);
                    break;

                case DbType.DateTime:
                    toReturn = typeof(DateTime);
                    break;

                case DbType.Time:
                    toReturn = typeof(DateTime);
                    break;

                case DbType.StringFixedLength:
                    toReturn = typeof(string);
                    break;

                case DbType.UInt16:
                    toReturn = typeof(UInt16);
                    break;

                case DbType.Int16:
                    toReturn = typeof(Int16);
                    break;

                case DbType.SByte:
                    toReturn = typeof(byte);
                    break;

                case DbType.Object:
                    toReturn = typeof(object);
                    break;

                case DbType.AnsiString:
                    toReturn = typeof(string);
                    break;

                case DbType.AnsiStringFixedLength:
                    toReturn = typeof(string);
                    break;

                case DbType.VarNumeric:
                    toReturn = typeof(decimal);
                    break;

                case DbType.Currency:
                    toReturn = typeof(double);
                    break;

                case DbType.Binary:
                    toReturn = typeof(byte[]);
                    break;

                case DbType.Decimal:
                    toReturn = typeof(decimal);
                    break;

                case DbType.Double:
                    toReturn = typeof(Double);
                    break;

                case DbType.Guid:
                    toReturn = typeof(Guid);
                    break;

                case DbType.Boolean:
                    toReturn = typeof(bool);
                    break;
            }

            return toReturn;
        }
    }
}
