using System;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using SmorcIRL.TempMail.Messaging;

namespace SmorcIRL.TempMail.Tests
{
    public class ResultTests
    {
        [Test]
        public void MissingMessageTest()
        {
            Assert.Throws<ArgumentNullException>(() => new Result(null));
        }
        
        [Test]
        public void MissingDataTest()
        {
            _ = new Result<object>(new HttpResponseMessage(HttpStatusCode.OK), new object());
            Assert.Throws<ArgumentNullException>(() => new Result<object>(new HttpResponseMessage(HttpStatusCode.OK), null));
        }
    }
}