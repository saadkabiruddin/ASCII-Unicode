namespace ASCII_Unicode.Services;

public sealed class UnicodeConverter
{
    private readonly ConversionMappingsLoader? _mappingsLoader;
    private readonly ILogger<UnicodeConverter>? _logger;

    public UnicodeConverter()
    {
        // Parameterless constructor for backward compatibility when not using DI
    }

    public UnicodeConverter(ConversionMappingsLoader mappingsLoader, ILogger<UnicodeConverter> logger)
    {
        _mappingsLoader = mappingsLoader;
        _logger = logger;
    }

    private static bool IsBanglaDigit(char c) => c >= '०' && c <= '९';

    private static bool IsBanglaPreKar(char c) => c is 'ি' or 'ৈ' or 'ে';

    private static bool IsBanglaPostKar(char c) => c is 'া' or 'ো' or 'ৌ' or 'ৗ' or 'ু' or 'ূ' or 'ী' or 'ৃ';

    private static bool IsBanglaKar(char c) => IsBanglaPreKar(c) || IsBanglaPostKar(c);

    private static bool IsBanglaBanjonborno(char c)
        => c is 'ক' or 'খ' or 'গ' or 'ঘ' or 'ঙ' or 'চ' or 'ছ' or 'জ' or 'ঝ' or 'ঞ' or 'ট' or 'ঠ' or 'ড' or 'ঢ' or 'ণ' or 'ত' or 'থ' or 'দ' or 'ধ' or 'ন' or 'প' or 'ফ' or 'ব' or 'ভ' or 'ম' or 'য' or 'র' or 'ল' or 'শ' or 'ষ' or 'স' or 'হ' or 'ড়' or 'ঢ়' or 'য়' or 'ৎ' or 'ং' or 'ঃ' or 'ঁ';

    private static bool IsBanglaSoroborno(char c)
        => c is 'অ' or 'আ' or 'ই' or 'ঈ' or 'উ' or 'ঊ' or 'ঋ' or 'ঌ' or 'এ' or 'ঐ' or 'ও' or 'ঔ';

    private static bool IsBanglaNukta(char c) => c == 'ঁ';

    private static bool IsBanglaHalant(char c) => c == '্';

    private static bool IsSpace(char c) => c is ' ' or '\t' or '\n' or '\r';

    public string ReArrangeUnicodeConvertedText(string value)
    {
        var text = value;

        var i = 0;
        while (i < ConverterUtil.MbStrLen(text))
        {
            if (i < ConverterUtil.MbStrLen(text) - 1
                && ConverterUtil.MbCharAt(text, i) == 'র'
                && IsBanglaHalant(ConverterUtil.MbCharAt(text, i + 1))
                && !IsBanglaHalant(ConverterUtil.MbCharAt(text, i - 1)))
            {
                var j = 1;
                while (true)
                {
                    if (i - j < 0)
                    {
                        break;
                    }

                    if (IsBanglaBanjonborno(ConverterUtil.MbCharAt(text, i - j))
                        && IsBanglaHalant(ConverterUtil.MbCharAt(text, i - j - 1)))
                    {
                        j += 2;
                    }
                    else if (j == 1 && IsBanglaKar(ConverterUtil.MbCharAt(text, i - j)))
                    {
                        j += 1;
                    }
                    else
                    {
                        break;
                    }
                }

                var temp = ConverterUtil.SubString(text, 0, i - j);
                temp += ConverterUtil.MbCharAt(text, i);
                temp += ConverterUtil.MbCharAt(text, i + 1);
                temp += ConverterUtil.SubString(text, i - j, i);
                temp += ConverterUtil.SubString(text, i + 2, ConverterUtil.MbStrLen(text));
                text = temp;
                i += 1;
                continue;
            }

            i += 1;
        }

        i = 0;
        while (i < ConverterUtil.MbStrLen(text))
        {
            if (i < ConverterUtil.MbStrLen(text) - 1
                && ConverterUtil.MbCharAt(text, i) == 'র'
                && IsBanglaHalant(ConverterUtil.MbCharAt(text, i + 1))
                && !IsBanglaHalant(ConverterUtil.MbCharAt(text, i - 1))
                && IsBanglaHalant(ConverterUtil.MbCharAt(text, i + 2)))
            {
                var j = 1;
                while (true)
                {
                    if (i - j < 0)
                    {
                        break;
                    }

                    if (IsBanglaBanjonborno(ConverterUtil.MbCharAt(text, i - j))
                        && IsBanglaHalant(ConverterUtil.MbCharAt(text, i - j - 1)))
                    {
                        j += 2;
                    }
                    else if (j == 1 && IsBanglaKar(ConverterUtil.MbCharAt(text, i - j)))
                    {
                        j += 1;
                    }
                    else
                    {
                        break;
                    }
                }

                var temp = ConverterUtil.SubString(text, 0, i - j);
                temp += ConverterUtil.MbCharAt(text, i);
                temp += ConverterUtil.MbCharAt(text, i + 1);
                temp += ConverterUtil.SubString(text, i - j, i);
                temp += ConverterUtil.SubString(text, i + 2, ConverterUtil.MbStrLen(text));
                text = temp;
                i += 1;
                continue;
            }

            if (i > 0
                && ConverterUtil.MbCharAt(text, i) == '\u09CD'
                && (IsBanglaKar(ConverterUtil.MbCharAt(text, i - 1)) || IsBanglaNukta(ConverterUtil.MbCharAt(text, i - 1)))
                && i < ConverterUtil.MbStrLen(text) - 1
                && IsBanglaBanjonborno(ConverterUtil.MbCharAt(text, i + 1)))
            {
                var temp = ConverterUtil.SubString(text, 0, i - 1);
                temp += ConverterUtil.MbCharAt(text, i);
                temp += ConverterUtil.MbCharAt(text, i + 1);
                temp += ConverterUtil.MbCharAt(text, i - 1);
                temp += ConverterUtil.SubString(text, i + 2, ConverterUtil.MbStrLen(text));
                text = temp;
            }

            if (i > 0
                && i < ConverterUtil.MbStrLen(text) - 1
                && ConverterUtil.MbCharAt(text, i) == '\u09CD'
                && ConverterUtil.MbCharAt(text, i - 1) == '\u09B0'
                && ConverterUtil.MbCharAt(text, i - 2) != '\u09CD'
                && IsBanglaKar(ConverterUtil.MbCharAt(text, i + 1)))
            {
                var temp = ConverterUtil.SubString(text, 0, i - 1);
                temp += ConverterUtil.MbCharAt(text, i + 1);
                temp += ConverterUtil.MbCharAt(text, i - 1);
                temp += ConverterUtil.MbCharAt(text, i);
                temp += ConverterUtil.SubString(text, i + 2, ConverterUtil.MbStrLen(text));
                text = temp;
            }

            if (i < ConverterUtil.MbStrLen(text) - 1
                && IsBanglaPreKar(ConverterUtil.MbCharAt(text, i))
                && !IsSpace(ConverterUtil.MbCharAt(text, i + 1)))
            {
                var temp = ConverterUtil.SubString(text, 0, i);

                var j = 1;
                while ((i + j) < ConverterUtil.MbStrLen(text) - 1
                       && IsBanglaBanjonborno(ConverterUtil.MbCharAt(text, i + j)))
                {
                    if ((i + j) < ConverterUtil.MbStrLen(text)
                        && IsBanglaHalant(ConverterUtil.MbCharAt(text, i + j + 1)))
                    {
                        j += 2;
                    }
                    else
                    {
                        break;
                    }
                }

                temp += ConverterUtil.SubString(text, i + 1, i + j + 1);

                var l = 0;
                if (ConverterUtil.MbCharAt(text, i) == 'ে' && ConverterUtil.MbCharAt(text, i + j + 1) == 'া')
                {
                    temp += "ো";
                    l = 1;
                }
                else if (ConverterUtil.MbCharAt(text, i) == 'ে' && ConverterUtil.MbCharAt(text, i + j + 1) == 'ৗ')
                {
                    temp += "ৌ";
                    l = 1;
                }
                else
                {
                    temp += ConverterUtil.MbCharAt(text, i);
                }

                temp += ConverterUtil.SubString(text, i + j + l + 1, ConverterUtil.MbStrLen(text));
                text = temp;
                i += j;
            }

            if (i < ConverterUtil.MbStrLen(text) - 1
                && IsBanglaNukta(ConverterUtil.MbCharAt(text, i))
                && IsBanglaPostKar(ConverterUtil.MbCharAt(text, i + 1)))
            {
                var temp = ConverterUtil.SubString(text, 0, i);
                temp += ConverterUtil.MbCharAt(text, i + 1);
                temp += ConverterUtil.MbCharAt(text, i);
                temp += ConverterUtil.SubString(text, i + 2, ConverterUtil.MbStrLen(text));
                text = temp;
            }

            i += 1;
        }

        return text;
    }

    public string ReArranceUnicodeTextForASCI(string value)
    {
        var text = value;
        var cY = 0;
        var i = 0;

        while (i < ConverterUtil.MbStrLen(text))
        {
            if (i < ConverterUtil.MbStrLen(text) && IsBanglaPreKar(ConverterUtil.MbCharAt(text, i)))
            {
                var j = 1;
                while (IsBanglaBanjonborno(ConverterUtil.MbCharAt(text, i - j)))
                {
                    if ((i - j) < 0)
                    {
                        break;
                    }

                    if ((i - j) <= cY)
                    {
                        break;
                    }

                    if (IsBanglaHalant(ConverterUtil.MbCharAt(text, i - j - 1)))
                    {
                        j += 2;
                    }
                    else
                    {
                        break;
                    }
                }

                var r = ConverterUtil.SubString(text, 0, i - j);
                r += ConverterUtil.MbCharAt(text, i);
                r += ConverterUtil.SubString(text, i - j, i);
                r += ConverterUtil.SubString(text, i + 1, ConverterUtil.MbStrLen(text));

                text = r;
                cY = i + 1;
                continue;
            }

            if (i < ConverterUtil.MbStrLen(text) - 1
                && IsBanglaHalant(ConverterUtil.MbCharAt(text, i))
                && ConverterUtil.MbCharAt(text, i - 1) == 'র'
                && !IsBanglaHalant(ConverterUtil.MbCharAt(text, i - 2)))
            {
                var j = 1;
                var aZ = 0;

                while (true)
                {
                    if (IsBanglaBanjonborno(ConverterUtil.MbCharAt(text, i + j))
                        && IsBanglaHalant(ConverterUtil.MbCharAt(text, i + j + 1)))
                    {
                        j += 2;
                    }
                    else if (IsBanglaBanjonborno(ConverterUtil.MbCharAt(text, i + j))
                             && IsBanglaPreKar(ConverterUtil.MbCharAt(text, i + j + 1)))
                    {
                        aZ = 1;
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

                var r = ConverterUtil.SubString(text, 0, i - 1);
                r += ConverterUtil.SubString(text, i + j + 1, i + j + aZ + 1);
                r += ConverterUtil.SubString(text, i + 1, i + j + 1);
                r += ConverterUtil.MbCharAt(text, i - 1);
                r += ConverterUtil.MbCharAt(text, i);
                r += ConverterUtil.SubString(text, i + j + aZ + 1, ConverterUtil.MbStrLen(text));

                text = r;
                i += j + aZ;
                cY = i + 1;
                continue;
            }

            i += 1;
        }

        return text;
    }

    public string ConvertUnicodeToBijoy(string srcString)
    {
        if (string.IsNullOrEmpty(srcString))
        {
            return srcString;
        }

        srcString = "কর্ড" + srcString;

        srcString = ConverterUtil.PregReplace("ো", "ো", srcString);
        srcString = ConverterUtil.PregReplace("ৌ", "ৌ", srcString);
        srcString = ConverterUtil.PregReplace("ড়", "ড়", srcString);
        srcString = ConverterUtil.PregReplace("য়", "য়", srcString);

        srcString = ReArranceUnicodeTextForASCI(srcString);

        // Use JSON-loaded mappings if available, otherwise fall back to hardcoded data
        var mainCharMap = _mappingsLoader != null
            ? ConversionMappingsLoader.ToKeyValuePairs(_mappingsLoader.LoadMappings().MainCharMap)
            : ConverterData.MainCharMap;

        srcString = ConverterUtil.DoCharMap(srcString, mainCharMap);

        if (srcString.EndsWith('©'))
        {
            srcString = srcString[..^1];
        }

        srcString = srcString.Replace("q‡", "‡q");
        srcString = srcString.Replace("o‡", "‡o");

        var chars = srcString.ToCharArray();
        var k = 0;
        while (k < chars.Length)
        {
            if (chars[k] == '©')
            {
                if (k + 1 < chars.Length)
                {
                    (chars[k], chars[k + 1]) = (chars[k + 1], chars[k]);
                }

                k += 2;
            }
            else
            {
                k += 1;
            }
        }

        var text = new string(chars);
        text = text.Length >= 2 ? text[2..] : string.Empty;
        return text;
    }
}
