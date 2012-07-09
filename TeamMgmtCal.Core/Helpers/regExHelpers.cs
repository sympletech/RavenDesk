using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TeamMgmtCal.Core.Helpers
{
    public static class regExHelpers
    {
        public static bool IsValidEmail(string strIn)
        {
            return IsValidEmail(strIn, true);
        }
        /// <summary>
        /// Checks to ensure a valid e-mail format
        /// </summary>
        public static bool IsValidEmail(string strIn, bool AllowBlank)
        {
            if (strIn != "" && strIn != null)
            {
                // Return true if strIn is in valid e-mail format.
                return Regex.IsMatch(strIn,
                       @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                       @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
            }
            else
            {
                return AllowBlank;
            }
        }
    }
}
