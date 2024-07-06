using Newtonsoft.Json;

namespace SmorcIRL.TempMail.Models
{
    public class UserInfo
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}