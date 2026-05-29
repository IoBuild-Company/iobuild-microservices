using System.Reflection;
using System.Text.Json;
using FluentAssertions;
using IoBuild.Devices.Infrastructure.InfluxDB;
using IoBuild.Devices.Infrastructure.Mqtt;
using IoBuild.Devices.Workers;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace IoBuild.Devices.Tests.UnitTests;

public class TelemetryWorkerTests
{
    [Fact]
    public void TelemetryRawPayload_ShouldDeserializeSnakeCaseJson()
    {
        var json = """
        {
          "deviceId": 7,
          "timestamp": "2026-05-28T12:34:56Z",
          "energy_kwh": 10.5,
          "temperature_c": 22.7,
          "voltage_v": 219.8,
          "status": "online",
          "location": "line-a"
        }
        """;

        var payloadType = GetTelemetryRawPayloadType();

        var payload = JsonSerializer.Deserialize(
            json,
            payloadType,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = false
            });

        payload.Should().NotBeNull();

        GetValue<int>(payload!, "deviceId").Should().Be(7);
        GetValue<DateTime>(payload!, "timestamp").Should().Be(new DateTime(2026, 5, 28, 12, 34, 56, DateTimeKind.Utc));
        GetValue<double>(payload!, "energy_kwh").Should().Be(10.5);
        GetValue<double>(payload!, "temperature_c").Should().Be(22.7);
        GetValue<double>(payload!, "voltage_v").Should().Be(219.8);
        GetValue<string?>(payload!, "status").Should().Be("online");
        GetValue<string?>(payload!, "location").Should().Be("line-a");
    }

    [Fact]
    public void TelemetryRawPayload_ShouldThrowForInvalidJson()
    {
        var payloadType = GetTelemetryRawPayloadType();

        var invalidJson = """
        {
          "deviceId": 1,
          "energy_kwh":
        }
        """;

        Action act = () => JsonSerializer.Deserialize(invalidJson, payloadType);

        act.Should().Throw<JsonException>();
    }

    [Fact(Skip = "Current worker hides the MQTT loop inside private methods; this test needs a small refactor to be deterministic.")]
    public async Task Should_Retry_WhenMqttConnectionFails()
    {
        var writeService = new Mock<ITelemetryWriteService>();
        var mqttOptions = Options.Create(new MqttOptions());
        var logger = NullLogger<TelemetryWorker>.Instance;

        _ = new TelemetryWorker(writeService.Object, mqttOptions, logger);

        await Task.CompletedTask;
    }

    private static Type GetTelemetryRawPayloadType()
    {
        return typeof(TelemetryWorker).GetNestedType("TelemetryRawPayload", BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("TelemetryRawPayload type was not found.");
    }

    private static T GetValue<T>(object instance, string propertyName)
    {
        var property = instance.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException($"Property '{propertyName}' was not found.");

        return (T)property.GetValue(instance)!;
    }
}