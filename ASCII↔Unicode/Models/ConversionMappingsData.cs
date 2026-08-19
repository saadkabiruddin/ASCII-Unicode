namespace ASCII_Unicode.Models;

/// <summary>
/// Root model for conversion-mappings.json
/// </summary>
public class ConversionMappingsData
{
    public string? Description { get; set; }
    public List<ConversionMapping> PreConversionMap { get; set; } = [];
    public List<ConversionMapping> ConversionMap { get; set; } = [];
    public List<ConversionMapping> ProConversionMap { get; set; } = [];
    public List<ConversionMapping> PostConversionMap { get; set; } = [];
    public List<ConversionMapping> MainCharMap { get; set; } = [];
}
