using System;
using Newtonsoft.Json;

namespace SmorcIRL.TempMail.Models
{
    public class MessageDetailInfo : MessageInfo
    {
        [JsonProperty("cc")]
        public UserInfo[] CC { get; set; }
        
        [JsonProperty("bcc")]
        public UserInfo[] BCC { get; set; }
        
        [JsonProperty("flagged")]
        public bool Flagged { get; set; }
        
        [JsonProperty("verifications")]
        public VerificationsInfo Verifications { get; set; }
        
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