namespace OpenOFM.Core.Models
{
    public class Song : IEquatable<Song>
    {
        public string Title { get; set; }
        public string Artist { get; set; }

        public Song(string title, string artist)
        {
            Title = title;
            Artist = artist;
        }

        public bool Equals(Song? other)
        {
            return other?.Title == Title && other?.Artist == Artist;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Song);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Title, Artist);
        }

        public static bool operator ==(Song? s1, Song? s2)
        {
            return s1?.Equals(s2) == true;
        }

        public static bool operator !=(Song? s1, Song? s2)
        {
            return !(s1 == s2);
        }
    }
}
