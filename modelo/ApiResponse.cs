using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace modelo
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public ApiResponse(HttpStatusCode code, string message, object details = null)
        {
            StatusCode = (int)code;
            Message = message;
            Data = details;
        }
    }
}
