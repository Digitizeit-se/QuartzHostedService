using Digitizeit.Quartz.HostedService.Helpers;
using Xunit;

namespace Digitizeit.Quartz.HostedService.Test.Helpers
{
    public class EmbeddedResourceHelperTest
    {
        [Fact]
        public void When_accessing_embedded_resource_QuartzMySql_embedded_resource_is_returned()
        {
            //Arrange
            var resourceText = "DROP TABLE";
            var fileName = "QuartzMySql.sql";

            //Act
            var resourceResult = EmbeddedResourceHelper.GetTextResource(fileName);

            //Assert
            Assert.NotNull(resourceResult);
            Assert.Contains(resourceText, resourceText);
        }

        [Fact]
        public void When_accessing_embedded_resource_QurtzSqlite_embedded_resource_is_returned()
        {
            //Arrange
            var resourceText = "DROP TABLE";
            var fileName = "QurtzSqlite.sql";

            //Act
            var resourceResult = EmbeddedResourceHelper.GetTextResource(fileName);

            //Assert
            Assert.NotNull(resourceResult);
            Assert.Contains(resourceText, resourceText);
        }

        [Fact]
        public void When_accessing_embedded_resource_QuartzSqlServer_embedded_resource_is_returned()
        {
            //Arrange
            var resourceText = "DROP TABLE";
            var fileName = "QuartzSqlServer.sql";

            //Act
            var resourceResult = EmbeddedResourceHelper.GetTextResource(fileName);

            //Assert
            Assert.NotNull(resourceResult);
            Assert.Contains(resourceText, resourceText);
        }
    }
}