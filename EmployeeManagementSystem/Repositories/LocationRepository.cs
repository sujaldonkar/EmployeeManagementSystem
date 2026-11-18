using Dapper;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models.ApiModels;
using EmployeeManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace EmployeeManagementSystem.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public LocationRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task SyncCountriesAsync(
            IEnumerable<CountryApiModel> countries)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var country in countries)
                {
                    if (string.IsNullOrWhiteSpace(country.iso2) ||
                        string.IsNullOrWhiteSpace(country.name))
                        continue;

                    await connection.ExecuteAsync(@"
                        UPDATE dbo.Countries
                        SET CountryCode = @CountryCode
                        WHERE CountryName = @CountryName;

                        IF @@ROWCOUNT = 0
                        BEGIN
                            INSERT INTO dbo.Countries
                                (CountryName, CountryCode)
                            VALUES
                                (@CountryName, @CountryCode);
                        END",
                        new
                        {
                            CountryCode = country.iso2,
                            CountryName = country.name
                        },
                        transaction);
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task SyncStatesAsync(
            int countryId,
            IEnumerable<StateApiModel> states)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var state in states)
                {
                    if (string.IsNullOrWhiteSpace(state.iso2) ||
                        string.IsNullOrWhiteSpace(state.name))
                        continue;

                    await connection.ExecuteAsync(@"
                        UPDATE dbo.States
                        SET StateCode = @StateCode
                        WHERE CountryId = @CountryId
                          AND StateName = @StateName;

                        IF @@ROWCOUNT = 0
                        BEGIN
                            INSERT INTO dbo.States
                                (StateName, CountryId, StateCode)
                            VALUES
                                (@StateName, @CountryId, @StateCode);
                        END",
                        new
                        {
                            CountryId = countryId,
                            StateCode = state.iso2,
                            StateName = state.name
                        },
                        transaction);
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task SyncCitiesAsync(
            int stateId,
            IEnumerable<CityApiModel> cities)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var city in cities)
                {
                    if (string.IsNullOrWhiteSpace(city.name))
                        continue;

                    await connection.ExecuteAsync(@"
                        IF NOT EXISTS (
                            SELECT 1 FROM dbo.Cities
                            WHERE StateId = @StateId
                              AND CityName = @CityName
                        )
                        BEGIN
                            INSERT INTO dbo.Cities
                                (CityName, StateId)
                            VALUES
                                (@CityName, @StateId);
                        END",
                        new
                        {
                            StateId = stateId,
                            CityName = city.name
                        },
                        transaction);
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<int?> GetCountryIdByCodeAsync(string countryCode)
        {
            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<int?>(
                @"SELECT CountryId
                  FROM dbo.Countries
                  WHERE CountryCode = @CountryCode",
                new { CountryCode = countryCode });
        }

        public async Task<int?> GetStateIdByCodeAsync(
            int countryId, string stateCode)
        {
            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<int?>(
                @"SELECT StateId
                  FROM dbo.States
                  WHERE CountryId = @CountryId
                    AND StateCode = @StateCode",
                new { CountryId = countryId, StateCode = stateCode });
        }

        public async Task<string?> GetCountryCodeByIdAsync(int countryId)
        {
            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<string>(
                @"SELECT CountryCode
          FROM dbo.Countries
          WHERE CountryId = @CountryId",
                new { CountryId = countryId });
        }

        public async Task<string?> GetStateCodeByIdAsync(int stateId)
        {
            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<string>(
                @"SELECT StateCode
          FROM dbo.States
          WHERE StateId = @StateId",
                new { StateId = stateId });
        }

        public async Task<IEnumerable<LocationDropdownItem>> GetCountriesAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryAsync<LocationDropdownItem>(
                @"SELECT
            CountryId AS Id,
            CountryName AS Name,
            CountryCode AS Code
          FROM dbo.Countries
          ORDER BY CountryName");
        }

        public async Task<IEnumerable<LocationDropdownItem>> GetStatesAsync(int countryId)
        {
            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryAsync<LocationDropdownItem>(
                @"SELECT
            StateId AS Id,
            StateName AS Name,
            StateCode AS Code
          FROM dbo.States
          WHERE CountryId = @CountryId
          ORDER BY StateName",
                new { CountryId = countryId });
        }

        public async Task<IEnumerable<SelectListItem>> GetCitiesAsync(int stateId)
        {
            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryAsync<SelectListItem>(
                @"SELECT CityId AS [Value],
                         CityName AS [Text]
                  FROM dbo.Cities
                  WHERE StateId = @StateId
                  ORDER BY CityName",
                new { StateId = stateId });
        }
    }
}