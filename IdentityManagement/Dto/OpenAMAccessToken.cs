
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManagement.Dto
{
    public class OpenAMAccessToken
    {
            public List<string> scope { get; set; }
            public string realm { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string access_token { get; set; }
            public string refresh_token { get; set; }
            public string error { get; set; }
            public string error_description { get; set; }
    }

    public class OpenAMAccessTokenInfo : OpenAMAccessToken
    {
        public string mail { get; set; }
        public string uid { get; set; }
        public string cn { get; set; }
    }
    
}
