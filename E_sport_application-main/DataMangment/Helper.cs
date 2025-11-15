using System;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DataMangment
{
    public static class Helper
    {
        private static string? _resolvedConnectionString;

        /// <summary>
        /// Reads the App.config file and returns the details of the connection string matching the
        /// provided name.
        /// </summary>
        /// <param name="name">The name of the desired connection string</param>
        /// <returns>A string containing all the connection string details.</returns>
        private static string GetConnectionString(string name)
        {
            // Try the provided name first
            var cs = ConfigurationManager.ConnectionStrings[name];
            // Fallbacks between common names
            if (cs == null && string.Equals(name, "Default", StringComparison.OrdinalIgnoreCase))
                cs = ConfigurationManager.ConnectionStrings["DefaultConnection"];
            if (cs == null && string.Equals(name, "DefaultConnection", StringComparison.OrdinalIgnoreCase))
                cs = ConfigurationManager.ConnectionStrings["Default"];
            if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString))
                throw new InvalidOperationException($"Connection string '{name}' not found or empty in App.config.");
            return cs.ConnectionString;
        }

        /// <summary>
        /// Creates an SqlConnection object which is used for connecting to an SQL Server database.
        /// As part of this it retrieves the required connection string via the GetConnectionString
        /// method.
        /// </summary>
        /// <param name="name">The name of the desired connection string.</param>
        /// <returns>A configured Sql Server connection object.</returns>
        public static SqlConnection CreateSQLServerConnection(string name)
        {
            if (_resolvedConnectionString == null)
            {
                _resolvedConnectionString = GetConnectionString(name);
            }
            return new SqlConnection(_resolvedConnectionString!);
        }

        /// <summary>
        /// Ensures there is a working database connection. Tries the configured connection string first.
        /// If it fails, attempts to create and initialize a LocalDB database on Windows machines.
        /// </summary>
        /// <param name="connectionString">Resulting usable connection string.</param>
        /// <param name="created">True if a new LocalDB database was created.</param>
        /// <returns>True if a usable connection was established or created.</returns>
        public static bool EnsureLocalDatabase(out string connectionString, out bool created)
        {
            created = false;

            // Always work from the configured connection string in App.config.
            // We ask for \"Default\" but GetConnectionString also supports \"DefaultConnection\"
            // as a fallback, so both names continue to work.
            string cfg;
            try
            {
                cfg = GetConnectionString("Default");
            }
            catch
            {
                connectionString = string.Empty;
                return false;
            }

            var builder = new SqlConnectionStringBuilder(cfg);
            var dbName = builder.InitialCatalog;
            if (string.IsNullOrWhiteSpace(dbName))
            {
                // We require a Database/Initial Catalog so we know what to create.
                connectionString = string.Empty;
                return false;
            }

            // 1) Try to open the configured database and ensure schema.
            try
            {
                using (var conn = new SqlConnection(cfg))
                {
                    conn.Open();
                    InitializeSchema(conn);
                }
                _resolvedConnectionString = cfg;
                connectionString = cfg;
                return true;
            }
            catch
            {
                // Fall through and try to create the database.
            }

            // 2) Attempt to create the database on the same server (LocalDB or SQL Server),
            //    then create the schema.
            try
            {
                // Connect to the server's master database.
                var masterBuilder = new SqlConnectionStringBuilder(cfg)
                {
                    InitialCatalog = "master"
                };

                using (var masterConn = new SqlConnection(masterBuilder.ConnectionString))
                {
                    masterConn.Open();
                    using (var cmd = masterConn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;

                        // Safely embed the database name and create it if it does not exist.
                        var safeNameForDbId = dbName.Replace("'", "''");
                        var safeNameForBracket = dbName.Replace("]", "]]");
                        cmd.CommandText =
                            $"IF DB_ID(N'{safeNameForDbId}') IS NULL CREATE DATABASE [{safeNameForBracket}]";

                        cmd.ExecuteNonQuery();
                    }
                }

                // Now open the newly created (or existing) database and ensure tables.
                using (var conn = new SqlConnection(cfg))
                {
                    conn.Open();
                    InitializeSchema(conn);
                }

                _resolvedConnectionString = cfg;
                connectionString = cfg;
                created = true;
                return true;
            }
            catch
            {
                connectionString = string.Empty;
                return false;
            }
        }

        private static void InitializeSchema(SqlConnection conn)
        {
            // Create tables if they do not exist. Matches the app's model and queries.
            string sql = @"
IF OBJECT_ID('dbo.Team', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Team (
        Team_id INT IDENTITY(1,1) PRIMARY KEY,
        Team_Name NVARCHAR(100) NOT NULL,
        primary_contact NVARCHAR(100) NULL,
        ContactPhone NVARCHAR(50) NULL,
        ContactEmail NVARCHAR(100) NULL,
        CompetitionPoints INT NOT NULL DEFAULT(0)
    );
END

IF OBJECT_ID('dbo.Events', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Events (
        EventID INT IDENTITY(1,1) PRIMARY KEY,
        EventName NVARCHAR(100) NOT NULL,
        EventLocation NVARCHAR(100) NOT NULL,
        EventDate DATE NOT NULL
    );
END

IF OBJECT_ID('dbo.Games', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Games (
        GameID INT IDENTITY(1,1) PRIMARY KEY,
        GameName NVARCHAR(100) NOT NULL,
        GameType NVARCHAR(50) NOT NULL
    );
END

IF OBJECT_ID('dbo.TeamResults', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TeamResults (
        ResultID INT IDENTITY(1,1) PRIMARY KEY,
        EventID INT NOT NULL,
        GameID INT NOT NULL,
        TeamID INT NOT NULL,
        OpposingTeamID INT NOT NULL,
        Result NVARCHAR(10) NOT NULL
    );
END
";

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }
        public static void EnsureDatabaseAndSchema(string name)
        {
            // Only use configured connection string; no LocalDB fallback
            var cs = GetConnectionString(name);
            using (var conn = new SqlConnection(cs))
            {
                conn.Open();
                InitializeSchema(conn);
            }
            _resolvedConnectionString = cs;
        }
    }
}