using InfluxDB.Client.Core;
using IoBuild.Devices.Infrastructure.InfluxDB;
using IoBuild.Devices.Infrastructure.Mqtt;
using Xunit;

namespace IoBuild.Devices.Tests.UnitTests;

public class TelemetryInfrastructureTests
{
    [Fact]
    public void InfluxDbOptions_ShouldHave_DefaultValues()
    {
        var options = new InfluxDbOptions();

        Assert.Equal("influxdb", options.Host);
        Assert.Equal(8086, options.Port);
        Assert.Equal("iobuild", options.Org);
        Assert.Equal("iobuild-telemetry", options.Bucket);
    }

    [Fact]
    public void MqttOptions_ShouldHave_DefaultValues()
    {
        var options = new MqttOptions();

        Assert.Equal("mosquitto", options.Host);
        Assert.Equal(1883, options.Port);
        Assert.Equal("telemetry/#", options.Topic);
    }

    [Fact]
    public void TelemetryPoint_ShouldHave_MeasurementAttribute()
    {
        var measurementAttribute = typeof(TelemetryPoint)
            .GetCustomAttributes(typeof(Measurement), false)
            .Cast<Measurement>()
            .FirstOrDefault();

        Assert.NotNull(measurementAttribute);
        Assert.Equal("telemetry", measurementAttribute!.Name);
    }

    [Fact]
    public void DeviceId_ShouldHave_ColumnAttribute()
    {
        var property = typeof(TelemetryPoint)
            .GetProperty(nameof(TelemetryPoint.DeviceId));

        var attribute = property!
            .GetCustomAttributes(typeof(Column), false)
            .Cast<Column>()
            .FirstOrDefault();

        Assert.NotNull(attribute);
    }

    [Fact]
    public void Timestamp_ShouldHave_ColumnAttribute()
    {
        var property = typeof(TelemetryPoint)
            .GetProperty(nameof(TelemetryPoint.Timestamp));

        var attribute = property!
            .GetCustomAttributes(typeof(Column), false)
            .Cast<Column>()
            .FirstOrDefault();

        Assert.NotNull(attribute);
    }
}