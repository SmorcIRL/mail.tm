using System;
using Newtonsoft.Json;

namespace SmorcIRL.TempMail.Models
{
    public class MessageInfo
    {
        [JsonProperty("id")] 
        public string Id { get; set; }

        [JsonProperty("accountId")] 
        public string AccountId { get; set; }

        [JsonProperty("msgid")] 
        public string MessageId { get; set; }

        [JsonProperty("from")] 
        public UserInfo From { get; set; }

        [JsonProperty("to")] 
        public UserInfo[] To { get; set; }

        [JsonProperty("subject")] 
        public string Subject { get; set; }

        [JsonProperty("intro")] 
        public string Intro { get; set; }

        [JsonProperty("seen")] 
        public bool Seen { get; set; }

        [JsonProperty("isDeleted")] 
        public bool IsDeleted { get; set; }

        [JsonProperty("hasAttachments")] 
        public bool HasAttachments { get; set; }

        [JsonProperty("size")] 
        public int Size { get; set; }

        [JsonProperty("downloadUrl")] 
        public string DownloadUrl { get; set; }

        [JsonProperty("createdAt")] 
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")] 
        public DateTime UpdatedAt { get; set; }
    }
}