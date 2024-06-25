using System.Data.SqlClient;
using APBD6.Models;

namespace APBD6.Order;


public interface IOrderRepository
{
    Task<Order?> GetOrder(int idProduct, int amount);
    Task<bool> UpdateOrder(Order order);
}

public class OrderRepository : IOrderRepository
{
    private readonly IConfiguration _configuration;
    public OrderRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task<Order?> GetOrder(int idProduct, int amount)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();
        var query = "SELECT IdProduct, Amount, IdOrder, CreatedAt, FulfilledAt FROM s29143.Order " +
                    " WHERE idProduct=@IdProduct AND Amount=@Amount";
        await using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("IdProduct", idProduct);
        command.Parameters.AddWithValue("Amount ", amount);
        var reader = await command.ExecuteReaderAsync();
        reader.Read();
        return new Order
        {
            IdProduct = (int)reader["IdProduct"],
            Amount = (int)reader["Amount"],
            IdOrder = (int)reader["IdOrder"],
            CreatedAt = (DateTime) reader["CreatedAt"],
            FulfilledAt = (DateTime) reader["FulfilledAt"],
        };
    }

    public async Task<bool> UpdateOrder(Order order)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();
        var query = "SELECT IdProduct, Name, Description, Price FROM s29143.Order WHERE IdOrder = @IdOrder";
        await using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("IdOrder", order.IdOrder);
        var reader = await command.ExecuteNonQueryAsync();
        return reader == 1;
    }
}