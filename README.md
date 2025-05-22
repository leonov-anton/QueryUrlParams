# QueryUrlParams

**QueryUrlParams** is a C# Source Generator high-performance source generator for building query URLs from DTO objects in C#.  
This library generates `ToQueryString()` extension methods for your DTO classes, at compile time, to efficiently convert your decorated DTO classes into URL query strings, avoiding reflection overhead. You control the output using a set of custom attributes.

## ✨ Features
- Compile-time generation of `ToQueryString()` extension methods.
- Custom attributes for fine-grained control over serialization.
- Simple, clear syntax and predictable output.
- No reflection – high performance at runtime.
- Supports:
  - Nullable types
  - Dates and booleans
  - Nested decoreted DTOs
  - Arrays and collections (e.g., `List<T>`, `T[]`)
  - Custom objects (using ToString() method)
 
## 📋 Usage
Follow these steps to get started:
1. **Mark your DTO class with the `[GenerateQueryUrl]` attribute**  
   This enables the source generator to process your class and generate the extension method.
   You can optionally specify parameters to customize the behavior, for example:
   - Set a **Base URL** for the generated method, so you don’t need to pass it explicitly every time.  
   - Disable automatic conversion of property names to `snake_case` if you want to keep original format in lower-case. Set **SnakeCaseNameConvert** to `false`.
3. **Customize properties with attributes** (optional)  
   - `[QueryParameterIgnore]` — exclude a property from query parameters  
   - `[QueryParameterName("customName")]` — specify a custom key for a property  
   - `[DateTimeFormat("yyyy-MM-dd")]` — format DateTime properties
4. **Call `.ToQueryUrl(baseUrl)` on your DTO instance**  
   This returns the full URL with all non-null properties serialized as query parameters with passing Base Url or without it.

## 🛠️ Example
```csharp
using QueryUrlParams;

[GenerateQueryUrl]
public class SomeUrlParams
{
    public string? Name { get; set; }

    public int? Age { get; set; }

    [QueryParameterIgnore] // This property will be ignored
    public string? InternalToken { get; set; }

    [QueryParameterName("phone")] // Use a custom key name in the query string
    public string? PhoneNumber { get; set; }

    [DateTimeFormat("yyyy-MM-dd")] // Format DateTime to only include date
    public DateTime? StartDate { get; set; }

    public List<string>? Tags { get; set; }

    public Dictionary<string, string>? Metadata { get; set; }

    public bool? IsActive { get; set; }
}

var dto = new SomeUrlParams
        {
            Name = "Alice",
            Age = 28,
            InternalToken = "secret", // Will NOT appear in URL
            PhoneNumber = "+1234567890",
            StartDate = new DateTime(2024, 5, 22),
            Tags = new List<string> { "admin", "tester" },
            Metadata = new Dictionary<string, string>
            {
                { "ref", "email" },
                { "campaign", "spring2024" }
            },
            IsActive = true
        };

string baseUrl = "https://example.com/api";

string url = dto.ToQueryUrl(baseUrl);
// Result: https://example.com/api/?name=Alice&age=28&phone=%2B1234567890&start_date=2024-05-22&tags=admin&tags=tester&ref=email&campaign=spring2024&is_active=true
```

## 📊 Performance Benchmark
**QueryUrlParams** shows significant performance improvements compared to traditional reflection-based URL generation.
| Method                     | Mean     | Error     | StdDev    | Gen0   | Allocated |
|--------------------------- |---------:|----------:|----------:|-------:|----------:|
| GenerateQueryUrl           | 1.513 us | 0.0300 us | 0.0718 us | 0.5449 |   2.23 KB |
| GenerateQueryUrlReflection | 5.200 us | 0.1020 us | 0.1990 us | 0.8672 |   3.57 KB |

This [benchmark](./test/QueryUrlParams.Benchmarks/QueryUrlParamsBenchmarks.cs) was measured by executing 1000 URL generations per method per iteration, capturing the total time for all and then averaging. The results demonstrate that source-generated code is about 3.5x faster and allocates significantly less memory then [reflection-based URL generation](./test/QueryUrlParams.Benchmarks/QueryUrlParamsReflection.cs).

## 📄 License
MIT License — feel free to use and contribute!

## 🙋 Contributing
Contributions and feedback are welcome. Please open issues ✌️
