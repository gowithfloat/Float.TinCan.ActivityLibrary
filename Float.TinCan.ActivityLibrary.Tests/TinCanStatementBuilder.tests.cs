using Xunit;

namespace Float.TinCan.ActivityLibrary.Tests
{
    public class TinCanStatementBuilderTests
    {
        [Fact]
        public void TestBuildStatement()
        {
            var statement = new TinCanStatementBuilder()
                .SetVerb("http://example.com/tested", "tested")
                .SetEmail("test@example.com")
                .Build();
            Assert.NotNull(statement);
        }

        [Fact]
        public void TestDefaultStatementTimestamp()
        {
            var statement = new TinCanStatementBuilder()
                .Build();

            // The default timestamp should be (approximately) now.
            Assert.InRange(statement.timestamp ?? new System.DateTime(), System.DateTime.UtcNow.AddSeconds(-1), System.DateTime.UtcNow);
            System.Console.WriteLine(statement.ToJSON());
        }
    }
}
