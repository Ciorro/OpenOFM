namespace OpenOFM.Core.Models
{
    public class RadioStation
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public string? StreamUrl { get; set; }
        public string? CoverUrl { get; set; }
        public bool IsPremium { get; set; }
        public List<RadioCategory> Categories { get; set; } = [];
    }
}
