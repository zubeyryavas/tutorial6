﻿using System.Data;
using System.Data.SqlClient;

namespace APBD6.Warehouse;

public interface IWarehouseRepository
{
    Task<Warehouse?> GetWarehouse(int id);
    Task<int?> RegisterProduct(int idWarehouse, int idProduct, int idOrder, DateTime createdAt);
}

public class WarehouseRepository : IWarehouseRepository
{
    private readonly IConfiguration _configuration;
    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<Warehouse?> GetWarehouse(int id)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();
        var query = "SELECT IdWarehouse, Name, Address FROM s29143.Warehouse WHERE id=@Id";
        await using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("Id", id);
        var reader = await command.ExecuteReaderAsync();
        reader.Read();
        return new Warehouse
        {
            IdWarehouse = (int)reader["IdWarehouse"],
            Name = reader["Name"].ToString()!,
            Address = reader["Address"].ToString()!,
        };
    }

    public async Task<int?> RegisterProduct(int idWarehouse, int idProduct, int idOrder, DateTime createdAt)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();
        
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var query = "UPDATE \"s29143.Order\" SET FulfilledAt = @FulfilledAt WHERE IdOrder = @IdOrder";
            await using var command = new SqlCommand(query, connection);
            command.Transaction = (SqlTransaction)transaction;
            command.Parameters.AddWithValue("@IdOrder", idOrder);
            command.Parameters.AddWithValue("@FulfilledAt", DateTime.UtcNow);
            await command.ExecuteNonQueryAsync();
            
            command.CommandText = @"
                      INSERT INTO s29143.Product_Warehouse (IdWarehouse, IdProduct, IdOrder, CreatedAt, Amount, Price)
                      OUTPUT Inserted.IdProductWarehouse
                      VALUES (@IdWarehouse, @IdProduct, @IdOrder, @CreatedAt, 0, 0);";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@IdWarehouse", idWarehouse);
            command.Parameters.AddWithValue("@IdProduct", idProduct);
            command.Parameters.AddWithValue("@IdOrder", idOrder);
            command.Parameters.AddWithValue("@CreatedAt", createdAt);
            var idProductWarehouse = (int)await command.ExecuteScalarAsync();

            await transaction.CommitAsync();
            return idProductWarehouse;
        }
        catch
        {
            await transaction.RollbackAsync();
            return null;
        }
    }
    
    public async Task RegisterProductInWarehouseByProcedureAsync(int idWarehouse, int idProduct, DateTime createdAt)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();
        await using var command = new SqlCommand("AddProductToWarehouse", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("IdProduct", idProduct);
        command.Parameters.AddWithValue("IdWarehouse",idWarehouse);
        command.Parameters.AddWithValue("Amount", 0);
        command.Parameters.AddWithValue("CreatedAt", createdAt);
        await command.ExecuteNonQueryAsync();
    }
}