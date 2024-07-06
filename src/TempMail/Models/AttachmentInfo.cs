using System;
using Newtonsoft.Json;

namespace SmorcIRL.TempMail.Models
{
    public class AttachmentInfo
    {
        [JsonProperty("id")] 
        public string Id { get; set; }
        
        [JsonProperty("filename")] 
        public string Filename { get; set; }
        
        [JsonProperty("contentType")] 
        public string ContentType { get; set; }
        
        [JsonProperty("disposition")] 
        public string Disposition { get; set; }
        
        [JsonProperty("transferEncoding")] 
        public string TransferEncoding { get; set; }
        
        [JsonProperty("related")] 
        public bool Related { get; set; }
        
        [JsonProperty("size")] 
        public int Size { get; set; }
        
        [JsonProperty("downloadUrl")] 
        public string DownloadUrl { get; set; }
        

        
    }
}