using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBIReports.Domain.Entities
{
    public class EmbedReport
    {
        public Guid ReportId { get; set; }
        public string ReportName { get; set; }
        public string EmbedUrl { get; set; }
    }
}
