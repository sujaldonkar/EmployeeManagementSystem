using EmployeeManagementSystem.Models.ApiModels;
using EmployeeManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagementSystem.Repositories
{
    public interface ILocationRepository
    {
        Task SyncCountriesAsync(IEnumerable<CountryApiModel> countries);

        Task SyncStatesAsync(int countryId, IEnumerable<StateApiModel> states);

        Task SyncCitiesAsync(int stateId, IEnumerable<CityApiModel> cities);

        Task<int?> GetCountryIdByCodeAsync(string countryCode);

        Task<int?> GetStateIdByCodeAsync(int countryId, string stateCode);

        Task<IEnumerable<LocationDropdownItem>> GetCountriesAsync();

        Task<IEnumerable<LocationDropdownItem>> GetStatesAsync(int countryId);

        Task<IEnumerable<SelectListItem>> GetCitiesAsync(int stateId);

        Task<string?> GetCountryCodeByIdAsync(int countryId);

        Task<string?> GetStateCodeByIdAsync(int stateId);
    }
}