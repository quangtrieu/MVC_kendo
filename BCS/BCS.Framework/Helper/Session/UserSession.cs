using System;
using System.Linq;

namespace BCS.Framework.Helper.Session
{
    [Serializable]
    public class UserSession
    {
        /// <summary>
        /// User ID in Tenant
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// User ID in CRM
        /// </summary>
        public int SSOId { get; set; }

        /// <summary>
        /// User Display Name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// User Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Current Company Name
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Current Company Schema
        /// </summary>
        public string CurrentSchema { get; set; }

        /// <summary>
        /// Current Company ID
        /// </summary>
        public int CurrentCompanyId { get; set; }

        /// <summary>
        /// Current User Avatar 
        /// </summary>
        public string Avartar { get; set; }

        private string _langCode;

        public string LangCode
        {
            get { return this._langCode; }
            set { this._langCode = value; }
        }


        public string SessionId { get; set; }

        private string _timeZone = TimeZoneInfo.Utc.Id;

        public bool HasSetTimeZone { get; set; }

        public string TimeZone
        {
            get { return _timeZone; }
            set
            {
                _timeZone = (string.IsNullOrEmpty(value) || TimeZoneInfo.GetSystemTimeZones().All(p => p.Id != value))
                                ? TimeZoneInfo.Utc.Id
                                : value;
            }
        }

        public double TotalMinuteByTimeZone
        {
            get { return TimeZoneInfo.FindSystemTimeZoneById(TimeZone).BaseUtcOffset.TotalMinutes; }
        }

    }
}
