using OpenOFM.Core.Api.DTO;
using OpenOFM.Core.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenOFM.Core.Api
{
    public class PlaylistApiClient
    {
        private readonly ApiClient _api;
        private readonly JsonSerializerOptions _jsonOptions;

        public PlaylistApiClient(ApiClient api)
        {
            _api = api;
            _jsonOptions = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<IEnumerable<Playlist>> GetPlaylists(CancellationToken ct = default)
        {
            var response = await _api.Get("/radio/playlist", ct);

            var playlists =  await JsonSerializer.DeserializeAsync<Dictionary<int, PlaylistDTO>>(
                await response.Content.ReadAsStreamAsync(ct), _jsonOptions, ct) ?? [];

            return playlists.Select(x =>
            {
                var queue = x.Value.CurrentSong is not null ?
                    x.Value.Playlist.Prepend(x.Value.CurrentSong) :
                    x.Value.Playlist;

                return new Playlist(x.Key, queue);
            }).ToList();
        }
    }
}
