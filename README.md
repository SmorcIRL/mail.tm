# mail.tm
A [mail.tm](https://mail.tm) / [mail.gw](https://mail.gw) API wrapper for .NET. These services currently have identical APIs, but this may change in the future.

## Installation

Available as a [NuGet](https://www.nuget.org/packages/SmorcIRL.TempMail).

`PM> Install-Package SmorcIRL.TempMail`

## Getting started

### Introduction
`mail.tm` is a great temporary/disposable mail service, and recently a new 10 minute mail service called `mail.gw` was also launched. As I said, these two services have identical API's, so everything said about `mail.tm` will also apply to `mail.gw` (at least it was the last time I compared Swagger configs). Basically all API calls require authorisation, but some do not (e.g. getting domain info). If you have an authorised `MailClient` instance, you can get its authorisation token via the `BearerToken` property, which can be useful for manual debugging using [Swagger](https://api.mail.tm/docs).

### Register

Interaction with account API is performed through a `MailClient` instance. You can simply register like this
```C#
MailClient client = new MailClient();
await client.Register("pepethefrog@sad.me", "*password*");
```
You can also specify the base api url. The default refers to [mail.tm](https://api.mail.tm), but if you want to use [mail.gw](https://api.mail.gw/), create a client like this
```C#
MailClient client = new MailClient(new Uri("https://api.mail.gw/"));
```

Make sure you specify the domain correctly - the list of domains is limited by the service. You can get an up-to-date list using
```C#
DomainInfo[] domains = await client.GetAvailableDomains();
```

If you just need to get any valid domain name for your account
```C#
string domain = await client.GetFirstAvailableDomainName();
```

There is also an option to generate a new account. Note that it won't be possible to recover the password if you omit it. 
```C#
await client.GenerateAccount("*optional_password*");
```

### Log in

If you already have an account, you can log in using your creds
```C#
await client.Login("pepethefrog@sad.me", "*password*");
```

If you have an auth token, you can use it instead
```C#
await client.LoginWithToken("*token*");
```

### Get account info

If you need to get info about the account you are using, such as creation date or message quota, call
```C#
AccountInfo info = await client.GetAccountInfo();
```

### Delete account

Once you are finished with the account, you can delete it
```C#
await client.DeleteAccount();
```

After that you can reuse the client instance with a new authorisation via `Login()`/`Register()` if needed.

### Working with messages

- Get info about all messages
```C#
MessageInfo[] messages = await client.GetAllMessages();
```

- Get info about all messages from the page
```C#
MessageInfo[] messages = await client.GetMessages(*page*);
```

- Get message details by id
```C#
MessageDetailInfo message = await client.GetMessage("*id*");
```

- Get message source
```C#
MessageSource source = await client.GetMessageSource("*id*");
string rawMessage = source.Data;
```

- Mark as seen
```C#
await client.MarkMessageAsSeen("*id*", true);
```

- Delete
```C#
await client.DeleteMessage("*id*");
```
