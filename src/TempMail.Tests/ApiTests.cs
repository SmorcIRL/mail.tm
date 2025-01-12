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

            Assert.That(domains, Is.Not.Empty);
     
            var domainFromList = domains[0];
            var domain = await client.GetDomain(domainFromList.Id);
     
            Assert.That(domainFromList.Id, Is.EqualTo(domain.Id));
            Assert.That(domainFromList.Domain, Is.EqualTo(domain.Domain));
            Assert.That(domainFromList.CreatedAt, Is.EqualTo(domain.CreatedAt));
            Assert.That(domainFromList.IsActive, Is.EqualTo(domain.IsActive));
            Assert.That(domainFromList.IsPrivate, Is.EqualTo(domain.IsPrivate));
     
            var firstAvailableDomainName = await client.GetFirstAvailableDomainName();
            Assert.That(domains.Any(x => x.Domain == firstAvailableDomainName), Is.True);
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

            Assert.That(address, Is.EqualTo(info.Address));
        }
        
        [Test, Order(3)]
        public async Task MessageProcessingTest()
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            
            var messages = await _mailClient.GetAllMessages();
            Assert.That(messages, Is.Empty);

            messages = await _mailClient.GetMessages(1);
            Assert.That(messages, Is.Empty);

            var body = Guid.NewGuid().ToString();
            await _smtpClient.SendMailAsync(new MailMessage(_from, new MailAddress(_mailClient.Email))
            {
                Body = body,
            });

            await Task.Delay(TimeSpan.FromSeconds(10));

            messages = await _mailClient.GetAllMessages();
            Assert.That(messages.Length == 1, Is.True);

            var message = messages.First();

            var source = await _mailClient.GetMessageSource(message.Id);
            Assert.That(source.Data.Contains(body), Is.True);

            var messageById = await _mailClient.GetMessage(message.Id);

            Assert.That(messageById.BodyText.Contains(body), Is.True);
            Assert.That(messageById.BodyHtml.First().Contains(body), Is.True);

            Assert.That(message.Id, Is.EqualTo(messageById.Id));
            Assert.That(message.MessageId, Is.EqualTo(messageById.MessageId));
            Assert.That(message.AccountId, Is.EqualTo(messageById.AccountId));
            Assert.That(message.CreatedAt, Is.EqualTo(messageById.CreatedAt));
            Assert.That(messageById.Seen, Is.False);

            await _mailClient.MarkMessageAsSeen(message.Id, true);

            await Task.Delay(TimeSpan.FromSeconds(10));
            messageById = await _mailClient.GetMessage(message.Id);
            Assert.That(messageById.Seen, Is.True);

            await _mailClient.MarkMessageAsSeen(message.Id, false);

            await Task.Delay(TimeSpan.FromSeconds(10));
            messageById = await _mailClient.GetMessage(message.Id);
            Assert.That(messageById.Seen, Is.False);
            Assert.That(message.UpdatedAt, Is.Not.EqualTo(messageById.UpdatedAt));

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

            Assert.That(info1.Id, Is.EqualTo(info2.Id));
            Assert.That(info1.Address, Is.EqualTo(info2.Address));
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