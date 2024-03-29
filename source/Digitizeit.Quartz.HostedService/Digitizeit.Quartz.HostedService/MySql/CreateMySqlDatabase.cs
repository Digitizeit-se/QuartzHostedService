﻿using Digitizeit.Quartz.HostedService.Extensions;
using Digitizeit.Quartz.HostedService.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Specialized;
using System.Data;
using Digitizeit.Quartz.HostedService.Interfaces;
using Digitizeit.Quartz.HostedService.Models;
using Digitizeit.Quartz.HostedService.Options;

namespace Digitizeit.Quartz.HostedService.MySql
{
    public class CreateMySqlDatabase : ICreateDatabase
    {
        private readonly QuartzMySqlOptions _options;
        private readonly JobStore _settings;
        private readonly ILogger<CreateMySqlDatabase> _logger;
        private readonly NameValueCollection _propertiesCollection;
        private readonly string _databaseName;

        public CreateMySqlDatabase(ILogger<CreateMySqlDatabase> logger, QuartzMySqlOptions options, JobStore settings)
        {
            _options = options;
            _settings = settings;
            _logger = logger ?? new NullLogger<CreateMySqlDatabase>();
            _propertiesCollection = _options.GetDatabaseProperties(settings);
            _databaseName = _settings.ConnectionString.GetDatabaseNameSqlServer();
        }

        public NameValueCollection Init()
        {
            if (!DbExist(_settings.ConnectionString))
            {
                _logger.LogDebug($"Database {_databaseName} not found, trying to create database.");
                CreateDatabase(_settings.ConnectionString);
            }

            _logger.LogDebug($"Found and using mysql database name: {_databaseName}");
            return _propertiesCollection;
        }

        private bool DbExist(string connectionString)
        {
            try
            {
                var connection = new MySqlConnection(connectionString.GetConnectionOnlySqlServer());
                var sqlCreateDbQuery = $"select schema_name from information_schema.schemata where schema_name = '{_databaseName}'";

                using (connection)
                {
                    using var sqlCmd = new MySqlCommand(sqlCreateDbQuery, connection);
                    connection.Open();
                    var resultObj = sqlCmd.ExecuteScalar();

                    if (resultObj != null)
                    {
                        if (resultObj.ToString() == _databaseName) return true;
                    }

                    connection.Close();
                    return false;
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

                using (var connection = new MySqlConnection(connectionOnly))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"CREATE DATABASE {_databaseName} CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci";
                    command.ExecuteNonQuery();
                }

                _logger.LogDebug($"Database {_databaseName} Created.");

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