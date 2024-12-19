using OpenOFM.Core.Api.DTO;
using OpenOFM.Core.Extensions;
using OpenOFM.Core.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenOFM.Core.Api
{
    public class StationsApiClient
    {
        private readonly ApiClient _api;
        private readonly JsonSerializerOptions _jsonOptions;

        public StationsApiClient(ApiClient api)
        {
            _api = api;
            _jsonOptions = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<IEnumerable<RadioStation>> GetRadioStations(CancellationToken ct = default)
        {
            var response = await _api.Get("/radio/stations");

            var stations = await GetStations(ct);
            var categories = await GetCategories(ct);

            return stations.Select(x =>
            {
                var station = x.ToModel();

                foreach (var category in categories)
                {
                    if (category.Stations.Contains(station.Id))
                    {
                        station.Categories.Add(category.ToModel());
                    }
                }

                return station;
            }).ToList();
        }

        private async Task<IEnumerable<RadioStationDTO>> GetStations(CancellationToken ct)
        {
            var response = await _api.Get("/radio/stations");

            var stations = await JsonSerializer.DeserializeAsync<Dictionary<int, RadioStationDTO>>(
                await response.Content.ReadAsStreamAsync(), _jsonOptions);

            return stations?.Values ?? Enumerable.Empty<RadioStationDTO>();
        }

        private async Task<IEnumerable<RadioCategoryDTO>> GetCategories(CancellationToken ct)
        {
            var response = await _api.Get("/radio/categories");

            var categories = await JsonSerializer.DeserializeAsync<List<RadioCategoryDTO>>(
                await response.Content.ReadAsStreamAsync(), _jsonOptions) ?? [];

            return await Task.WhenAll(categories.Select(async x =>
            {
                var response = await _api.Get($"/radio/category/{x.Slug}");

                var category = await JsonSerializer.DeserializeAsync<RadioCategoryDTO>(
                    await response.Content.ReadAsStreamAsync(), _jsonOptions);

                return category ?? new();
            }));
        }
    }
}
