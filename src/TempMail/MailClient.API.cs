using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmorcIRL.TempMail.Helpers;
using SmorcIRL.TempMail.Messaging;
using SmorcIRL.TempMail.Models;

namespace SmorcIRL.TempMail
{
    public sealed partial class MailClient
    {
        public async Task Login(string fullAddress, string password)
        {
            Ensure.IsPresent(fullAddress, nameof(fullAddress));
            Ensure.IsPresent(password, nameof(password));

            var result = await _httpClient.PostAsync<GetTokenRequest, TokenInfo>(FormatUri(Endpoints.PostToken), new GetTokenRequest
            {
                Address = fullAddress,
                Password = password,
            }).ConfigureAwait(false);

            result.Message.EnsureSuccessStatusCode();

            Email = fullAddress;
            AccountId = result.Data.Id;
            BearerToken = result.Data.Token;
        }

        public async Task Register(string fullAddress, string password)
        {
            Ensure.IsPresent(fullAddress, nameof(fullAddress));
            Ensure.IsPresent(password, nameof(password));

            var createAccountResult = await _httpClient.PostAsync<CreateAccountRequest, AccountInfo>(FormatUri(Endpoints.PostAccount), new CreateAccountRequest
            {
                Address = fullAddress,
                Password = password,
            }).ConfigureAwait(false);

            createAccountResult.Message.EnsureSuccessStatusCode();

            fullAddress = createAccountResult.Data.Address;
            var tokenResult = await _httpClient.PostAsync<GetTokenRequest, TokenInfo>(FormatUri(Endpoints.PostToken), new GetTokenRequest
            {
                Address = fullAddress,
                Password = password,
            }).ConfigureAwait(false);

            tokenResult.Message.EnsureSuccessStatusCode();

            Email = fullAddress;
            AccountId = tokenResult.Data.Id;
            BearerToken = tokenResult.Data.Token;
        }

        public async Task Register(string address, string domain, string password)
        {
            Ensure.IsPresent(address, nameof(address));
            Ensure.IsPresent(domain, nameof(domain));
            Ensure.IsPresent(password, nameof(password));

            await Register($"{address}@{domain}", password).ConfigureAwait(false);
        }

        //========================================//

        public async Task<AccountInfo> GetAccountInfo()
        {
            var result = await _httpClient
                .GetAsync<AccountInfo>(FormatUri(Endpoints.GetMe), BearerToken)
                .ConfigureAwait(false);

            result.Message.EnsureSuccessStatusCode();

            return result.Data;
        }

        public async Task DeleteAccount()
        {
            if (BearerToken == default)
            {
                return;
            }

            var result = await _httpClient
                .DeleteAsync(FormatUri(Endpoints.DeleteAccount, AccountId), BearerToken)
                .ConfigureAwait(false);

            result.Message.EnsureSuccessStatusCode();

            Email = default;
            AccountId = default;
            BearerToken = default;
        }

        //========================================//

        public async Task<DomainInfo[]> GetAvailableDomains()
        {
            var result = await _httpClient
                .GetAsync<DomainInfo[]>(FormatUri(Endpoints.GetDomains))
                .ConfigureAwait(false);

            result.Message.EnsureSuccessStatusCode();

            return result.Data;
        }

        public async Task<DomainInfo> GetDomain(string id)
        {
            Ensure.IsPresent(id, nameof(id));

            var result = await _httpClient
                .GetAsync<DomainInfo>(FormatUri(Endpoints.GetDomain, id))
                .ConfigureAwait(false);

            result.Message.EnsureSuccessStatusCode();

            return result.Data;
        }

        public async Task<string> GetFirstAvailableDomainName()
        {
            var domains = await GetAvailableDomains().ConfigureAwait(false);

            return domains.FirstOrDefault()?.Domain;
        }

        //========================================//

        public async Task<MessageInfo[]> GetMessages(int page)
        {
            Ensure.IsPositive(page, nameof(page));

            var result = await _httpClient
                .GetAsync<MessageInfo[]>(FormatUri(Endpoints.GetMessages, page), BearerToken)
                .ConfigureAwait(false);

            result.Message.EnsureSuccessStatusCode();

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

            var result = await _httpClient
                .GetAsync<MessageInfo>(FormatUri(Endpoints.GetMessage, id), BearerToken)
                .ConfigureAwait(false);

            result.Message.EnsureSuccessStatusCode();

            return result.Data;
        }

        public async Task DeleteMessage(string id)
        {
            Ensure.IsPresent(id, nameof(id));

            var result = await _httpClient
                .DeleteAsync(FormatUri(Endpoints.DeleteMessage, id), BearerToken)
                .ConfigureAwait(false);

            result.Message.EnsureSuccessStatusCode();
        }

        public async Task<MessageSource> GetMessageSource(string id)
        {
            Ensure.IsPresent(id, nameof(id));

            var result = await _httpClient
                .GetAsync<MessageSource>(FormatUri(Endpoints.GetSource, id), BearerToken)
                .ConfigureAwait(false);

            result.Message.EnsureSuccessStatusCode();

            return result.Data;
        }

        public async Task MarkMessageAsSeen(string id, bool seen)
        {
            Ensure.IsPresent(id, nameof(id));

            var result = await _httpClient
                .PatchAsync(FormatUri(Endpoints.PatchMessage, id), new UpdateMessageRequest { Seen = seen }, BearerToken)
                .ConfigureAwait(false);

            result.Message.EnsureSuccessStatusCode();
        }
    }
}