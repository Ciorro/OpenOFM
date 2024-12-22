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
            var response = await _api.Get("/radio/stations", ct);

            var stations = await GetStationsInternal(ct);
            var categories = await GetCategoriesInternal(ct);

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

        public async Task<IEnumerable<int>> GetFeaturedRadioStationsIds(CancellationToken ct = default)
        {
            var response = await _api.Get("/radio/featured", ct);

            return JsonSerializer.Deserialize<int[]>(
                await response.Content.ReadAsStreamAsync(ct), _jsonOptions) ?? [];
        }

        public async Task<IEnumerable<RadioCategory>> GetRadioStationCategories(CancellationToken ct = default)
        {
            return (await GetCategoriesInternal(ct)).Select(x => x.ToModel()).ToList(); 
        }

        private async Task<IEnumerable<RadioStationDTO>> GetStationsInternal(CancellationToken ct)
        {
            var response = await _api.Get("/radio/stations", ct);

            var stations = await JsonSerializer.DeserializeAsync<Dictionary<int, RadioStationDTO>>(
                await response.Content.ReadAsStreamAsync(ct), _jsonOptions, ct);

            return stations?.Values ?? Enumerable.Empty<RadioStationDTO>();
        }

        private async Task<IEnumerable<RadioCategoryDTO>> GetCategoriesInternal(CancellationToken ct)
        {
            var response = await _api.Get("/radio/categories", ct);

            var categories = JsonSerializer.Deserialize<List<RadioCategoryDTO>>(
                await response.Content.ReadAsStreamAsync(ct), _jsonOptions) ?? [];

            return await Task.WhenAll(categories.Select(async x =>
            {
                var response = await _api.Get($"/radio/category/{x.Slug}", ct);

                var category = JsonSerializer.Deserialize<RadioCategoryDTO>(
                    await response.Content.ReadAsStreamAsync(ct), _jsonOptions);

                return category ?? new();
            }));
        }
    }
}
