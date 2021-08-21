using System;
using Newtonsoft.Json;

namespace SmorcIRL.TempMail.Models
{
    public class MessageInfo
    {
        public string Id { get; set; }

        public string AccountId { get; set; }

        [JsonProperty("msgid")]
        public string MessageId { get; set; }

        public UserInfo From { get; set; }

        public UserInfo[] To { get; set; }

        public string Subject { get; set; }

        public string Intro { get; set; }

        public bool Seen { get; set; }

        public bool IsDeleted { get; set; }

        public bool HasAttachments { get; set; }

        public int Size { get; set; }

        public string DownloadUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}