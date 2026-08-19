using System.Text.Json;
using ASCII_Unicode.Models;

namespace ASCII_Unicode.Services;

/// <summary>
/// Loads conversion mappings from JSON file
/// </summary>
public sealed class ConversionMappingsLoader
{
    private readonly string _filePath;
    private readonly ILogger<ConversionMappingsLoader> _logger;
    private ConversionMappingsData? _cachedMappings;

    public ConversionMappingsLoader(IWebHostEnvironment environment, ILogger<ConversionMappingsLoader> logger)
    {
        _logger = logger;
        _filePath = Path.Combine(environment.ContentRootPath, "conversion-mappings.json");
    }

    /// <summary>
    /// Loads mappings from JSON file
    /// </summary>
    public ConversionMappingsData LoadMappings()
    {
        try
        {
            if (_cachedMappings != null)
            {
                _logger.LogInformation("Returning cached conversion mappings");
                return _cachedMappings;
            }

            if (!File.Exists(_filePath))
            {
                _logger.LogWarning("Conversion mappings file not found at {FilePath}. Using empty mappings.", _filePath);
                return new ConversionMappingsData();
            }

            var json = File.ReadAllText(_filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var mappings = JsonSerializer.Deserialize<ConversionMappingsData>(json, options);

            if (mappings == null)
            {
                _logger.LogWarning("Failed to deserialize conversion mappings. JSON file may be empty or invalid.");
                return new ConversionMappingsData();
            }

            _cachedMappings = mappings;
            _logger.LogInformation(
                "Loaded conversion mappings from {FilePath}. " +
                "PreConversion: {PreCount}, Main: {MainCount}, Post: {PostCount}",
                _filePath,
                mappings.PreConversionMap.Count,
                mappings.ConversionMap.Count,
                mappings.PostConversionMap.Count
            );

            return mappings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading conversion mappings from {FilePath}", _filePath);
            throw;
        }
    }

    /// <summary>
    /// Reloads mappings from file (clears cache)
    /// </summary>
    public ConversionMappingsData ReloadMappings()
    {
        _logger.LogInformation("Reloading conversion mappings from {FilePath}", _filePath);
        _cachedMappings = null;
        return LoadMappings();
    }

    /// <summary>
    /// Converts loaded mappings to KeyValuePair format used by existing code
    /// </summary>
    public static IReadOnlyList<KeyValuePair<string, string>> ToKeyValuePairs(List<ConversionMapping> mappings)
    {
        return mappings
            .Select(m => new KeyValuePair<string, string>(m.From, m.To))
            .ToList()
            .AsReadOnly();
    }
}
