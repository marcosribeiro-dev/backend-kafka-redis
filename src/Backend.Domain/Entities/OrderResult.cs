namespace Backend.Domain.Entities;

public class OrderResult
{
    public string? OrderId { get; set; }
    public string? CustomerName { get; set; }
    public decimal Amount { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
}
