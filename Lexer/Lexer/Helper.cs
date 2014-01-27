#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Helper.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Lexer
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Globalization;
  using System.Linq;
  using System.Text.RegularExpressions;

  internal enum FieldType
  {
    type_unknown,
    type_double,
    type_float,
    type_uint64,
    type_fixed64,
    type_fixed32,
    type_bool,
    type_string,
    type_group,
    type_bytes,
    type_uint32,
    type_sfixed32,
    type_sfixed64,
    type_int32,
    type_int64,
    type_sint32,
    type_sint64,
    type_error,  // this one is only for unit tests and should not be used outside of unit tests

  }

  internal static class Helper
  {
    /// <summary>
    /// Split the text into tokens
    /// 1. whole words
    /// 2. //
    /// 3. /*
    /// 4. */
    /// 5. {
    /// 6. }
    /// 7. =
    /// 8. ;
    /// 9. [
    /// 10. ]
    /// 11. (
    /// 12. )
    /// 13. .
    /// 14. +
    /// 15. ,
    /// 16. "
    /// </summary>
    internal static List<Match> SplitText(string text)
    {
      Regex regex = new Regex(
        // "\\w+|//|/\\*|\\*/|\\{|\\}|=|;|\\[|\\]|\"|\\n|\\(|\\)|\\.",
        // @"\w+|//|/\*|\*/|\{|\}|=|;|\[|\]|\n|\(|\)|\.|\""",
        // @"\w+|//|/\*|\*/|\{|\}|=|;|\[|\]|\n|\(|\)|\.|([""'])(?:(?=(\\?))\2.)*?\1", // http://stackoverflow.com/questions/171480/regex-grabbing-values-between-quotation-marks
        @"(-\w+|\w+)|//|/\*|\*/|\{|\}|=|;|\[|\]|\n|\(|\)|\.|\+|,|\""",
        RegexOptions.IgnoreCase
        | RegexOptions.CultureInvariant
        | RegexOptions.IgnorePatternWhitespace
        | RegexOptions.Compiled
        );

      // Capture all Matches in the InputText
      var ms = regex.Matches(text);

      Debug.WriteLine("Number of matches: " + ms.Count);

      //foreach (Match m in ms)
      //  Debug.WriteLine(m.Value);

      return ms.OfType<Match>().ToList();
    }

    internal static bool IsIdentifier(string text)
    {
      Regex regex = new Regex(@"\b([A-Za-z_]{1}[a-zA-Z0-9_]*)\z", RegexOptions.Compiled);

      //// var res = regex.Matches(text); // only for tests

      return regex.IsMatch(text);
    }

    internal static bool IsFieldRule(string text)
    {
      List<string> list = new List<string>
      {
        "optional", "repeated", "required"
      };

      return list.Contains(text);
    }

    /// <summary>
    /// Get the field type
    /// </summary>
    /// <param name="text">The text to check</param>
    /// <returns>The field type or unknown if not found</returns>
    internal static FieldType GetFieldType(string text)
    {
      Dictionary<string, FieldType> list = new Dictionary<string, FieldType>
      {
        {"double", FieldType.type_double},
        {"float", FieldType.type_float},
        {"uint64", FieldType.type_uint64},
        {"uint32", FieldType.type_uint32},
        {"fixed64", FieldType.type_fixed64},
        {"fixed32", FieldType.type_fixed32},
        {"sfixed32", FieldType.type_sfixed32},
        {"sfixed64", FieldType.type_sfixed64},
        {"int32", FieldType.type_int32},
        {"int64", FieldType.type_int64},
        {"sint32", FieldType.type_sint32},
        {"sint64", FieldType.type_sint64},
        {"bool", FieldType.type_bool},
        {"string", FieldType.type_string},
        {"bytes", FieldType.type_bytes},
      };

      FieldType result;
      if (list.TryGetValue(text, out result))
        return result;

      return FieldType.type_unknown;
    }

    /// <summary>
    /// check if the given text is a positive 32 bit integer
    /// </summary>
    /// <param name="text">The text to check</param>
    /// <returns>True if it is a 32 bit positive integer otherwise false</returns>
    internal static bool IsPositiveInteger(string text)
    {
      long result;

      if (!Int64.TryParse(text, NumberStyles.AllowLeadingSign, new CultureInfo("en-US"), out result))
        return false;

      if (result < 0)
        return false;

      return result <= Int32.MaxValue;
    }


    /// <summary>
    /// check if the given text is a positive 64 bit integer
    /// </summary>
    /// <param name="text">The text to check</param>
    /// <returns>True if it is a 64 bit positive integer otherwise false</returns>
    internal static bool IsPositive64Integer(string text)
    {
      long result;

      if (!Int64.TryParse(text, NumberStyles.AllowLeadingSign, new CultureInfo("en-US"), out result))
        return false;

      return result >= 0;
    }

    /// <summary>
    /// check if the given text is an integer (64 bit)
    /// </summary>
    /// <param name="text">The text to check</param>
    /// <returns>True if it is an integer otherwise false</returns>
    internal static bool IsInteger(string text)
    {
      long result;

      return Int64.TryParse(text, NumberStyles.AllowLeadingSign, new CultureInfo("en-US"), out result);
    }

    /// <summary>
    /// check if the given text is a double or float
    /// </summary>
    /// <param name="text">The text to check</param>
    /// <returns>True if it is a double or float otherwise false</returns>
    internal static bool IsDoubleOrFloat(string text)
    {
      double result;

      return Double.TryParse(text,
        NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.Float,
        new CultureInfo("en-US"),
        out result);
    }

    /// <summary>
    /// check if the text is true or false
    /// </summary>
    /// <param name="text">The text to check</param>
    /// <returns>True if text is true or false otherwise false</returns>
    internal static bool IsTrueOrFalse(string text)
    {
      if (text == "true")
        return true;

      return text == "false";
    }
  }
}
