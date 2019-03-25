using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentHub.Entities.Utilities
{
    public static class CultureInfoHelper
    {
        public static string GetFullName(string firstName, string lastName, string cultureInfoCode)
        {
            var format = "{0} {1}";
            if (cultureInfoCode.ToLower() == "vi-vn")
            {
                format = "{1} {0}";
            }

            return string.Format(format, firstName, lastName);
        }
    }
}
