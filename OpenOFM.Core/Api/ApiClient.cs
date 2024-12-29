using System.Net;

namespace OpenOFM.Core.Api
{
    public class ApiClient
    {
        private HttpClient _http;

        public ApiClient()
        {
            _http = new HttpClient()
            {
                BaseAddress = new Uri("http://open.fm/api/")
            };
            _http.DefaultRequestHeaders.Add(
                "User-Agent", "BULDOZER449");
        }

        public async Task<HttpResponseMessage> Get(string path, CancellationToken ct = default)
        {
            var response = await _http.GetAsync(path.Trim('/'), ct);

            // OpenFM 449 error workaround.
            if (response.StatusCode == (HttpStatusCode)449)
            {
                // Change base address to proxy address.
                _http = new HttpClient()
                {
                    BaseAddress = new Uri("http://oofm.runasp.net/api/")
                };
                _http.DefaultRequestHeaders.Add(
                    "User-Agent", "BULDOZER449");

                return await Get(path, ct);
            }

            return response.EnsureSuccessStatusCode();
        }
    }
}
