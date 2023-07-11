using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Models.API
{
    public struct APIMessage
    {
        public HttpStatusCode StatusCode { get; set; }
        public object Content { get; set; }

        public APIMessage(HttpStatusCode statusCode, object content)
        {
            StatusCode = statusCode;
            Content = content;
        }
    }
}
