using FluentAssertions;
using QueryUrlParams.Attributes;

namespace QueryUrlParams.IntegrationTests
{
    [GenerateQueryUrl]
    public class SomeUrlParams
    {
        public string? Name { get; set; }
        public int? Age { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public List<string>? Tags { get; set; }
        public Dictionary<string, string>? Metadata { get; set; }
        public bool IsValid { get; set; }
        public InnerUrlParams? InnerParams { get; set; }
    }

    [GenerateQueryUrl]
    public class InnerUrlParams
    {
        public string? InnerName { get; set; }
        public int? InnerAge { get; set; }
    }

    public class QueryUrlParametersBuildTests
    {
        [Fact]
        public void ToQueryUrl_Should_Handle_Primitives_Collections_And_NestedObjects()
        {
            // Arrange
            var startTime = DateTime.UtcNow;
            var endTime = DateTime.UtcNow.AddHours(1);

            var urlParams = new SomeUrlParams
            {
                Name = "John Doe",
                Age = 30,
                PhoneNumber = "123-456-7890",
                Email = "fake@email.com",
                StartTime = startTime,
                EndTime = endTime,
                Tags = new List<string> { "tag1", "tag2" },
                Metadata = new Dictionary<string, string>
                {
                    { "key1", "value1" },
                    { "key2", "value2" }
                },
                IsValid = true,
                InnerParams = new InnerUrlParams
                {
                    InnerName = "Inner Name",
                    InnerAge = 25
                }
            };

            // Act
            var baseUrl = "https://example.com/api";
            var uri = urlParams.ToQueryUrl(baseUrl);

            // Assert
            uri.Should().NotBeNullOrEmpty();
            uri.Should().StartWith(baseUrl);
            uri.Should().Contain("name=John%20Doe");
            uri.Should().Contain("age=30");
            uri.Should().Contain("phone_number=123-456-7890");
            uri.Should().Contain("email=fake%40email.com");
            uri.Should().Contain("start_time=" + Uri.EscapeDataString(startTime.ToString("yyyy-MM-ddTHH:mm:ssZ")));
            uri.Should().Contain("end_time=" + Uri.EscapeDataString(endTime.ToString("yyyy-MM-ddTHH:mm:ssZ")));
            uri.Should().Contain("tags=tag1");
            uri.Should().Contain("tags=tag2");
            uri.Should().Contain("key1=value1");
            uri.Should().Contain("key2=value2");
            uri.Should().Contain("is_valid=true");
            uri.Should().Contain("inner_name=Inner%20Name");
            uri.Should().Contain("inner_age=25");
        }
    }
}