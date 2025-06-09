using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Studdit.Persistence.Helpers
{
    /// <summary>
    /// Database connection helper
    /// </summary>
    public static class DatabaseHelper
    {
        /// <summary>
        /// Build connection string for different environments
        /// </summary>
        public static string BuildConnectionString(string server, string database, bool integratedSecurity = true, string? userId = null, string? password = null)
        {
            var builder = new System.Data.SqlClient.SqlConnectionStringBuilder
            {
                DataSource = server,
                InitialCatalog = database,
                IntegratedSecurity = integratedSecurity,
                MultipleActiveResultSets = true,
                ConnectTimeout = 30,
                CommandTimeout = 30
            };

            if (!integratedSecurity && !string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(password))
            {
                builder.UserID = userId;
                builder.Password = password;
            }

            return builder.ConnectionString;
        }

        /// <summary>
        /// Test database connectivity
        /// </summary>
        public static async Task<bool> TestConnectionAsync(string connectionString)
        {
            try
            {
                using var connection = new System.Data.SqlClient.SqlConnection(connectionString);
                await connection.OpenAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
