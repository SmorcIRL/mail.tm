using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SmorcIRL.TempMail.Tests
{
    public partial class ApiTests : IDisposable
    {
        private readonly SmtpClient _smtpClient;
        private readonly MailAddress _from;
        private readonly string _login;
        private readonly string _password;
        private readonly MailClient _mailClient;

        public ApiTests()
        {
            var login = GetEnvironmentVariableOrThrow("TM_SMTP_LOGIN");
            var pass = GetEnvironmentVariableOrThrow("TM_SMTP_PASSWORD");
            var host = GetEnvironmentVariableOrThrow("TM_SMTP_HOST");
            var port = GetEnvironmentVariableOrThrow("TM_SMTP_PORT");
            var from = GetEnvironmentVariableOrThrow("TM_SMTP_FROM");
            var apiUri = GetEnvironmentVariableOrThrow("TM_API_URI");
            
            _login = Guid.NewGuid().ToString();
            _password = Guid.NewGuid().ToString();

            _smtpClient = new SmtpClient
            {
                Credentials = new NetworkCredential(login, pass),
                Port = int.Parse(port),
                Host = host,
                EnableSsl = true,
            };
            
            _from = new MailAddress(from);
            _mailClient = new MailClient(new Uri(apiUri));
        }

        public void Dispose()
        {   
            _smtpClient.Dispose();

            try
            {
                _mailClient.DeleteAccount().Wait();
            }
            catch
            {
            }
        }

        private static string GetEnvironmentVariableOrThrow(string variable)
        {
            return Environment.GetEnvironmentVariable(variable) ?? throw new InvalidOperationException($"Missing environment variable \"{variable}\"");
        }
    }

    public partial class ApiTests
    {
        [Test, Order(1)]
        public async Task GetDomainsTest()
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
     
            var client = new MailClient();
     
            var domains = await client.GetAvailableDomains();
     
            Assert.IsNotEmpty(domains);
     
            var domainFromList = domains[0];
            var domain = await client.GetDomain(domainFromList.Id);
     
            Assert.AreEqual(domainFromList.Id, domain.Id);
            Assert.AreEqual(domainFromList.Domain, domain.Domain);
            Assert.AreEqual(domainFromList.CreatedAt, domain.CreatedAt);
            Assert.AreEqual(domainFromList.IsActive, domain.IsActive);
            Assert.AreEqual(domainFromList.IsPrivate, domain.IsPrivate);
     
            var firstAvailableDomainName = await client.GetFirstAvailableDomainName();
            Assert.IsTrue(domains.Any(x => x.Domain == firstAvailableDomainName));
        }
        
        [Test, Order(2)]
        public async Task LoginAccountTest()
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            
            var address = $"{_login}@{await _mailClient.GetFirstAvailableDomainName()}";

            Assert.ThrowsAsync<HttpRequestException>(async () => await _mailClient.Login(address, _password));

            await _mailClient.Register(address, _password);
            await _mailClient.Login(_mailClient.Email, _password);
                
            var info = await _mailClient.GetAccountInfo();

            Assert.AreEqual(address, info.Address);
        }
        
        [Test, Order(3)]
        public async Task MessageProcessingTest()
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            
            var messages = await _mailClient.GetAllMessages();
            Assert.IsEmpty(messages);

            messages = await _mailClient.GetMessages(1);
            Assert.IsEmpty(messages);

            const string body = "fbd49602-39ee-4fea-b555-c4796d256a2d";
            await _smtpClient.SendMailAsync(new MailMessage(_from, new MailAddress(_mailClient.Email))
            {
                Body = body,
            });

            await Task.Delay(TimeSpan.FromSeconds(10));

            messages = await _mailClient.GetAllMessages();
            Assert.IsTrue(messages.Length == 1);

            var message = messages.First();

            var source = await _mailClient.GetMessageSource(message.Id);
            Assert.IsTrue(source.Data.Contains(body));
            
            var messageById = await _mailClient.GetMessage(message.Id);

            Assert.AreEqual(message.Id, messageById.Id);
            Assert.AreEqual(message.MessageId, messageById.MessageId);
            Assert.AreEqual(message.AccountId, messageById.AccountId);
            Assert.AreEqual(message.CreatedAt, messageById.CreatedAt);
            Assert.IsFalse(messageById.Seen);

            await _mailClient.MarkMessageAsSeen(message.Id, true);

            await Task.Delay(TimeSpan.FromSeconds(10));
            messageById = await _mailClient.GetMessage(message.Id);
            Assert.IsTrue(messageById.Seen);

            await _mailClient.MarkMessageAsSeen(message.Id, false);

            await Task.Delay(TimeSpan.FromSeconds(10));
            messageById = await _mailClient.GetMessage(message.Id);
            Assert.IsFalse(messageById.Seen);
            Assert.AreNotEqual(message.UpdatedAt, messageById.UpdatedAt);

            await _mailClient.DeleteMessage(message.Id);
            Assert.ThrowsAsync<HttpRequestException>(async () => await _mailClient.GetMessage(message.Id));
        }

        [Test, Order(4)]
        public async Task UnauthorizedTest()
        {
            await Task.Delay(TimeSpan.FromSeconds(10));

            var client = new MailClient();

            Assert.ThrowsAsync<HttpRequestException>(async () => await client.GetAccountInfo());
        }

        [Test, Order(5)]
        public async Task FailedLoginTest()
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            
            var info1 = await _mailClient.GetAccountInfo();

            Assert.ThrowsAsync<HttpRequestException>(async () => await _mailClient.Login("wrong", "creds"));
            var info2 = await _mailClient.GetAccountInfo();

            Assert.AreEqual(info1.Id, info2.Id);
            Assert.AreEqual(info1.Address, info2.Address);
        }
        
        [Test, Order(6)]
        public async Task DeleteAccountTest()
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
                    
            var info = await _mailClient.GetAccountInfo();

            await _mailClient.DeleteAccount();
        
            Assert.ThrowsAsync<HttpRequestException>(async () => await _mailClient.Login(info.Address, _password));
        }
    }
}