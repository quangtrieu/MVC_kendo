using System.Activities.Statements;

namespace BCS.Framework.Constants
{
    public static class Constant
    {
        public const string DATE_FORMAT = "MM-dd-yyyy";
        public const string TIME_SPAN_FORMAT = "HH:mm:ss";
        public const string TIME_SPAN_FORMAT_AM_PM = "hh:mm tt";
        public const string REG_TIMESPAN = "^(0[1-9]|1[0-2]):[0-5][0-9] [ap]m$";

        // const
        public const string BCS_CONTEXT = "BCS.CONTEXT";
        public const string BCS_CURRENT_USER = "CurrentUserInfo";
        //public const string BCS_SUPPORT_USER = "SupportUserInfo";
        public const string BCS_MEMBER_PREFIX = "BCS.MEMBER_INFO";
        public const string BCS_MEMBER_SEPARATOR = "BCS.MEMBER_SEP";

        public const string BCS_LOGIN_INFO = "BCS.USER";
        public const string BCS_LOGIN_USERNAME = "BCS.LOG.UID";
        public const string BCS_LOGIN_PASSWORD = "BCS.LOG.PWD";


        /*
         * Database Config 
         */
        public const string DATABASE_CURRENT = "BCSRead";
        public const string DATABASE_INVENTORY = "BCSWrite";
        /**End**/

        //System Configuration 
        public const string ITEM_PER_PAGE = "ITEM_PER_PAGE";

        // Operations const
        public const string OPERATION_STARTSWITH = "startswith";
        public const string OPERATION_ENDSWITH = "endswith";
        public const string OPERATION_CONTAINS = "contains";
        public const string OPERATION_EQ = "eq";
        public const string OPERATION_NEQ = "neq";
        public const string OPERATION_AND = " and ";
        public const string OPERATION_SEPARATOR = "~";
        public const string OPERATION_AND_SEPARATOR = "~and~";
        

        public const string OPERATION_PERCEN_END = "%'";
        public const string OPERATION_PERCEN_START = "%";

        public const string OPERATION_LIKE = " like '";
        public const string OPERATION_EQUAL = " = '";
        public const string OPERATION_NEQUAL = " <> '";
        public const string OPERATION_SEP =  "' ";

        public const string SEPARATOR_COMMA = ",";

        public const string GROUP_SUPER = "1";
        public const string GROUP_TECNICAL = "2";
        public const string GROUP_SITEADMIN = "3";

        public const string ROLE_SITE_ADMIN = "SiteAdmin";
        public const string ROLE_MANAGER = "Manager";
        public const string ROLE_EMPLOYEE = "Employee";

        public static string DATABASE_DEFAULT { get; set; }

        public const string MEAL_PERIOD_OFF = "OFF";
        public const string MEAL_PERIOD_OPEN = "OPEN";


        public const string SCHEDULER_CHARACTOR_SLIPT_PROPERTY = "@@" ;
        public const string SCHEDULER_CHARACTOR_SLIPT_ENTITY = "_|";


    }
}