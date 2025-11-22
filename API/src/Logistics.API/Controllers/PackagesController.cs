using Logistics.Application.DTOs.Package;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PackagesController : ControllerBase
{
    private readonly IPackageService _service;

    public PackagesController(IPackageService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<PackageResponse>> Create([FromBody] CreatePackageRequest request)
    {
        var package = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = package.Id }, package);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PackageResponse>> GetById(Guid id)
    {
        var package = await _service.GetByIdAsync(id);
        return Ok(package);
    }

    [HttpGet("tracking/{trackingNumber}")]
    public async Task<ActionResult<PackageResponse>> GetByTracking(string trackingNumber)
    {
        var package = await _service.GetByTrackingNumberAsync(trackingNumber);
        return Ok(package);
    }

    [HttpGet("packing-task/{packingTaskId}")]
    public async Task<ActionResult<List<PackageResponse>>> GetByPackingTask(Guid packingTaskId)
    {
        var packages = await _service.GetByPackingTaskIdAsync(packingTaskId);
        return Ok(packages.ToList());
    }

    [HttpPost("{id}/ship")]
    public async Task<ActionResult> Ship(Guid id)
    {
        await _service.ShipPackageAsync(id);
        return Ok();
    }
}
