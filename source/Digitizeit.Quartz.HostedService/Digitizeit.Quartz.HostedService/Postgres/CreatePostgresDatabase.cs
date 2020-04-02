using Digitizeit.Quartz.HostedService.Extensions;
using Digitizeit.Quartz.HostedService.Helpers;
using Digitizeit.Quartz.HostedService.Interfaces;
using Digitizeit.Quartz.HostedService.Models;
using Digitizeit.Quartz.HostedService.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using System;
using System.Collections.Specialized;
using System.Data;

namespace Digitizeit.Quartz.HostedService.Postgres
{
    public class CreatePostgresDatabase : ICreateDatabase
    {
        private readonly QuartzPostgresOptions _options;
        private readonly JobStore _settings;
        private readonly ILogger<CreatePostgresDatabase> _logger;
        private readonly NameValueCollection _propertiesCollection;
        private readonly string _databseName;

        public CreatePostgresDatabase(ILogger<CreatePostgresDatabase> logger, QuartzPostgresOptions options, JobStore settings)
        {
            _options = options;
            _settings = settings;
            _logger = logger ?? new NullLogger<CreatePostgresDatabase>();
            _propertiesCollection = _options.GetDatabaseProperties(settings);
            _databseName = _settings.ConnectionString.GetDatabaseNameSqlServer();
        }

        public NameValueCollection Init()
        {
            if (!DbExist(_settings.ConnectionString))
            {
                _logger.LogDebug($"Database {_settings.ConnectionString.GetDatabaseNameSqlServer()} not found, trying to create database.");
                CreateDatabase(_settings.ConnectionString);
            }

            _logger.LogDebug($"Found and using Postgres database name: {_databseName}");
            return _propertiesCollection;
        }

        private bool DbExist(string connectionString)
        {
            try
            {
                var connection = new NpgsqlConnection(connectionString.GetConnectionOnlySqlServer());
                var sqlCreateDbQuery = $"SELECT datname from pg_database WHERE datname='{_databseName}'";

                using (connection)
                {
                    using (var sqlCmd = new NpgsqlCommand(sqlCreateDbQuery, connection))
                    {
                        connection.Open();
                        var resultObj = sqlCmd.ExecuteScalar();

                        if (resultObj != null)
                        {
                            if (resultObj.ToString() == _databseName) return true;
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

                using (var connection = new NpgsqlConnection(connectionOnly))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"CREATE DATABASE {_databseName}";
                    command.ExecuteNonQuery();
                }

                _logger.LogDebug($"Database {_databseName} Created.");

                //Create tables for database
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.Text;

                    _logger.LogDebug("using QuartzPostgres.sql to create database tables.");

                    command.CommandText = EmbeddedResourceHelper.GetTextResource("QuartzPostgres.sql");
                    if (string.IsNullOrEmpty(command.CommandText))
                    {
                        _logger.LogError($"Embedded resource QuartzPostgres.sql is missing.");
                        throw new Exception("Reading embedded resource QuartzPostgres.sql resulted in null value.");
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