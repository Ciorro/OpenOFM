namespace OpenOFM.Core.Models
{
    public class RadioCategory : IEquatable<RadioCategory>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }

        public bool Equals(RadioCategory? other)
        {
            return other?.Id == Id;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as RadioCategory);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator== (RadioCategory? r1, RadioCategory? r2)
        {
            return r1?.Equals(r2) == true;
        }

        public static bool operator !=(RadioCategory? r1, RadioCategory? r2)
        {
            return !(r1 == r2);
        }
    }
}
