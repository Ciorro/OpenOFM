using System.Text.Json.Serialization;

namespace OpenOFM.Core.Api.DTO
{
    public class RadioStationDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public string? StreamUrl { get; set; }

        [JsonPropertyName("logoUrl")]
        public string? CoverUrl { get; set; }

        [JsonPropertyName("premium")]
        public bool IsPremium { get; set; }
    }
}
