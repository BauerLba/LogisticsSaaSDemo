namespace LogisticsSaaS.Core.Domain.Entities;

public class Customer
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int ActiveShipments { get; set; }
    public decimal TotalSpent { get; set; }
    public string Icon { get; set; } = "fa-user";
    public string Color { get; set; } = "#6366f1";
}
