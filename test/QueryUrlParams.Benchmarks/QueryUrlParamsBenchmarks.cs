﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Bogus;
using QueryUrlParams.Attributes;


namespace QueryUrlParams.Benchmarks
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
        public bool? IsValid { get; set; }
    }

    [MemoryDiagnoser]
    public class QueryUrlParamsBenchmarks
    {
        private List<SomeUrlParams> _dtos;
        private const string BaseUrl = "https://example.com/api";

        [GlobalSetup]
        public void Setup()
        {
            var faker = new Faker<SomeUrlParams>()
                .RuleFor(x => x.Name, f => f.Name.FullName())
                .RuleFor(x => x.Age, f => f.Random.Int(18, 90).OrNull(f, 0.2f))
                .RuleFor(x => x.PhoneNumber, f => f.Phone.PhoneNumber().OrNull(f, 0.2f))
                .RuleFor(x => x.Email, f => f.Internet.Email().OrNull(f, 0.2f))
                .RuleFor(x => x.StartTime, f => f.Date.Past().OrNull(f, 0.2f))
                .RuleFor(x => x.EndTime, f => f.Date.Past().OrNull(f, 0.2f))
                .RuleFor(x => x.Tags, f => f.Make(f.Random.Int(2, 10), () => f.Lorem.Word()).OrNull(f, 0.4f))
                .RuleFor(x => x.Metadata, f => f.Make(f.Random.Int(1, 5), () =>
                    {
                        var key = f.Lorem.Word() + "_" + f.Random.AlphaNumeric(4);
                        var value = f.Lorem.Word();
                        return new KeyValuePair<string, string>(key, value);
                    }).ToDictionary(kvp => kvp.Key, kvp => kvp.Value).OrNull(f, 0.4f))
                .RuleFor(x => x.IsValid, f => f.Random.Bool().OrNull(f, 0.5f));

            _dtos = faker.Generate(1000);
        }

        private readonly Consumer _consumer = new();

        [Benchmark(OperationsPerInvoke = 1000)]
        public void GenerateQueryUrl()
        {
            for (int i = 0; i < 1000; i++)
            {
                var queryUrl = _dtos[i].ToQueryUrl(BaseUrl);
                _consumer.Consume(queryUrl);
            }
        }

        [Benchmark(OperationsPerInvoke = 1000)]
        public void GenerateQueryUrlReflection()
        {
            for (int i = 0; i < 1000; i++)
            {
                var queryUrl = QueryUrlParamsReflection.ToQueryUrl(_dtos[i], BaseUrl, true);
                _consumer.Consume(queryUrl);
            }
        }
    }
}
