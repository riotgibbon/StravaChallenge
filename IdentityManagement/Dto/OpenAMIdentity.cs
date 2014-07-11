using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManagement.Dto
{
    public class OpenAMIdentity
    {
        public Token token { get; set; }
        public List<string> roles { get; set; }
        public List<Attribute> attributes { get; set; }
    }

    public class Token
    {
        public string tokenId { get; set; }
    }

    public class Attribute
    {
        public List<string> values { get; set; }
        public string name { get; set; }
    }

    
}
