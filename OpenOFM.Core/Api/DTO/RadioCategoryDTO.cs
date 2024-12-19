using System.Text.Json.Serialization;

namespace OpenOFM.Core.Api.DTO
{
    internal class RadioCategoryDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }

        [JsonPropertyName("items")]
        public int[] Stations { get; set; } = [];
    }
}
