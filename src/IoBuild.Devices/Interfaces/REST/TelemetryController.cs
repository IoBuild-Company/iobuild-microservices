using IoBuild.Devices.Domain.Model.Queries;
using IoBuild.Devices.Domain.Services;
using IoBuild.Devices.Interfaces.REST.Transform;
using IoBuild.Shared.Infrastructure.ASP.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace IoBuild.Devices.Interfaces.REST;

[ApiController]
[Route("api/v1/devices")]
[Authorize]
public class TelemetryController(IDeviceQueryService deviceQueryService, ITelemetryQueryService telemetryQueryService)
    : ControllerBase
{
    [HttpGet("{id}/energy")]
    public async Task<IActionResult> GetEnergy(int id, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var device = await deviceQueryService.Handle(new GetDeviceByIdQuery(id));
        if (device is null)
            return NotFound();

        var toValue = to ?? DateTime.UtcNow;
        var fromValue = from ?? toValue.AddHours(-24);

        var query = new GetDeviceEnergyQuery(id, fromValue, toValue);
        var results = await telemetryQueryService.Handle(query);
        var resources = results.Select(TelemetryResourceAssembler.ToResourceFromEntity);

        return Ok(resources);
    }

    [HttpGet("{id}/status")]
    public async Task<IActionResult> GetStatus(int id)
    {
        var device = await deviceQueryService.Handle(new GetDeviceByIdQuery(id));
        if (device is null)
            return NotFound();

        var query = new GetDeviceStatusQuery(id);
        var report = await telemetryQueryService.Handle(query);

        if (report is null)
            return Ok(TelemetryResourceAssembler.ToUnknownStatusResource(id));

        var resource = TelemetryResourceAssembler.ToResourceFromEntity(report);
        return Ok(resource);
    }
}
