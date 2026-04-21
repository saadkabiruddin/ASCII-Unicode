using System.Text.RegularExpressions;

namespace ASCII_Unicode.Services;

internal static class ConverterUtil
{
    public static string DoCharMap(string text, IReadOnlyList<KeyValuePair<string, string>> charMap)
    {
        foreach (var entry in charMap)
        {
            text = PregReplace(entry.Key, entry.Value, text);
        }

        return text;
    }

    public static int MbStrLen(string value) => value.Length;

    public static char MbCharAt(string value, int index)
    {
        var normalized = index;
        if (normalized < 0)
        {
            normalized = value.Length + normalized;
        }

        return normalized >= 0 && normalized < value.Length ? value[normalized] : '\0';
    }

    public static string SubString(string value, int from, int to)
    {
        var start = from < 0 ? value.Length + from : from;
        var end = to < 0 ? value.Length + to : to;

        if (start < 0)
        {
            start = 0;
        }

        if (end < 0)
        {
            end = 0;
        }

        if (start > value.Length)
        {
            start = value.Length;
        }

        if (end > value.Length)
        {
            end = value.Length;
        }

        if (end <= start)
        {
            return string.Empty;
        }

        return value[start..end];
    }

    public static string PregReplace(string pattern, string replacement, string text)
        => Regex.Replace(text, pattern, replacement);
}
