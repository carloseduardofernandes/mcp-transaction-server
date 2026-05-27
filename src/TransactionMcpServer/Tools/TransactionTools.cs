using System.ComponentModel;
using System.Net.Http.Json;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace TransactionMcpServer.Tools;

[McpServerToolType]
public sealed class TransactionTools
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TransactionTools> _logger;

    public TransactionTools(IHttpClientFactory httpClientFactory, ILogger<TransactionTools> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [McpServerTool(
        Name = "get_transaction_details",
        Title = "Detalhes da Transação",
        ReadOnly = true,
        Idempotent = true)]
    [Description("Retorna os dados da transação relacionada a um pedido. Informe o ID da transação (ex: tx-001, tx-002).")]
    public async Task<string> GetTransactionDetails(
        [Description("ID da transação (ex: tx-001, tx-002, tx-003, tx-004, tx-005)")] string transactionId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(transactionId))
            throw new McpException("O ID da transação é obrigatório.");

        _logger.LogInformation("Buscando transação {TransactionId}", transactionId);

        var client = _httpClientFactory.CreateClient("TransactionApi");
        var response = await client.GetAsync($"/api/transactions/{Uri.EscapeDataString(transactionId.Trim())}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return $"Transação '{transactionId}' não encontrada.";
            throw new McpException($"Erro ao consultar transação: {response.StatusCode}");
        }

        var transaction = await response.Content.ReadFromJsonAsync<TransactionDto>(cancellationToken);
        if (transaction == null)
            return "Resposta inválida da API de transações.";

        return $"ID: {transaction.Id}\n" +
               $"Pedido: {transaction.OrderId}\n" +
               $"Valor: {transaction.Amount:N2} {transaction.Currency}\n" +
               $"Status: {transaction.Status}\n" +
               $"Data: {transaction.CreatedAt:dd/MM/yyyy HH:mm}\n" +
               $"Pagamento: {transaction.PaymentMethod}\n" +
               (string.IsNullOrEmpty(transaction.Description) ? "" : $"Descrição: {transaction.Description}");
    }

    private sealed record TransactionDto(
        string Id,
        string OrderId,
        decimal Amount,
        string Currency,
        string Status,
        DateTime CreatedAt,
        string PaymentMethod,
        string? Description
    );
}
