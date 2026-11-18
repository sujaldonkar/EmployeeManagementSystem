
using Dapper;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagementSystem.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public EmployeeRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // Get all employees
        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryAsync<Employee>(
                "sp_Employee_GetAll",
                commandType: CommandType.StoredProcedure
            );
        }

        // Get employee by ID
        public async Task<Employee?> GetByIdAsync(int employeeId)
        {
            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Employee>(
                "sp_Employee_GetById",
                new { EmployeeId = employeeId },
                commandType: CommandType.StoredProcedure
            );
        }

        // Insert employee
        public async Task<int> InsertAsync(Employee employee)
        {
            using var connection = _connectionFactory.CreateConnection();

            try
            {
                return await connection.QuerySingleAsync<int>(
                    "sp_Employee_Insert",
                    new
                    {
                        employee.FirstName,
                        employee.LastName,
                        employee.DateOfBirth,
                        employee.CountryId,
                        employee.StateId,
                        employee.CityId,
                        employee.QualificationId,
                        employee.EmailAddress,
                        employee.PhoneNo
                    },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (SqlException ex) when (
                ex.Number == 50007 ||
                ex.Number == 50008 ||
                ex.Number == 2627 ||
                ex.Number == 2601)
            {
                throw new InvalidOperationException(
                    ex.Number == 50007
                        ? "Email address already exists."
                        : ex.Number == 50008
                            ? "Phone number already exists."
                            : "An employee with this email or phone number already exists.",
                    ex
                );
            }
        }

        // Update employee
        public async Task<bool> UpdateAsync(Employee employee)
        {
            using var connection = _connectionFactory.CreateConnection();

            try
            {
                int employeeId = await connection.QuerySingleAsync<int>(
                    "sp_Employee_Update",
                    new
                    {
                        employee.EmployeeId,
                        employee.FirstName,
                        employee.LastName,
                        employee.DateOfBirth,
                        employee.CountryId,
                        employee.StateId,
                        employee.CityId,
                        employee.QualificationId,
                        employee.EmailAddress,
                        employee.PhoneNo
                    },
                    commandType: CommandType.StoredProcedure
                );

                return employeeId > 0;
            }
            catch (SqlException ex) when (
                ex.Number == 50017 ||
                ex.Number == 50018 ||
                ex.Number == 2627 ||
                ex.Number == 2601)
            {
                throw new InvalidOperationException(
                    ex.Number == 50017
                        ? "Email address already exists."
                        : ex.Number == 50018
                            ? "Phone number already exists."
                            : "An employee with this email or phone number already exists.",
                    ex
                );
            }
        }

        // Delete employee
        public async Task<bool> DeleteAsync(int employeeId)
        {
            using var connection = _connectionFactory.CreateConnection();

            int deletedId = await connection.QuerySingleAsync<int>(
                "sp_Employee_Delete",
                new { EmployeeId = employeeId },
                commandType: CommandType.StoredProcedure
            );

            return deletedId > 0;
        }

        // Get countries
        public async Task<IEnumerable<SelectListItem>> GetCountriesAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryAsync<SelectListItem>(
                "sp_GetCountries",
                commandType: CommandType.StoredProcedure
            );
        }

        // Get states by country
        public async Task<IEnumerable<SelectListItem>> GetStatesByCountryAsync(int countryId)
        {
            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryAsync<SelectListItem>(
                "sp_GetStatesByCountry",
                new { CountryId = countryId },
                commandType: CommandType.StoredProcedure
            );
        }

        // Get cities by state
        public async Task<IEnumerable<SelectListItem>> GetCitiesByStateAsync(int stateId)
        {
            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryAsync<SelectListItem>(
                "sp_GetCitiesByState",
                new { StateId = stateId },
                commandType: CommandType.StoredProcedure
            );
        }

        // Get qualifications
        public async Task<IEnumerable<SelectListItem>> GetQualificationsAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryAsync<SelectListItem>(
                "sp_GetQualifications",
                commandType: CommandType.StoredProcedure
            );
        }

        // Search employees
        public async Task<IEnumerable<Employee>> SearchAsync(string searchTerm)
        {
            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryAsync<Employee>(
                "sp_Employee_Search",
                new { SearchTerm = searchTerm },
                commandType: CommandType.StoredProcedure
            );
        }

        // Get country ID by state ID
        public async Task<int?> GetCountryIdByStateAsync(int stateId)
        {
            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<int?>(
                @"SELECT CountryId
                  FROM dbo.States
                  WHERE StateId = @StateId",
                new { StateId = stateId }
            );
        }
    }
}
