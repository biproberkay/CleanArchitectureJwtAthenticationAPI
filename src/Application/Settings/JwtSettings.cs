using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Settings
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public DateTime NotBefore { get; set; }
        public DateTime Expires { get; set; }

        //public IEnumerable<string> ValisIssuers { get; set; }
        //public IEnumerable<string> ValidAuidiences { get; set; }
    }
}
