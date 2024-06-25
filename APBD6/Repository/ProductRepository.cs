using System.Data.SqlClient;

namespace APBD6.Models;

public interface IProductRepository
{
    Task<Product?> GetProduct(int id);
}

public class ProductRepository : IProductRepository
{
    private readonly IConfiguration _configuration;
    public ProductRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<Product?> GetProduct(int id)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();
        const string query = "SELECT IdProduct, Name, Description, Price FROM s29143.Product WHERE id=@Id";
        await using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("Id", id);
        var reader = await command.ExecuteReaderAsync();
        reader.Read();
        return new Product
        {
            IdProduct = (int)reader["IdProduct"],
            Name = reader["Name"].ToString()!,
            Description = reader["Description"].ToString()!,
            Price = (double) reader["Price"]
        };
    }
}