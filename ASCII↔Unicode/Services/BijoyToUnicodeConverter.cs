namespace ASCII_Unicode.Services;

public sealed class BijoyToUnicodeConverter
{
    private readonly ConversionMappingsLoader _mappingsLoader;
    private readonly ILogger<BijoyToUnicodeConverter> _logger;

    public BijoyToUnicodeConverter(ConversionMappingsLoader mappingsLoader, ILogger<BijoyToUnicodeConverter> logger)
    {
        _mappingsLoader = mappingsLoader;
        _logger = logger;
    }

    public string ConvertBijoyToUnicode(string srcString)
    {
        if (string.IsNullOrEmpty(srcString))
        {
            return srcString;
        }

        try
        {
            var mappings = _mappingsLoader.LoadMappings();

            srcString = ConverterUtil.DoCharMap(
                srcString,
                ConversionMappingsLoader.ToKeyValuePairs(mappings.PreConversionMap));

            srcString = ConverterUtil.DoCharMap(
                srcString,
                ConversionMappingsLoader.ToKeyValuePairs(mappings.ConversionMap));

            srcString = ConverterUtil.DoCharMap(
                srcString,
                ConversionMappingsLoader.ToKeyValuePairs(mappings.ProConversionMap));

            srcString = new UnicodeConverter().ReArrangeUnicodeConvertedText(srcString);

            srcString = ConverterUtil.DoCharMap(
                srcString,
                ConversionMappingsLoader.ToKeyValuePairs(mappings.PostConversionMap));

            return srcString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Bijoy to Unicode conversion");
            throw;
        }
    }
}
