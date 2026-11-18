using Microsoft.Data.SqlClient;

namespace EmployeeManagementSystem.Data
{
    public class DbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SqlConnection CreateConnection()
        {
            string connectionString =
                _configuration.GetConnectionString("EmployeeConnection")!;

            return new SqlConnection(connectionString);
        }
    }
}