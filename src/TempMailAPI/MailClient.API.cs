using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SmorcIRL.TempMail.Helpers;
using SmorcIRL.TempMail.Messaging;
using SmorcIRL.TempMail.Models;

namespace SmorcIRL.TempMail
{
    public sealed partial class MailClient
    {
        //========================================//

        public static async Task<MailClient> Login(string address, string password)
        {
            Ensure.IsPresent(address, nameof(address));
            Ensure.IsPresent(password, nameof(password));

            var result = await HttpClient.PostAsync<GetTokenRequest, TokenInfo>(CreateUri(Endpoints.PostToken), new GetTokenRequest
            {
                Address = address,
                Password = password,
            }).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode)
            {
                if (result.StatusCode == HttpStatusCode.Unauthorized)
                {
                    result.Throw("Authorization failed");
                }

                result.ThrowUnknown();
            }

            return new MailClient
            {
                Email = address,
                Password = password,
                AccountId = result.Data.Id,
                BearerToken = result.Data.Token,
            };
        }

        public static async Task<MailClient> Register(string address, string password)
        {
            Ensure.IsPresent(address, nameof(address));
            Ensure.IsPresent(password, nameof(password));

            var createAccountResult = await HttpClient.PostAsync<CreateAccountRequest, AccountInfo>(CreateUri(Endpoints.PostAccount), new CreateAccountRequest
            {
                Address = address,
                Password = password,
            }).ConfigureAwait(false);

            if (!createAccountResult.IsSuccessStatusCode)
            {
                if (createAccountResult.StatusCode == HttpStatusCode.BadRequest)
                {
                    createAccountResult.Throw("Invalid input");
                }

                if (createAccountResult.StatusCode == (HttpStatusCode)422)
                {
                    createAccountResult.Throw("Unprocessable entity");
                }

                createAccountResult.ThrowUnknown();
            }

            var tokenResult = await HttpClient.PostAsync<GetTokenRequest, TokenInfo>(CreateUri(Endpoints.PostToken), new GetTokenRequest
            {
                Address = address,
                Password = password,
            }).ConfigureAwait(false);

            if (!tokenResult.IsSuccessStatusCode)
            {
                if (tokenResult.StatusCode == HttpStatusCode.Unauthorized)
                {
                    tokenResult.Throw("Authorization failed");
                }

                tokenResult.ThrowUnknown();
            }

            return new MailClient
            {
                Email = address,
                Password = password,
                AccountId = tokenResult.Data.Id,
                BearerToken = tokenResult.Data.Token,
            };
        }

        public static async Task<MailClient> Register(string addressWithoutDomain, string domain, string password)
        {
            Ensure.IsPresent(addressWithoutDomain, nameof(addressWithoutDomain));
            Ensure.IsPresent(domain, nameof(domain));
            Ensure.IsPresent(password, nameof(password));

            return await Register($"{addressWithoutDomain}@{domain}", password).ConfigureAwait(false);
        }


        public static async Task<DomainInfo[]> GetAvailableDomains()
        {
            var result = await HttpClient
                .GetAsync<DomainInfo[]>(CreateUri(Endpoints.GetDomains))
                .ConfigureAwait(false);

            if (!result.IsSuccessStatusCode)
            {
                result.ThrowUnknown();
            }

            return result.Data;
        }

        public static async Task<DomainInfo> GetDomain(string id)
        {
            Ensure.IsPresent(id, nameof(id));

            var result = await HttpClient
                .GetAsync<DomainInfo>(CreateUri(Endpoints.GetDomain, id))
                .ConfigureAwait(false);

            if (!result.IsSuccessStatusCode)
            {
                if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    result.Throw("Resource not found");
                }

                result.ThrowUnknown();
            }

            return result.Data;
        }

        public static async Task<string> GetFirstAvailableDomainName()
        {
            var domains = await GetAvailableDomains().ConfigureAwait(false);

            return domains.FirstOrDefault()?.Domain;
        }

        //========================================//

        public async Task<AccountInfo> GetAccountInfo()
        {
            EnsureAlive();

            var result = await HttpClient
                .GetAsync<AccountInfo>(CreateUri(Endpoints.GetMe), BearerToken)
                .ConfigureAwait(false);

            if (!result.IsSuccessStatusCode)
            {
                if (result.StatusCode == HttpStatusCode.Unauthorized)
                {
                    result.Throw("Authorization failed");
                }

                if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    result.Throw("Resource not found");
                }

                result.ThrowUnknown();
            }

            return result.Data;
        }

        public async Task DeleteAccount()
        {
            EnsureAlive();

            var result = await HttpClient
                .DeleteAsync(CreateUri(Endpoints.DeleteAccount, AccountId), BearerToken)
                .ConfigureAwait(false);

            if (!result.IsSuccessStatusCode)
            {
                if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    result.Throw("Resource not found");
                }

                result.ThrowUnknown();
            }

            IsDeleted = true;
        }


        public async Task<MessageInfo[]> GetMessages(int page)
        {
            Ensure.IsPositive(page, nameof(page));

            EnsureAlive();

            var result = await HttpClient
                .GetAsync<MessageInfo[]>(CreateUri(Endpoints.GetMessages, page), BearerToken)
                .ConfigureAwait(false);

            if (!result.IsSuccessStatusCode)
            {
                if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    result.Throw("Resource not found");
                }

                result.ThrowUnknown();
            }

            return result.Data;
        }

        public async Task<MessageInfo[]> GetAllMessages()
        {
            var list = new List<MessageInfo>();

            for (var i = 1; i < int.MaxValue; i++)
            {
                var fromPage = await GetMessages(i);

                if (!fromPage.Any())
                {
                    break;
                }

                list.AddRange(fromPage);
            }

            return list.ToArray();
        }
        
        public async Task<MessageInfo> GetMessage(string id)
        {
            Ensure.IsPresent(id, nameof(id));

            EnsureAlive();

            var result = await HttpClient
                .GetAsync<MessageInfo>(CreateUri(Endpoints.GetMessage, id), BearerToken)
                .ConfigureAwait(false);

            if (!result.IsSuccessStatusCode)
            {
                if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    result.Throw("Resource not found");
                }

                result.ThrowUnknown();
            }

            return result.Data;
        }

        public async Task DeleteMessage(string id)
        {
            Ensure.IsPresent(id, nameof(id));

            EnsureAlive();

            var result = await HttpClient
                .DeleteAsync(CreateUri(Endpoints.DeleteMessage, id), BearerToken)
                .ConfigureAwait(false);

            if (!result.IsSuccessStatusCode)
            {
                if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    result.Throw("Resource not found");
                }

                result.ThrowUnknown();
            }
        }

        public async Task<MessageSource> GetMessageSource(string id)
        {
            Ensure.IsPresent(id, nameof(id));

            EnsureAlive();

            var result = await HttpClient
                .GetAsync<MessageSource>(CreateUri(Endpoints.GetSource, id), BearerToken)
                .ConfigureAwait(false);

            if (!result.IsSuccessStatusCode)
            {
                result.ThrowUnknown();
            }

            return result.Data;
        }

        public async Task MarkMessageAsSeen(string id, bool seen)
        {
            Ensure.IsPresent(id, nameof(id));

            EnsureAlive();

            var result = await HttpClient
                .PatchAsync(CreateUri(Endpoints.PatchMessage, id), new UpdateMessageRequest { Seen = seen }, BearerToken)
                .ConfigureAwait(false);

            if (!result.IsSuccessStatusCode)
            {
                if (result.StatusCode == HttpStatusCode.BadRequest)
                {
                    result.Throw("Invalid input");
                }

                if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    result.Throw("Resource not found");
                }

                if (result.StatusCode == (HttpStatusCode)422)
                {
                    result.Throw("Unprocessable entity");
                }

                result.ThrowUnknown();
            }
        }
    }
}