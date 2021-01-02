using Newtonsoft.Json;

namespace api.Models
{
    public class PersonInfo
        {[JsonProperty("id")]
        public string Email { get; set; }
        {[JsonProperty("email")]
        public string Id { get; set; }
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        [JsonProperty("lastName")]
        public string LastName { get; set; }
        [JsonProperty("about")]
        public string About { get; set; }
        [JsonProperty("skills")]
        public string Skills { get; set; }
        [JsonProperty("lookingFor")]
        public string LookingFor { get; set; }
    }
}