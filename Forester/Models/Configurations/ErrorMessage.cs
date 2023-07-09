using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Models.Configurations
{
    public struct ErrorMessage
    {
        public HttpStatusCode StatusCode { get; set; }
        public string FriendlyMessage { get; set; }
        public string TechnicalMessage { get; set; }
    }
}
