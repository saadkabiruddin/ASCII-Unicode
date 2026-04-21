namespace ASCII_Unicode.Services;

public sealed class BijoyToUnicodeConverter
{
    public string ConvertBijoyToUnicode(string srcString)
    {
        if (string.IsNullOrEmpty(srcString))
        {
            return srcString;
        }

        srcString = ConverterUtil.DoCharMap(srcString, ConverterData.PreConversionMap);
        srcString = ConverterUtil.DoCharMap(srcString, ConverterData.ConversionMap);
        srcString = ConverterUtil.DoCharMap(srcString, ConverterData.ProConversionMap);
        srcString = new UnicodeConverter().ReArrangeUnicodeConvertedText(srcString);
        srcString = ConverterUtil.DoCharMap(srcString, ConverterData.PostConversionMap);

        return srcString;
    }
}
