using System;
using System.Linq;
using BCS.Framework.SecurityServices.Entity;

namespace BCS.Framework.SecurityServices
{
    public static class SecurityUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="allowUsers"></param>
        /// <returns></returns>
        public static bool IsAuthorizeAction(UserInfo user,string allowUsers)
        {
            if (user==null)
            {
                return false;
            }

            var allowUsersSplit = SplitString(allowUsers);


            // Check User is Authorize CurrentUser
            if (allowUsersSplit.Length > 0 && allowUsersSplit.Contains(user.UserName, StringComparer.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        internal static string[] SplitString(string original)
        {
            if (String.IsNullOrEmpty(original))
            {
                return new string[0];
            }

            var split = from piece in original.Split(',')
                        let trimmed = piece.Trim()
                        where !String.IsNullOrEmpty(trimmed)
                        select trimmed;
            return split.ToArray();
        }
    }
}