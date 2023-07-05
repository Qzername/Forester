using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Models
{
    public struct APIMessage
    {
        public HttpStatusCode StatusCode { get; set; }  
        public string Message { get; set; }

        public APIMessage(HttpStatusCode statusCode, string message) 
        {
            StatusCode = statusCode;
            Message = message;
        }
    }
}
