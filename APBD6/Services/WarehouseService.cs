using APBD6.Exceptions;
using APBD6.Models;
using APBD6.Order;

namespace APBD6.Warehouse;

public interface IWarehouseService
{
    Task<int> CreateProduct(WarehouseDto dto);
}


public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IProduct_WarehouseRepository _productWarehouseRepository;

    public WarehouseService(IWarehouseRepository warehouseRepository,
        IProductRepository productRepository, IOrderRepository orderRepository,
        IProduct_WarehouseRepository productWarehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _productWarehouseRepository = productWarehouseRepository;
    }

    public async Task<int> CreateProduct(WarehouseDto dto)
    {
        if (dto.Amount <= 0)
        {
            throw new ConflictException("Amount cannot be 0 or lower");
        }
        var product = await _productRepository.GetProduct(dto.IdProduct.Value);
        if (product is null)
        {
            throw new NotFoundException($"There is no product id = {dto.IdProduct}");
        }

        var warehouse = await _warehouseRepository.GetWarehouse(dto.IdWarehouse.Value);
        if (warehouse is null)
        {
            throw new NotFoundException($"There is no warehouse id = {dto.IdWarehouse}");
        }
        const int idOrder = 1;

        var order = await _orderRepository.GetOrder(dto.IdProduct.Value, dto.Amount.Value);
        if (order is null || order.CreatedAt > DateTime.Now)
        {
            throw new NotFoundException($"There is no order id = {idOrder}");
        }

        var productWarehouse = await _productWarehouseRepository.GetProduct_Warehouse(dto.IdProduct.Value, dto.IdWarehouse.Value);
        if (productWarehouse is not null)
        {
            throw new ConflictException("Product already in warehouse");
        }
        
        var idProductWarehouse = await _warehouseRepository.RegisterProduct(
            idWarehouse: dto.IdWarehouse!.Value,
            idProduct: dto.IdProduct!.Value,
            idOrder: idOrder,
            createdAt: DateTime.UtcNow);

        if (!idProductWarehouse.HasValue)
            throw new Exception("Failed to register product in warehouse");

        return idProductWarehouse.Value;
    }
}