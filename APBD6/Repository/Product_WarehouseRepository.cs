using System.Data.SqlClient;
using APBD6.Warehouse;

namespace APBD6.Models;

public interface IProduct_WarehouseRepository
{
    Task<Product_Warehouse?> GetProduct_Warehouse(int idProduct, int idWarehouse);
}

public class Product_WarehouseRepository : IProduct_WarehouseRepository
{
    private readonly IConfiguration _configuration;
    public Product_WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<Product_Warehouse?> GetProduct_Warehouse(int idProduct, int idWarehouse)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();
        var query = "SELECT IdProduct, Name, Description, Price FROM s29143.Product_Warehouse WHERE " +
                    "idProduct = @IdProduct AND idWarehouse = @IdWarehouse";
        await using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("IdProduct", idProduct);
        command.Parameters.AddWithValue("IdWarehouse", idWarehouse);
        var reader = await command.ExecuteReaderAsync();
        reader.Read();
        return new Product_Warehouse
        {
            IdProduct = (int)reader["IdProduct"],
            IdWarehouse = (int)reader["IdWarehouse"],
            IdOrder = (int)reader["IdOrder"],
            IdProductWarehouse = (int)reader["IdProductWarehouse"],
            CreatedAt = (DateTime)reader["CreatedAt"]
        };
    }
}