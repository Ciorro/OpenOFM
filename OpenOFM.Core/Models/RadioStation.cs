namespace OpenOFM.Core.Models
{
    public class RadioStation : IEquatable<RadioStation>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public string? StreamUrl { get; set; }
        public string? CoverUrl { get; set; }
        public bool IsPremium { get; set; }
        public List<RadioCategory> Categories { get; set; } = [];

        public bool Equals(RadioStation? other)
        {
            return other?.Id == Id;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as RadioStation);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(RadioStation? r1, RadioStation? r2)
        {
            return r1?.Equals(r2) == true;
        }

        public static bool operator !=(RadioStation? r1, RadioStation? r2)
        {
            return !(r1 == r2);
        }
    }
}
