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
    public void TelemetryPoint_ShouldHave_CorrectAttributes()
    {
        var measurementAttr =
            typeof(TelemetryPoint)
                .GetCustomAttributes(typeof(MeasurementAttribute), false)
                .FirstOrDefault() as MeasurementAttribute;

        Assert.NotNull(measurementAttr);
        Assert.Equal("telemetry", measurementAttr!.Name);

        var deviceIdProp =
            typeof(TelemetryPoint).GetProperty(nameof(TelemetryPoint.DeviceId));

        var deviceIdAttr =
            deviceIdProp!
                .GetCustomAttributes(typeof(ColumnAttribute), false)
                .FirstOrDefault() as ColumnAttribute;

        Assert.NotNull(deviceIdAttr);
        Assert.True(deviceIdAttr!.IsTag);

        var timestampProp =
            typeof(TelemetryPoint).GetProperty(nameof(TelemetryPoint.Timestamp));

        var timestampAttr =
            timestampProp!
                .GetCustomAttributes(typeof(ColumnAttribute), false)
                .FirstOrDefault() as ColumnAttribute;

        Assert.NotNull(timestampAttr);
        Assert.True(timestampAttr!.IsTimestamp);
    }
}