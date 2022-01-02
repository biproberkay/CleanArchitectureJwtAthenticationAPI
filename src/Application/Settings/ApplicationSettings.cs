using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Settings
{
    public class ApplicationSettings
    {
        public string ConnectionString { get; set; }
        public IdentitySettings IdentitySettings { get; set; }
        public JwtSettings JwtSettings { get; set; }
        public MailSettings MailSettings { get; set; }
    }
}
