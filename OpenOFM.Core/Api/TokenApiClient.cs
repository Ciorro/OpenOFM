using OpenOFM.Core.Api.DTO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenOFM.Core.Api
{
    public class TokenApiClient
    {
        private readonly ApiClient _api;
        private readonly JsonSerializerOptions _jsonOptions;

        public TokenApiClient(ApiClient api)
        {
            _api = api;
            _jsonOptions = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<string> AppendToken(string url, CancellationToken ct = default)
        {
            var response = await _api.Get($"/user/token?fp={url}", ct);
            return JsonSerializer.Deserialize<TokenUrlDTO>(
                await response.Content.ReadAsStreamAsync(), _jsonOptions)?.Url ?? "";
        }
    }
}
