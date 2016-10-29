// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Helper.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MichaelReukauff.Lexer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The field type.
    /// </summary>
    internal enum FieldType
    {
        /// <summary>
        /// The type unknown.
        /// </summary>
        TypeUnknown,

        /// <summary>
        /// The type double.
        /// </summary>
        TypeDouble,

        /// <summary>
        /// The type float.
        /// </summary>
        TypeFloat,

        /// <summary>
        /// The type uint 64.
        /// </summary>
        TypeUint64,

        /// <summary>
        /// The type fixed 64.
        /// </summary>
        TypeFixed64,

        /// <summary>
        /// The type fixed 32.
        /// </summary>
        TypeFixed32,

        /// <summary>
        /// The type bool.
        /// </summary>
        TypeBool,

        /// <summary>
        /// The type string.
        /// </summary>
        TypeString,

        /// <summary>
        /// The type group.
        /// </summary>
        TypeGroup,

        /// <summary>
        /// The type bytes.
        /// </summary>
        TypeBytes,

        /// <summary>
        /// The type uint32.
        /// </summary>
        TypeUint32,

        /// <summary>
        /// The type sfixed32.
        /// </summary>
        TypeSfixed32,

        /// <summary>
        /// The type sfixed64.
        /// </summary>
        TypeSfixed64,

        /// <summary>
        /// The type int32.
        /// </summary>
        TypeInt32,

        /// <summary>
        /// The type int64.
        /// </summary>
        TypeInt64,

        /// <summary>
        /// The type sint32.
        /// </summary>
        TypeSint32,

        /// <summary>
        /// The type sint64.
        /// </summary>
        TypeSint64,

        /// <summary>
        /// The type error.
        /// this one is only for unit tests and should not be used outside of unit tests.
        /// </summary>
        TypeError
    }

    /// <summary>
    /// The helper class.
    /// </summary>
    internal static class Helper
    {
        /// <summary>
        /// Split the text into tokens.
        /// <list type="number">
        /// <item>
        /// <description>whole words</description>
        /// </item>
        /// <item>
        /// <description>//</description>
        /// </item>
        /// <item>
        /// <description>/*</description>
        /// </item>
        /// <item>
        /// <description>*/</description>
        /// </item>
        /// <item>
        /// <description>{</description>
        /// </item>
        /// <item>
        /// <description>}</description>
        /// </item>
        /// <item>
        /// <description>=</description>
        /// </item>
        /// <item>
        /// <description>;</description>
        /// </item>
        /// <item>
        /// <description>[</description>
        /// </item>
        /// <item>
        /// <description>]</description>
        /// </item>
        /// <item>
        /// <description>(</description>
        /// </item>
        /// <item>
        /// <description>)</description>
        /// </item>
        /// <item>
        /// <description>.</description>
        /// </item>
        /// <item>
        /// <description>+</description>
        /// </item>
        /// <item>
        /// <description>,</description>
        /// </item>
        /// <item>
        /// <description>"</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="text">
        /// The text to split.
        /// </param>
        /// <returns>
        /// The <see cref="List{Match}"/>. The splitted text.
        /// </returns>
        internal static List<Match> SplitText(string text)
        {
            var regex = new Regex(
                //// "\\w+|//|/\\*|\\*/|\\{|\\}|=|;|\\[|\\]|\"|\\n|\\(|\\)|\\.",
                //// @"\w+|//|/\*|\*/|\{|\}|=|;|\[|\]|\n|\(|\)|\.|\""",
                //// @"\w+|//|/\*|\*/|\{|\}|=|;|\[|\]|\n|\(|\)|\.|([""'])(?:(?=(\\?))\2.)*?\1", // http://stackoverflow.com/questions/171480/regex-grabbing-values-between-quotation-marks
                @"(-\w+|\w+)|//|/\*|\*/|\{|\}|=|;|\[|\]|\n|\(|\)|\.|\+|,|<|>|\""",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

            // Capture all Matches in the InputText
            var ms = regex.Matches(text);

            Debug.WriteLine("Number of matches: " + ms.Count);

            ////foreach (Match m in ms)
            ////  Debug.WriteLine(m.Value);

            return ms.OfType<Match>().ToList();
        }

        /// <summary>
        /// Checks for an identifier.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <returns>The <see cref="bool"/>. True if it is an identifier.</returns>
        internal static bool IsIdentifier(string text)
        {
            var regex = new Regex(@"\b([A-Za-z_]{1}[a-zA-Z0-9_]*)\z", RegexOptions.Compiled);

            //// var res = regex.Matches(text); // only for tests

            return regex.IsMatch(text);
        }

        /// <summary>
        /// Checks for a field rule.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <returns>The <see cref="bool"/>. True if it is a field rule.</returns>
        internal static bool IsFieldRule(string text)
        {
            var list = new List<string> { "optional", "repeated", "required" };

            return list.Contains(text);
        }

        /// <summary>
        /// Get the field type.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <returns>The field type or unknown if not found.</returns>
        internal static FieldType GetFieldType(string text)
        {
            var list = new Dictionary<string, FieldType>
                           {
                               { "double", FieldType.TypeDouble },
                               { "float", FieldType.TypeFloat },
                               { "uint64", FieldType.TypeUint64 },
                               { "uint32", FieldType.TypeUint32 },
                               { "fixed64", FieldType.TypeFixed64 },
                               { "fixed32", FieldType.TypeFixed32 },
                               { "sfixed32", FieldType.TypeSfixed32 },
                               { "sfixed64", FieldType.TypeSfixed64 },
                               { "int32", FieldType.TypeInt32 },
                               { "int64", FieldType.TypeInt64 },
                               { "sint32", FieldType.TypeSint32 },
                               { "sint64", FieldType.TypeSint64 },
                               { "bool", FieldType.TypeBool },
                               { "string", FieldType.TypeString },
                               { "bytes", FieldType.TypeBytes },
                           };

            FieldType result;
            if (list.TryGetValue(text, out result))
            {
                return result;
            }

            return FieldType.TypeUnknown;
        }

        /// <summary>
        /// Check if the given text is a positive 32 bit integer.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <returns>True if it is a 32 bit positive integer otherwise false.</returns>
        internal static bool IsPositiveInteger(string text)
        {
            long result;

            if (!long.TryParse(text, NumberStyles.AllowLeadingSign, new CultureInfo("en-US"), out result))
            {
                if (!text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (!long.TryParse(text.Substring(2), NumberStyles.AllowHexSpecifier, new CultureInfo("en-US"), out result))
                {
                    return false;
                }
            }

            if (result < 0)
            {
                return false;
            }

            return result <= int.MaxValue;
        }

        /// <summary>
        /// Check if the given text is a positive 64 bit integer.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <returns>True if it is a 64 bit positive integer otherwise false.</returns>
        internal static bool IsPositive64Integer(string text)
        {
            long result;

            if (!long.TryParse(text, NumberStyles.AllowLeadingSign, new CultureInfo("en-US"), out result))
            {
                if (!text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (!long.TryParse(text.Substring(2), NumberStyles.AllowHexSpecifier, new CultureInfo("en-US"), out result))
                {
                    return false;
                }
            }

            return result >= 0;
        }

        /// <summary>
        /// Check if the given text is an integer (64 bit).
        /// Can be a decimal or an hexadecimal value.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <returns>True if it is an integer otherwise false.</returns>
        internal static bool IsInteger(string text)
        {
            long result;

            if (long.TryParse(text, NumberStyles.AllowLeadingSign, new CultureInfo("en-US"), out result))
            {
                return true;
            }

            if (!text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return long.TryParse(text.Substring(2), NumberStyles.AllowHexSpecifier, new CultureInfo("en-US"), out result);
        }

        /// <summary>
        /// Check if the given text is a double or float.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <returns>True if it is a double or float otherwise false.</returns>
        internal static bool IsDoubleOrFloat(string text)
        {
            double result;

            return double.TryParse(
                text,
                NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.Float,
                new CultureInfo("en-US"),
                out result);
        }

        /// <summary>
        /// Check if the text is true or false.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <returns>True if text is true or false otherwise false.</returns>
        internal static bool IsTrueOrFalse(string text)
        {
            if (text == "true")
            {
                return true;
            }

            return text == "false";
        }
    }
}
