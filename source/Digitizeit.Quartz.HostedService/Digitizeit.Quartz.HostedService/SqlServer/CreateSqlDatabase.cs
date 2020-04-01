using Digitizeit.Quartz.HostedService.Extensions;
using Digitizeit.Quartz.HostedService.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Data;
using System.Data.SqlClient;
using Digitizeit.Quartz.HostedService.Interfaces;

namespace Digitizeit.Quartz.HostedService.SqlServer
{
    public class CreateSqlDatabase : ICreateDatabase
    {
        private readonly ILogger<CreateSqlDatabase> _logger;

        public CreateSqlDatabase(ILogger<CreateSqlDatabase> logger)
        {
            _logger = logger ?? new NullLogger<CreateSqlDatabase>();
        }

        public void Init(string connectionString)
        {
            if (!DbExist(connectionString))
            {
                _logger.LogDebug($"Database {connectionString.GetDatabaseNameSqlServer()} not found, trying to create database.");
                CreateDatabase(connectionString);
                return;
            }
            _logger.LogDebug($"Found and using database {connectionString.GetDatabaseNameSqlServer()}");
        }

        private bool DbExist(string connectionString)
        {
            try
            {
                var connection = new SqlConnection(connectionString.GetConnectionOnlySqlServer());
                var sqlCreateDbQuery = $"SELECT database_id FROM sys.databases WHERE Name = '{connectionString.GetDatabaseNameSqlServer()}'";

                using (connection)
                {
                    using (var sqlCmd = new SqlCommand(sqlCreateDbQuery, connection))
                    {
                        connection.Open();
                        var resultObj = sqlCmd.ExecuteScalar();
                        var databaseId = 0;

                        if (resultObj != null)
                        {
                            int.TryParse(resultObj.ToString(), out databaseId);
                        }

                        connection.Close();
                        return (databaseId > 0);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        private void CreateDatabase(string connectionString)
        {
            try
            {
                //Create database
                var connectionOnly = connectionString.GetConnectionOnlySqlServer();
                var database = connectionString.GetDatabaseNameSqlServer();
                using (var connection = new SqlConnection(connectionOnly))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"CREATE DATABASE {database}";
                    command.ExecuteNonQuery();
                }

                _logger.LogDebug($"Database {database} Created.");

                //Create tables for database
                using (var connection = new SqlConnection(connectionOnly))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.Text;

                    _logger.LogDebug("Using QuartzSqlServer.sql to create database tables.");

                    command.CommandText = EmbeddedResourceHelper.GetTextResource("QuartzSqlServer.sql").Replace("my_database_name", database);

                    if (string.IsNullOrEmpty(command.CommandText))
                    {
                        _logger.LogError($"Embedded resource QuartzSqlServer.sql is missing.");
                        throw new Exception("Reading embedded resource QuartzSqlServer.sql resulted in null value.");
                    }

                    command.ExecuteNonQuery();
                }
                _logger.LogDebug("Quartz database tables created.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to create database.");
            }
        }
    }
}