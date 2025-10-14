using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace IchigoHoshimiya.BackgroundServices;

file record GraphQlResponse([property: JsonPropertyName("data")] DataPayload Data);

// You just gotta live with these warnings
file record DataPayload(
    [property: JsonPropertyName("dumpPaginator")]
    DumpPaginator Paginator);

file record DumpPaginator([property: JsonPropertyName("data")] List<DumpEntry> Data);

file record DumpEntry([property: JsonPropertyName("link")] string Link);

public class AnimeThemesUpdaterSettings
{
    public string GraphQlEndpoint { get; init; } = string.Empty;
}

public class AnimeThemesDbUpdateService(
    ILogger<AnimeThemesDbUpdateService> logger,
    HttpClient httpClient,
    IOptions<AnimeThemesUpdaterSettings> settings,
    IConfiguration configuration)
    : BackgroundService
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    private readonly AnimeThemesUpdaterSettings _settings = settings.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("AnimeThemes Update Service is starting.");

#if RELEASE
        await RunUpdateAsync(stoppingToken);
#endif
        while (!stoppingToken.IsCancellationRequested)
        {
            // Calculate delay until the next 00:00 UTC
            DateTime nowUtc = DateTime.UtcNow;
            DateTime nextRunTime = nowUtc.Date.AddDays(1); // Midnight tomorrow
            TimeSpan delay = nextRunTime - nowUtc;

            logger.LogInformation("Next database update scheduled for: {NextRunTime} UTC.", nextRunTime);

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break; // Service is stopping
            }

            await RunUpdateAsync(stoppingToken);
        }
    }

    private async Task RunUpdateAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation("Starting database update...");

            var dumpUrl = await GetDumpUrlAsync(stoppingToken);

            if (string.IsNullOrEmpty(dumpUrl))
            {
                logger.LogWarning("Could not retrieve SQL dump URL. Skipping update.");

                return;
            }

            await ExecuteSqlScriptFromUrlAsync(dumpUrl, stoppingToken);

            logger.LogInformation("Database update completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during the database update.");
        }
    }

    private async Task<string?> GetDumpUrlAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Querying GraphQL endpoint for dump URL...");

        var queryText = await File.ReadAllTextAsync("Queries/Animethemes/DumpQuery.graphql", cancellationToken);

        var query = new { query = queryText };

        var content = new StringContent(JsonSerializer.Serialize(query), Encoding.UTF8, "application/json");

        HttpResponseMessage response = await httpClient.PostAsync(
            _settings.GraphQlEndpoint,
            content,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var gqlResponse = await response.Content.ReadFromJsonAsync<GraphQlResponse>(cancellationToken);
        var link = gqlResponse?.Data.Paginator.Data.FirstOrDefault()?.Link;

        logger.LogInformation("Found dump URL: {Link}", link);

        return link;
    }

    private async Task ExecuteSqlScriptFromUrlAsync(string url, CancellationToken cancellationToken)
    {
        logger.LogInformation("Applying SQL dump directly from URL: {Url}", url);

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using Stream stream = await httpClient.GetStreamAsync(url, cancellationToken);
        using var reader = new StreamReader(stream);

        var scriptBuilder = new StringBuilder();

        while (await reader.ReadLineAsync(cancellationToken) is { } line)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("--"))
            {
                continue; // skip comments and blank lines
            }

            scriptBuilder.AppendLine(line);

            // Execute when a statement ends
            if (!line.TrimEnd().EndsWith(';'))
            {
                continue;
            }

            var sqlStatement = scriptBuilder.ToString();
            scriptBuilder.Clear();

            await using var cmd = new MySqlCommand(sqlStatement, connection);
            cmd.CommandTimeout = 0;
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}