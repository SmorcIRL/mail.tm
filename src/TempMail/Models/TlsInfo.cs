using Newtonsoft.Json;

namespace SmorcIRL.TempMail.Models
{
    public class TlsInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("standardName")]
        public string StandardName { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }
}