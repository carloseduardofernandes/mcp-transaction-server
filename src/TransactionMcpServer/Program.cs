using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using Microsoft.AspNetCore.Builder;

var baseUrl = Environment.GetEnvironmentVariable("TRANSACTION_API_BASE_URL") ?? "http://localhost:5273";
var transport = Environment.GetEnvironmentVariable("MCP_TRANSPORT") ?? "stdio";
var httpPort = Environment.GetEnvironmentVariable("MCP_HTTP_PORT") ?? "3000";

if (transport.Equals("http", StringComparison.OrdinalIgnoreCase))
{
    var webBuilder = WebApplication.CreateBuilder(args);

    webBuilder.Logging.AddConsole(consoleOptions =>
    {
        consoleOptions.LogToStandardErrorThreshold = LogLevel.Trace;
    });

    webBuilder.Services.AddHttpClient("TransactionApi", client =>
    {
        client.BaseAddress = new Uri(baseUrl.TrimEnd('/'));
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    });

    webBuilder.Services
        .AddMcpServer()
        .WithHttpTransport(options =>
        {
            options.Stateless = true;
        })
        .WithToolsFromAssembly();

    var app = webBuilder.Build();
    app.MapMcp();
    app.Run($"http://localhost:{httpPort}");
}
else
{
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
}
