using FluentAssertions;
using QueryUrlParams.Attributes;

namespace QueryUrlParams.IntegrationTests
{
    #region UrlParamsObjects
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
        public bool? IsValid { get; set; }
        public InnerUrlParams? InnerParams { get; set; }
        public decimal? Price { get; set; }
    }

    [GenerateQueryUrl]
    public class SomeUrlParamsNotNull
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<string> Tags { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public bool IsValid { get; set; }
        public InnerUrlParams InnerParams { get; set; }
        public decimal Price { get; set; }
    }

    [GenerateQueryUrl]
    public class InnerUrlParams
    {
        public string? InnerName { get; set; }
        public int? InnerAge { get; set; }
    }


    public enum Status
    {
        Active,
        Inactive,
        Pending
    }

    [GenerateQueryUrl]
    public class EnumUrlParams
    {
        public Status? CurrentStatus { get; set; }
    }

    [GenerateQueryUrl]
    public class EnumUrlToStringParams
    {
        [EnumAsString]
        public Status? CurrentStatus { get; set; }
    }

    [GenerateQueryUrl]
    public class DateTimeFormatParams
    {
        [DateTimeFormat("yyyy-MM")]
        public DateTime? YearAndMoths { get; set; }
    }

    [GenerateQueryUrl(baseUrl: "https://example.com/api", snakeCaseNameConvert: false)]
    public class CustomBaseUrlParams
    {
        public string? UserName { get; set; }
        public int? Age { get; set; }
    }

    [GenerateQueryUrl]
    public class UrlParamsWithInnerClass
    {
        public string? Name { get; set; }
        public InnerClass? InnerParams { get; set; }
    }

    public class InnerClass
    {
        public string? InnerName { get; set; }
        public int? InnerAge { get; set; }

        public override string ToString()
        {
            return $"{InnerName} {InnerAge}";
        }
    }
    #endregion

    public class QueryUrlParametersBuildTests
    {
        [Fact]
        public void ToQueryUrl_Should_Handle_Primitives_And_Collections()
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
                Price = 19.99m,
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
            uri.Should().Contain("price=19.99");
        }

        [Fact]
        public void ToQueryUrl_Should_Handle_Primitives_And_Collections_Not_Nullable()
        {
            // Arrange
            var startTime = DateTime.UtcNow;
            var endTime = DateTime.UtcNow.AddHours(1);

            var urlParams = new SomeUrlParamsNotNull
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
                Price = 19.99m,
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
            uri.Should().Contain("price=19.99");
        }

        [Fact]
        public void ToQueryUrl_Should_Return_BaseUrl_When_No_Parameters()
        {
            // Arrange
            var urlParams = new SomeUrlParams();

            // Act
            var baseUrl = "https://example.com/api";
            var uri = urlParams.ToQueryUrl(baseUrl);

            // Assert
            uri.Should().NotBeNullOrEmpty();
            uri.Should().Be(baseUrl);
        }

        [Fact]
        public void ToQueryUrl_Should_Handle_Nullable_Properties()
        {
            // Arrange
            var urlParams = new SomeUrlParams
            {
                Name = null,
                Age = null,
                PhoneNumber = null,
                Email = null,
                StartTime = null,
                EndTime = null,
                Tags = null,
                Metadata = null,
                IsValid = null,
                InnerParams = null
            };

            // Act
            var baseUrl = "https://example.com/api";
            var uri = urlParams.ToQueryUrl(baseUrl);

            // Assert
            uri.Should().NotBeNullOrEmpty();
            uri.Should().Be(baseUrl);
        }

        [Fact]
        public void ToQueryUrl_Should_Handle_Empty_Collections()
        {
            // Arrange
            var urlParams = new SomeUrlParams
            {
                Tags = new List<string>(),
                Metadata = new Dictionary<string, string>()
            };

            // Act
            var baseUrl = "https://example.com/api";
            var uri = urlParams.ToQueryUrl(baseUrl);

            // Assert
            uri.Should().NotBeNullOrEmpty();
            uri.Should().Be(baseUrl);
        }

        [Fact]
        public void ToQueryUrl_Should_Handle_Complex_Nested_Objects()
        {
            // Arrange
            var urlParams = new SomeUrlParams
            {
                InnerParams = new InnerUrlParams
                {
                    InnerName = "Nested Name",
                    InnerAge = 40
                }
            };

            // Act
            var baseUrl = "https://example.com/api";
            var uri = urlParams.ToQueryUrl(baseUrl);

            // Assert
            uri.Should().NotBeNullOrEmpty();
            uri.Should().Contain("inner_name=Nested%20Name");
            uri.Should().Contain("inner_age=40");
        }

        [Fact]
        public void ToQueryUrl_Should_Handle_Enums_As_Numeric()
        {
            // Arrange
            var urlParams = new EnumUrlParams
            {
                CurrentStatus = Status.Active
            };

            // Act
            var baseUrl = "https://example.com/api";
            var uri = urlParams.ToQueryUrl(baseUrl);

            // Assert
            uri.Should().NotBeNullOrEmpty();
            uri.Should().Contain("current_status=0");
        }

        [Fact]
        public void ToQueryUrl_Should_Handle_Enums_As_String()
        {
            // Arrange
            var urlParams = new EnumUrlToStringParams
            {
                CurrentStatus = Status.Inactive
            };

            // Act
            var baseUrl = "https://example.com/api";
            var uri = urlParams.ToQueryUrl(baseUrl);

            // Assert
            uri.Should().NotBeNullOrEmpty();
            uri.Should().Contain("current_status=Inactive");
        }

        [Fact]
        public void ToQueryUrl_Should_Handle_DateTime_With_Custom_Format()
        {
            // Arrange
            var time = DateTime.UtcNow;

            var urlParams = new DateTimeFormatParams
            {
                YearAndMoths = time
            };
            
            // Act
            var baseUrl = "https://example.com/api";
            var uri = urlParams.ToQueryUrl(baseUrl);
            
            // Assert
            uri.Should().NotBeNullOrEmpty();
            uri.Should().Contain("year_and_moths=" + Uri.EscapeDataString(time.ToString("yyyy-MM")));
        }

        [Fact]
        public void ToQueryUrl_Should_Handle_Custom_BaseUrl_And_SnakeCase()
        {
            // Arrange
            var urlParams = new CustomBaseUrlParams
            {
                UserName = "JaneDoe",
                Age = 25
            };
            
            // Act
            var uri = urlParams.ToQueryUrl();
            
            // Assert
            uri.Should().NotBeNullOrEmpty();
            uri.Should().StartWith("https://example.com/api");
            uri.Should().Contain("username=JaneDoe");
            uri.Should().Contain("age=25");
        }

        [Fact]
        public void ToQueryUrl_Should_Handle_Inner_Class_ToString()
        {
            // Arrange
            var urlParams = new UrlParamsWithInnerClass
            {
                Name = "Test",
                InnerParams = new InnerClass
                {
                    InnerName = "InnerTest",
                    InnerAge = 30
                }
            };
            
            // Act
            var baseUrl = "https://example.com/api";
            var uri = urlParams.ToQueryUrl(baseUrl);
            
            // Assert
            uri.Should().NotBeNullOrEmpty();
            uri.Should().Contain("name=Test");
            uri.Should().Contain($"inner_params={Uri.EscapeDataString("InnerTest 30")}");
        }
    }
}