﻿using System.Globalization;
using System.Text;

namespace Umbraco.Common.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Creates a URL And SEO friendly slug
    /// </summary>
    /// <param name="text">Text to slugify</param>
    /// <param name="maxLength">Max length of slug</param>
    /// <returns>URL and SEO friendly string</returns>
    public static string ToSlugFriendly(this string text, int maxLength = 0)
    {
        // Return empty value if text is null
        if (text.IsNullOrWhiteSpace()) 
            return string.Empty;

        var normalizedString = text
            .ToLowerInvariant()
            .Normalize(NormalizationForm.FormD);

        var stringBuilder = new StringBuilder();
        var stringLength = normalizedString.Length;
        var prevdash = false;
        var trueLength = 0;

        for (var i = 0; i < stringLength; i++)
        {
            var c = normalizedString[i];

            switch (CharUnicodeInfo.GetUnicodeCategory(c))
            {
                // Check if the character is a letter or a digit if the character is a
                // international character remap it to an ascii valid character
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.DecimalDigitNumber:
                    if (c < 128)
                        stringBuilder.Append(c);
                    else
                        stringBuilder.Append(c.CharToAscii());

                    prevdash = false;
                    trueLength = stringBuilder.Length;
                    break;

                // Check if the character is to be replaced by a hyphen but only if the last character wasn't
                case UnicodeCategory.SpaceSeparator:
                case UnicodeCategory.ConnectorPunctuation:
                case UnicodeCategory.DashPunctuation:
                case UnicodeCategory.OtherPunctuation:
                case UnicodeCategory.MathSymbol:
                    if (!prevdash)
                    {
                        stringBuilder.Append('-');
                        prevdash = true;
                        trueLength = stringBuilder.Length;
                    }
                    break;
            }

            // If we are at max length, stop parsing
            if (maxLength > 0 && trueLength >= maxLength)
                break;
        }

        // Trim excess hyphens
        var result = stringBuilder.ToString().Trim('-');

        // Remove any excess character to meet maxlength criteria
        return maxLength <= 0 || result.Length <= maxLength ? result : result[..maxLength];
    }
}