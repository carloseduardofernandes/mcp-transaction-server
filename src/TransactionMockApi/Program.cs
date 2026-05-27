using TransactionMockApi.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var transactions = new Dictionary<string, Transaction>(StringComparer.OrdinalIgnoreCase)
{
    ["tx-001"] = new(
        Id: "tx-001",
        OrderId: "ORD-1001",
        Amount: 299.90m,
        Currency: "BRL",
        Status: "Completed",
        CreatedAt: new DateTime(2025, 2, 1, 10, 30, 0),
        PaymentMethod: "CreditCard",
        Description: "Pedido de eletrônicos"
    ),
    ["tx-002"] = new(
        Id: "tx-002",
        OrderId: "ORD-1002",
        Amount: 89.50m,
        Currency: "BRL",
        Status: "Pending",
        CreatedAt: new DateTime(2025, 2, 5, 14, 15, 0),
        PaymentMethod: "Pix",
        Description: "Assinatura mensal"
    ),
    ["tx-003"] = new(
        Id: "tx-003",
        OrderId: "ORD-1003",
        Amount: 1250.00m,
        Currency: "BRL",
        Status: "Completed",
        CreatedAt: new DateTime(2025, 2, 7, 09, 00, 0),
        PaymentMethod: "BankTransfer",
        Description: "Compra em loja parceira"
    ),
    ["tx-004"] = new(
        Id: "tx-004",
        OrderId: "ORD-1004",
        Amount: 45.99m,
        Currency: "BRL",
        Status: "Refunded",
        CreatedAt: new DateTime(2025, 2, 6, 16, 45, 0),
        PaymentMethod: "CreditCard",
        Description: "Devolução processada"
    ),
    ["tx-005"] = new(
        Id: "tx-005",
        OrderId: "ORD-1005",
        Amount: 520.00m,
        Currency: "BRL",
        Status: "Completed",
        CreatedAt: new DateTime(2025, 2, 8, 11, 20, 0),
        PaymentMethod: "Pix",
        Description: "Pedido marketplace"
    ),
};

app.MapGet("/api/transactions/{id}", (string id) =>
{
    if (transactions.TryGetValue(id, out var transaction))
        return Results.Ok(transaction);
    return Results.NotFound(new { error = $"Transação '{id}' não encontrada." });
});

app.MapGet("/", () => Results.Ok(new
{
    message = "Mock API de Transações",
    endpoint = "GET /api/transactions/{id}",
    exampleIds = new[] { "tx-001", "tx-002", "tx-003", "tx-004", "tx-005" }
}));

var port = Environment.GetEnvironmentVariable("API_PORT") ?? "5274";
app.Run($"http://localhost:{port}");
