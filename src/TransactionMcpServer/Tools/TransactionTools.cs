using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
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
    public async Task<TransactionResult> GetTransactionDetails(
        [Description("ID da transação (ex: tx-001, tx-002, tx-003, tx-004, tx-005)")] string transactionId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(transactionId))
            throw new McpException("O ID da transação é obrigatório.");

        _logger.LogInformation("Buscando transação {TransactionId}", transactionId);

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var client = _httpClientFactory.CreateClient("TransactionApi");
            var response = await client.GetAsync($"/api/transactions/{Uri.EscapeDataString(transactionId.Trim())}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Transação {TransactionId} não encontrada (404)", transactionId);
                    return new TransactionResult(transactionId, null, null, null, null, null, null, null, "not_found");
                }

                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Erro ao consultar transação {TransactionId}: {StatusCode} - {Body}",
                    transactionId, response.StatusCode, errorBody);
                throw new McpException($"Erro ao consultar transação: {response.StatusCode}");
            }

            var dto = await response.Content.ReadFromJsonAsync<TransactionDto>(cancellationToken);
            if (dto == null)
            {
                _logger.LogError("Resposta da transação {TransactionId} desserializou para null", transactionId);
                return new TransactionResult(transactionId, null, null, null, null, null, null, null, "invalid_response");
            }

            _logger.LogInformation("Transação {TransactionId} encontrada com status {Status}", dto.Id, dto.Status);

            return new TransactionResult(
                dto.Id,
                dto.OrderId,
                dto.Amount,
                dto.Currency,
                dto.Status,
                dto.CreatedAt,
                dto.PaymentMethod,
                dto.Description,
                "ok"
            );
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Consulta à transação {TransactionId} foi cancelada", transactionId);
            throw;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Falha de conexão ao consultar transação {TransactionId}", transactionId);
            throw new McpException($"Falha de conexão com a API de transações. Verifique se a API está rodando.", ex);
        }
        catch (McpException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao consultar transação {TransactionId}", transactionId);
            throw new McpException($"Erro inesperado ao consultar transação: {ex.Message}", ex);
        }
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

public sealed record TransactionResult(
    string TransactionId,
    string? OrderId,
    decimal? Amount,
    string? Currency,
    string? Status,
    DateTime? CreatedAt,
    string? PaymentMethod,
    string? Description,
    [property: JsonPropertyName("result")]
    string Result
);
