using System;
using System.Net;
using System.Net.Http;
using SmorcIRL.TempMail.Exceptions;

namespace SmorcIRL.TempMail.Messaging
{
    internal class Result
    {
        public HttpResponseMessage Message { get; set; }

        public HttpStatusCode? StatusCode => Message.StatusCode;

        public bool IsSuccessStatusCode => Message.IsSuccessStatusCode;

        public void Throw(string message)
        {
            if (StatusCode == null || IsSuccessStatusCode)
            {
                throw new InvalidOperationException();
            }

            throw new WrappedHttpStatusCodeException(StatusCode.Value, message);
        }

        public void ThrowUnknown()
        {
            Throw("Undocumented http response status code");
        }
    }

    internal class Result<T> : Result
    {
        public T Data { get; set; }
    }
}