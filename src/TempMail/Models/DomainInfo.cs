using System;
using Newtonsoft.Json;

namespace SmorcIRL.TempMail.Models
{
    public class DomainInfo
    {
        [JsonProperty("id")] 
        public string Id { get; set; }

        [JsonProperty("domain")] 
        public string Domain { get; set; }

        [JsonProperty("isActive")] 
        public string IsActive { get; set; }

        [JsonProperty("isPrivate")] 
        public string IsPrivate { get; set; }

        [JsonProperty("createdAt")] 
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")] 
        public DateTime UpdatedAt { get; set; }
    }
}