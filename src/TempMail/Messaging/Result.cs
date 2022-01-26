using System;
using System.Net.Http;

namespace SmorcIRL.TempMail.Messaging
{
    internal class Result
    {
        public HttpResponseMessage Message { get; }

        public Result(HttpResponseMessage message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }

    internal class Result<T> : Result where T : class
    {
        public T Data { get; }

        public Result(HttpResponseMessage message, T data) : base(message)
        {
            if (message.IsSuccessStatusCode)
            {
                Data = data ?? throw new ArgumentNullException(nameof(data));
            }
        }
    }
}