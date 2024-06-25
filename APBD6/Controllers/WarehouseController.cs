using APBD6.Exceptions;
using Microsoft.AspNetCore.Mvc;
namespace APBD6.Warehouse;

[ApiController]
[Route("/api/warehouse")]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;
    public WarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterProduct([FromBody] WarehouseDto dto)
    {
        try
        {
            var idProductWarehouse = await _warehouseService.CreateProduct(dto);
            return Ok(idProductWarehouse);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ConflictException e)
        {
            return Conflict(e.Message);
        }
    }
}