﻿namespace APBD6.Models;

public class Product_Warehouse
{
    public int IdProductWarehouse { get; set; }
    public int IdWarehouse { get; set; }
    public int IdProduct { get; set; }
    public int IdOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}