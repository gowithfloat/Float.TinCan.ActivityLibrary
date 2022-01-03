using Float.TinCan.ActivityLibrary.Definition;
using Xunit;

namespace Float.TinCan.ActivityLibrary.Tests
{
    public class ActivityMetaDataStructTests
    {
        [Fact]
        public void TestInit()
        {
            _ = new ActivityMetaDataStruct();
            _ = new ActivityMetaDataStruct
            {
                Title = "Test Title",
            };
            _ = new ActivityMetaDataStruct
            {
                StartLocation = "http://example.com"
            };
            _ = new ActivityMetaDataStruct
            {
                UUID = "invalid"
            };
        }

        [Fact]
        public void TestEquality()
        {
            var s1 = new ActivityMetaDataStruct();
            var s2 = new ActivityMetaDataStruct();
            Assert.Equal(s1, s2);
            Assert.Equal(s1.StartLocation, s2.StartLocation);
            Assert.Equal(s1.Title, s2.Title);
            Assert.Equal(s1.UUID, s2.UUID);
        }
    }
}
