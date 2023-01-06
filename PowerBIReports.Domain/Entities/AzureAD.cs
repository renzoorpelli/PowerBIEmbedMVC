using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBIReports.Domain.Entities
{
    public class AzureAD
    {
        public string AuthenticationMode { get; set; }
        public string AuthorityUrl { get; set; }
        public string ClientId { get; set; }
        public string TenantId { get; set; }
        public string[] ScopeBase { get; set; }
        public string PbiUsername { get; set; }
        public string PbiPassword { get; set; }
        public string ClientSecret { get; set; }
    }
}
