using System.Net.Http.Headers;
using System.Text.Json;
using EmployeeManagementSystem.Models.ApiModels;

namespace EmployeeManagementSystem.Services
{
    public class CountryStateCityService
    {
        private readonly HttpClient _httpClient;

        private const string BaseUrl =
            "https://api.countrystatecity.in/v1/";

        public CountryStateCityService(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;

            string apiKey = configuration[
                "CountryStateCity:ApiKey"]
                ?? throw new InvalidOperationException(
                    "Country State City API key is missing.");

            _httpClient.BaseAddress = new Uri(BaseUrl);

            _httpClient.DefaultRequestHeaders.Add(
                "X-CSCAPI-KEY", apiKey);
        }

        private static readonly JsonSerializerOptions JsonOptions =
            new()
            {
                PropertyNameCaseInsensitive = true
            };

        public async Task<List<CountryApiModel>> GetCountriesAsync()
        {
            var response = await _httpClient.GetAsync("countries");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<CountryApiModel>>(
                json, JsonOptions) ?? new();
        }

        public async Task<List<StateApiModel>> GetStatesAsync(
            string countryCode)
        {
            var url = $"countries/{Uri.EscapeDataString(countryCode)}/states";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<StateApiModel>>(
                json, JsonOptions) ?? new();
        }

        public async Task<List<CityApiModel>> GetCitiesAsync(
            string countryCode,
            string stateCode)
        {
            var url =
                $"countries/{Uri.EscapeDataString(countryCode)}/states/{Uri.EscapeDataString(stateCode)}/cities";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<CityApiModel>>(
                json, JsonOptions) ?? new();
        }
    }
}