using Digitizeit.Quartz.HostedService.Extensions;
using System;
using Xunit;

namespace Digitizeit.Quartz.HostedService.Test.Extensions
{
    public class ConnectionStringExtensionTest
    {
        [Fact]
        public void When_given_connectionString_Database_name_is_extracted_correct()
        {
            //Arrange
            var connectionString =
                "Server=localhost;Database=Quartz;User Id=sa;Password=Secret123!%;MultipleActiveResultSets=true";
            var expected = "Quartz";

            //Act
            var result = connectionString.GetDatabaseNameSqlServer();

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void When_given_connectionString_Without_Database_name_MissingFieldException_is_thrown()
        {
            //Arrange
            var connectionString = "Server=localhost;User Id=sa;Password=Secret123!%;MultipleActiveResultSets=true";

            //Assert
            Assert.Throws<MissingFieldException>(() => connectionString.GetDatabaseNameSqlServer());
        }

        [Fact]
        public void When_given_connectionString_ConnectionString_without_databse_name_is_extracted()
        {
            //Arrange
            var connectionString = "Server=localhost;Database=Quartz;User Id=sa;Password=Secret123!%;MultipleActiveResultSets=true";
            var expected = "Server=localhost;User Id=sa;Password=Secret123!%;MultipleActiveResultSets=true";

            //Act
            var result = connectionString.GetConnectionOnlySqlServer();

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void When_given_connectionString_without_databaseName_Correct_connectionString_is_returned()
        {
            //Arrange
            var connectionString = "Server=localhost;User Id=sa;Password=Secret123!%;MultipleActiveResultSets=true";
            var expected = "Server=localhost;User Id=sa;Password=Secret123!%;MultipleActiveResultSets=true";

            //Act
            var result = connectionString.GetConnectionOnlySqlServer();

            //Assert
            Assert.Equal(expected, result);
        }
    }
}