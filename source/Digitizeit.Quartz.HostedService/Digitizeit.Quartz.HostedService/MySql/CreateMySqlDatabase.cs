using Digitizeit.Quartz.HostedService.Extensions;
using Digitizeit.Quartz.HostedService.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using Digitizeit.Quartz.HostedService.Interfaces;

namespace Digitizeit.Quartz.HostedService.MySql
{
    public class CreateMySqlDatabase : ICreateDatabase
    {
        private readonly ILogger<CreateMySqlDatabase> _logger;

        public CreateMySqlDatabase(ILogger<CreateMySqlDatabase> logger)
        {
            _logger = logger ?? new NullLogger<CreateMySqlDatabase>();
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
                var connection = new MySqlConnection(connectionString.GetConnectionOnlySqlServer());
                var sqlCreateDbQuery = $"select schema_name from information_schema.schemata where schema_name = '{ connectionString.GetDatabaseNameSqlServer()}'";

                using (connection)
                {
                    using (var sqlCmd = new MySqlCommand(sqlCreateDbQuery, connection))
                    {
                        connection.Open();
                        var resultObj = sqlCmd.ExecuteScalar();

                        if (resultObj != null)
                        {
                            if (resultObj.ToString() == connectionString.GetDatabaseNameSqlServer()) return true;
                        }

                        connection.Close();
                        return false;
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

                using (var connection = new MySqlConnection(connectionOnly))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"CREATE DATABASE {database} CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci";
                    command.ExecuteNonQuery();
                }

                _logger.LogDebug($"Database {database} Created.");

                //Create tables for database
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.Text;

                    _logger.LogDebug("using QuartzMySql.sql to create database tables.");

                    command.CommandText = EmbeddedResourceHelper.GetTextResource("QuartzMySql.sql");
                    if (string.IsNullOrEmpty(command.CommandText))
                    {
                        _logger.LogError($"Embedded resource QuartzMySql.sql is missing.");
                        throw new Exception("Reading embedded resource QuartzMySql.sql resulted in null value.");
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