namespace TransactionMockApi.Models;

public record Transaction(
    string Id,
    string OrderId,
    decimal Amount,
    string Currency,
    string Status,
    DateTime CreatedAt,
    string PaymentMethod,
    string? Description = null
);
