# mail.tm
A [mail.tm](https://mail.tm) API wrapper for .Net

## Installation

Available as a [NuGet](https://www.nuget.org/packages/SmorcIRL.TempMail).

`PM> Install-Package SmorcIRL.TempMail`

## Getting started

### Introduction

All API interaction is asynchronous. Some API calls require to be authorized and some are not. The first group is represented by static methods of `MailClient` class, the second - with its instance methods. Having a `MailClient` instance, you can get its authorization token via `BearerToken` property, that may be helpfull for manual debbuging using [Swagger](https://api.mail.tm/docs). Also you can specify the base API address using `MailClient.ApiUri` property, by default it references to https://api.mail.tm.

### Register

Interaction with account API is performed through a `MailClient` instance. You can simply register like this
```C#
MailClient client = await MailClient.Register("pepethefrog@sad.me", "*password*");
```

Make sure you specify the domain correctly - domains list is limited by the service. You can get an actual list using
```C#
DomainInfo[] domains = await MailClient.GetAvailableDomains();
```

If you only need to get any valid domain name for you account
```C#
string domain = await MailClient.GetFirstAvailableDomainName();
```

Now you can register using an overload that takes email and domain separately
```C#
string domain = await MailClient.GetFirstAvailableDomainName();
MailClient client = await MailClient.Register("pepethefrog", domain, "*password*");
```

### Log in

If you already have an account, you can log in using your creds
```C#
MailClient client = await MailClient.Login("pepethefrog@sad.me", "*password*");
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
After that all API calls from this client instance will throw an error.

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
