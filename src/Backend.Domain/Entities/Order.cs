﻿namespace Backend.Domain.Entities;

public class Order
{
    public string? OrderId { get; set; }
    public string? CustomerName { get; set; }
    public decimal Amount { get; set; }
}
