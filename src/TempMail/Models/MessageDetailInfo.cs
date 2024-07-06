using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SmorcIRL.TempMail.Models
{
    public class MessageDetailInfo : MessageInfo
    {
        [JsonProperty("cc")]
        public string[] CC { get; set; }
        
        [JsonProperty("bcc")]
        public string[] BCC { get; set; }
        
        [JsonProperty("flagged")]
        public bool Flagged { get; set; }
        
        // It should be string[] according to the docs, but for me, it was a single object
        // JToken should allow any kind of JSON
        [JsonProperty("verifications")]
        public JToken Verifications { get; set; }
        
        [JsonProperty("retention")]
        public bool Retention { get; set; }
        
        [JsonProperty("retentionDate")]
        public DateTime RetentionDate { get; set; }
        
        [JsonProperty("text")]
        public string BodyText { get; set; }
        
        [JsonProperty("html")]
        public string[] BodyHtml { get; set; }
        
        [JsonProperty("attachments")]
        public AttachmentInfo[] Attachments { get; set; }
    }
}