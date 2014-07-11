using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManagement.Dto
{
    public class StravaIdentity
    {
        public Token token { get; set; }
        public List<string> roles { get; set; }
        public List<Attribute> attributes { get; set; }
    }

   

    
    
}
