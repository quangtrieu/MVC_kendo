using BCS.Framework.Utilities;
using RestSharp;
using System;

namespace BCS.Web.WebUtils
{
    public class BCSWebUtils
    {
        /// <summary>
        /// Create rest client call web service.
        /// </summary>
        private static RestClient _client = new RestClient(Utils.GetSetting<string>("webServiceUrl", "http://bcsws.smartsystemspro.com/"));
        
        /// <summary>
        /// Get setting api get user by token
        /// </summary>
        private static string API_GetUserByToken = Utils.GetSetting<string>("GetUserByToken", "serviceApi/Users/getUserByToken/");

        /// <summary>
        /// get restaurant default setting from SSP
        /// </summary>
        private static string API_GetCategory = "serviceApi/CategorySetting/GetCategorySettingByRestCode/";

        /// <summary>
        /// Service get data actual sales & cogs from SSP
        /// </summary>
        private static string API_GetActualByRestCode = "serviceApi/BudgetCategory/GetActualByRestCode/";

        /// <summary>
        /// Method get user by token id from web service api
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public static IRestResponse GetUserByTokenId(string tokenId)
        {
            // check parameter input method
            if (string.IsNullOrEmpty(tokenId))
            {
                return null;
            }

            var request = new RestRequest(API_GetUserByToken, Method.GET);
            request.AddHeader("Token", tokenId);
            request.AddParameter("tokenId", tokenId);

            var result = _client.Execute(request);
            return result;
        }

        /// <summary>
        /// get all category from ssp system
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="restCode"></param>
        /// <returns></returns>
        public static IRestResponse GetAllCategory(string tokenId, string restCode)
        {
            // check parameter input method
            if (string.IsNullOrEmpty(restCode))
            {
                return null;
            }

            var request = new RestRequest(API_GetCategory, Method.GET);
            request.AddHeader("Token", tokenId);
            request.AddParameter("restCode", restCode);

            var result = _client.Execute(request);
            return result;
        }

        /// <summary>
        /// get all sales actual from ssp system
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="restCode"></param>
        /// <returns></returns>
        public static IRestResponse GetDataActualFromSSP(string tokenId, string restCode, DateTime budgetStartDate, int headerIndex, int weekRange, bool budgetType)
        {
            // check parameter input method
            if (string.IsNullOrEmpty(restCode))
            {
                return null;
            }

            // create new request
            var request = new RestRequest(API_GetActualByRestCode, Method.GET);
            request.AddHeader("Token", tokenId);
            request.AddParameter("RestCode", restCode);
            request.AddParameter("BudgetStartDate", budgetStartDate);
            request.AddParameter("HeaderIndex", headerIndex);
            request.AddParameter("WeekRange", weekRange);
            request.AddParameter("BudgetType", budgetType);

            // send request to server
            var result = _client.Execute(request);

            // return result
            return result;
        }
    }
}