using Digitizeit.Quartz.HostedService.Extensions;
using Digitizeit.Quartz.HostedService.Helpers;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.IO;

namespace Digitizeit.Quartz.HostedService.SqLite
{
    public class CreateSqLiteDatabase
    {
        private readonly ILogger<CreateSqLiteDatabase> _logger;

        public CreateSqLiteDatabase(ILogger<CreateSqLiteDatabase> logger)
        {
            _logger = logger ?? new NullLogger<CreateSqLiteDatabase>();
        }

        /// <summary>
        /// Initiation
        /// </summary>
        /// <param name="connectionString"></param>
        public void Init(string connectionString)
        {
            if (!DbExist(connectionString))
            {
                _logger.LogDebug($"Database {connectionString.GetDatabaseNameSqlite()} not found, trying to create database.");
                CreateDatabase(connectionString);
                return;
            }
            _logger.LogDebug($"Found and using database {connectionString.GetSqliteConnectionString()}");
        }

        /// <summary>
        /// Create database file for sqlite.
        /// </summary>
        /// <param name="connectionString"></param>
        private void CreateDatabase(string connectionString)
        {
            try
            {
                if (!Directory.Exists(connectionString))
                    Directory.CreateDirectory(connectionString.Substring(0, GetPathEnd(connectionString)));

                //Create DB file
                File.WriteAllBytes(connectionString.GetOsDependentString(), new byte[0]);

                _logger.LogDebug($"Created database file: {connectionString}");

                using (var connection = new SqliteConnection(connectionString.GetSqliteConnectionString()))
                {
                    connection.Open();
                    _logger.LogDebug($"Building database using: {connectionString.GetSqliteConnectionString()}");
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = System.Data.CommandType.Text;
                        command.CommandText = EmbeddedResourceHelper.GetTextResource("QurtzSqlite.sql");

                        if (string.IsNullOrEmpty(command.CommandText))
                        {
                            _logger.LogError($"Embedded resource QurtzSqlite.sql is missing.");
                            throw new Exception("Reading embedded resource QurtzSqlite.sql resulted in null value.");
                        }

                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                    _logger.LogDebug("Quartz database tables created.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to create database.");
            }
        }

        /// <summary>
        /// Find out if a file exist
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns>bool true if file found else false</returns>
        private static bool DbExist(string connectionString)
        {
            return File.Exists(connectionString.GetOsDependentString());
        }

        /// <summary>
        /// Get the last index of path separator
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Int index </returns>
        private static int GetPathEnd(string path)
        {
            if (path.Contains("\\"))
                return path.LastIndexOf("\\", StringComparison.Ordinal);

            return path.Contains("/")
                ? path.LastIndexOf("/", StringComparison.Ordinal)
                : 0;
        }
    }
}