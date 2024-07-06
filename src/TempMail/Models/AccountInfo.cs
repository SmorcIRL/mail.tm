using System;
using Newtonsoft.Json;

namespace SmorcIRL.TempMail.Models
{
    public class AccountInfo
    {
        [JsonProperty("id")] 
        public string Id { get; set; }

        [JsonProperty("address")] 
        public string Address { get; set; }

        [JsonProperty("quota")] 
        public int Quota { get; set; }

        [JsonProperty("used")] 
        public int Used { get; set; }

        [JsonProperty("isDisabled")] 
        public bool IsDisabled { get; set; }

        [JsonProperty("isDeleted")] 
        public bool IsDeleted { get; set; }

        [JsonProperty("createdAt")] 
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")] 
        public DateTime UpdatedAt { get; set; }
    }
}