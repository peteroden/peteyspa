using System.Text.Json.Serialization;

namespace api.Models
{
    public class PersonInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
        [JsonPropertyName("about")]
        public string About { get; set; }
        [JsonPropertyName("skills")]
        public string Skills { get; set; }
        [JsonPropertyName("lookingFor")]
        public string LookingFor { get; set; }
    }
}