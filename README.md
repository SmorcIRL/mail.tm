# mail.tm
A [mail.tm](https://mail.tm) / [mail.gw](https://mail.gw) API wrapper for .NET. These services have identical api at the moment, but this might change in future.

## Installation

Available as a [NuGet](https://www.nuget.org/packages/SmorcIRL.TempMail).

`PM> Install-Package SmorcIRL.TempMail`

## Getting started

### Introduction
`mail.tm` is a great temporary/disposable mail service, and also recently was launched a new 10 minute mail service called `mail.gw`. As I said, these two services have identical api, so all told about `mail.tm` will be true for `mail.gw` as well. Basically API calls require authorization, but some are not (getting domains info, for example). Having an authorized `MailClient` instance, you can get its authorization token via `BearerToken` property, that may be helpfull for manual debbuging using [Swagger](https://api.mail.tm/docs).

### Register

Interaction with account API is performed through a `MailClient` instance. You can simply register like this
```C#
MailClient client = new MailClient();
await client.Register("pepethefrog@sad.me", "*password*");
```
You can also specify the base api url. The default refer to [mail.tm](https://api.mail.tm), but if you want to use [mail.gw](https://api.mail.gw/), create a client like that
```C#
MailClient client = new MailClient(new Uri("https://api.mail.gw/"));
```

Make sure you specify the domain correctly - domains list is limited by the service. You can get an actual list using
```C#
DomainInfo[] domains = await client.GetAvailableDomains();
```

If you only need to get any valid domain name for you account
```C#
string domain = await client.GetFirstAvailableDomainName();
```

Now you can register using an overload that takes email and domain separately
```C#
string domain = await client.GetFirstAvailableDomainName();
await client.Register("pepethefrog", domain, "*password*");
```

### Log in

If you already have an account, you can log in using your creds
```C#
await client.Login("pepethefrog@sad.me", "*password*");
```

### Get account info

If you need to get info about the account you are using, like creation date or message quota, call
```C#
AccountInfo info = await client.GetAccountInfo();
```

### Delete account

After your work with the account is done, you can delete it
```C#
await client.DeleteAccount();
```
After that you can reuse the client with new authorization via `Login()`/`Register()` if needed.

### Working with messages

- Get info about all messages
```C#
MessageInfo[] messages = await client.GetAllMessages();
```

- Get info about all messages from the page
```C#
MessageInfo[] messages = await client.GetMessages(*page*);
```

- Get message info by id
```C#
MessageInfo message = await client.GetMessage("*id*");
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
