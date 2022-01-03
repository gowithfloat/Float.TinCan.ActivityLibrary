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
    }
}
