using System.Text.Json.Serialization;

namespace Domain.EventClient
{
    internal class EventContract
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("start")]
        public DateTimeOffset Start { get; set; }

        [JsonPropertyName("end")]
        public DateTimeOffset End { get; set; }

        [JsonPropertyName("location")]
        public string? Location { get; set; }

        [JsonPropertyName("filterList")]
        public string? FilterList { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("imageText")]
        public string? ImageText { get; set; }

        [JsonPropertyName("language")]
        public string? Language { get; set; }
    }
}
