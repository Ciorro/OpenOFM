using System.Text.Json;

namespace OpenOFM.Core.Settings
{
    public class JsonSettingsProvider<T> : ISettingsProvider<T>
        where T : new()
    {
        private readonly string _path;
        public T CurrentSettings { get; private set; } = new();

        public JsonSettingsProvider(string path)
        {
            _path = path;
        }

        public void Load()
        {
            EnsureDirectoryExists();

            if (Path.Exists(_path))
            {
                using (var jsonStream = File.OpenRead(_path))
                {
                    CurrentSettings = JsonSerializer.Deserialize<T>(jsonStream) ?? new();
                }
            }
        }

        public void Save()
        {
            EnsureDirectoryExists();

            using (var jsonStream = File.OpenWrite(_path))
            {
                jsonStream.SetLength(0);
                JsonSerializer.Serialize(jsonStream, CurrentSettings);
            }
        }

        private void EnsureDirectoryExists()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        }
    }
}
