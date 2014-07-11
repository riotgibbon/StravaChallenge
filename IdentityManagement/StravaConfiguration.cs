using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManagement
{
    public class StravaConfiguration
    {
        static StravaConfiguration()
        {
            ClientId = GetAppSetting("ClientID");
            ClientSecret = GetAppSetting("ClientSecret");
            StravaHost = GetAppSetting("StravaHost");
            Scope = GetAppSetting("Scope");
            ///ApiHost = GetAppSetting("ApiHost");
        }

        private static string GetAppSetting(string name)
        {
            return System.Configuration.ConfigurationManager.AppSettings[name];
        }

        public static string ClientSecret { get; private set; }


        public static string ApiHost { get; private set; }

        public static string Scope { get; private set; }

        public static string StravaHost { get; private set; }

        public static string ClientId { get; private set; }
    }
}
