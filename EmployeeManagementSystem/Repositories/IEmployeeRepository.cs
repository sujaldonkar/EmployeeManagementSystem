
using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagementSystem.Repositories
{
    public interface IEmployeeRepository
    {
        // Employee CRUD
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(int employeeId);
        Task<int> InsertAsync(Employee employee);
        Task<bool> UpdateAsync(Employee employee);
        Task<bool> DeleteAsync(int employeeId);

        // Search
        Task<IEnumerable<Employee>> SearchAsync(string searchTerm);

        // Dropdown data
        Task<IEnumerable<SelectListItem>> GetCountriesAsync();
        Task<IEnumerable<SelectListItem>> GetStatesByCountryAsync(int countryId);
        Task<IEnumerable<SelectListItem>> GetCitiesByStateAsync(int stateId);
        Task<IEnumerable<SelectListItem>> GetQualificationsAsync();

        Task<int?> GetCountryIdByStateAsync(int stateId);
    }
}
