using IoBuild.Devices.Domain.Model.Aggregates;
using IoBuild.Devices.Interfaces.REST.Resources;

namespace IoBuild.Devices.Interfaces.REST.Transform;

public static class TelemetryResourceAssembler
{
    public static EnergyDataResource ToResourceFromEntity(EnergyDataPoint point)
    {
        return new EnergyDataResource(
            point.Timestamp,
            point.EnergyKwh,
            point.TemperatureC,
            point.VoltageV
        );
    }

    public static DeviceStatusResource ToResourceFromEntity(DeviceStatusReport report)
    {
        return new DeviceStatusResource(
            report.DeviceId,
            report.Status,
            report.LastSeen,
            report.TemperatureC,
            report.VoltageV
        );
    }

    public static DeviceStatusResource ToUnknownStatusResource(int deviceId)
    {
        return new DeviceStatusResource(
            deviceId,
            "unknown",
            null,
            null,
            null
        );
    }
}
