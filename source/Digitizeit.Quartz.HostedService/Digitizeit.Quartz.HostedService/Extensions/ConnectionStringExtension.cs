using System;
using System.IO;
using System.Linq;

namespace Digitizeit.Quartz.HostedService.Extensions
{
    public static class ConnectionStringExtension
    {
        public static string GetDatabaseNameSqlServer(this string connectionString)
        {
            if (!connectionString.Contains("Database="))
            {
                throw new MissingFieldException("Database name missing in connectionString");
            }

            var parts = connectionString.Split(';');
            foreach (var part in parts)
            {
                if (part.Contains("Database="))
                {
                    return part.Replace("Database=", "");
                }
            }
            throw new MissingFieldException("Database name missing in connectionString");
        }

        public static string GetConnectionOnlySqlServer(this string connectionString)
        {
            if (!connectionString.Contains("Database=")) return connectionString;

            var parts = connectionString.Split(';');

            var returnString = parts
                .Where(part => !part.Contains("Database="))
                .Aggregate("", (current, part) => current + (part + ";"));

            return returnString.TrimEnd(';');
        }

        public static string GetDatabaseNameSqlite(this string connectionString)
        {
            var directorySeparatorChar = "";
            if (connectionString.Contains("/")) directorySeparatorChar = "/";
            if (directorySeparatorChar.Contains("\\")) directorySeparatorChar = "\\";
            return directorySeparatorChar == ""
                ? connectionString
                : connectionString.Substring(connectionString.LastIndexOf(directorySeparatorChar));
        }

        public static string GetOsDependentString(this string connectionString)
        {
            connectionString = connectionString.Replace('\\', Path.DirectorySeparatorChar);
            connectionString = connectionString.Replace('/', Path.DirectorySeparatorChar);
            return connectionString;
        }

        public static string GetSqliteConnectionString(this string connectionString)
        {
            return $"Data Source={connectionString.GetOsDependentString()}";
        }
    }
}