using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManagement
{
    public class OpenAMConfiguration
    {
        static OpenAMConfiguration()
        {
            ClientId = GetAppSetting("ClientID");
            ClientSecret = GetAppSetting("ClientSecret");
            OpenAMHost = GetAppSetting("OpenAMHost");
            Scope = GetAppSetting("Scope");
            ApiHost = GetAppSetting("ApiHost");
        }

        private static string GetAppSetting(string name)
        {
            return System.Configuration.ConfigurationManager.AppSettings[name];
        }

        public static string ClientSecret { get; private set; }


        public static string ApiHost { get; private set; }

        public static string Scope { get; private set; }

        public static string OpenAMHost { get; private set; }

        public static string ClientId { get; private set; }
    }
}
