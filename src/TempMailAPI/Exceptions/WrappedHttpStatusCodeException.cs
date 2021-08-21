using System;
using System.Net;

namespace SmorcIRL.TempMail.Exceptions
{
    public class WrappedHttpStatusCodeException : Exception
    {   
        public HttpStatusCode StatusCode { get; }
        
        public WrappedHttpStatusCodeException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}