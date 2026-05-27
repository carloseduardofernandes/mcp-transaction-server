using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

var baseUrl = Environment.GetEnvironmentVariable("TRANSACTION_API_BASE_URL") ?? "http://localhost:5273";

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole(consoleOptions =>
{
    consoleOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

builder.Services.AddHttpClient("TransactionApi", client =>
{
    client.BaseAddress = new Uri(baseUrl.TrimEnd('/'));
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();
