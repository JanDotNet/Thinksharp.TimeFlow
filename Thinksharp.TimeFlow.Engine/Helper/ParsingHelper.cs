using System;
using System.Globalization;
namespace Thinksharp.TimeFlow.Engine.Helper
{
  internal enum ParsingResultType { Succeeded, ErrorTypeNotSupported, ErrorInvalidFormat }
  internal class ParsingResult
  { 
    public ParsingResult(object value)
    {
      this.Value = value;
      this.Type = ParsingResultType.Succeeded;
      this.ErrorMessage = string.Empty;
    }

    public ParsingResult(ParsingResultType type, string error)
    { 
      this.Value = null;
      this.Type = type;
      this.ErrorMessage = error;
    }

    public object Value { get; }
    public ParsingResultType Type { get; }
    public string ErrorMessage { get; }

    public object GetValueOrThrowError()
    {
      if (this.Type == ParsingResultType.ErrorInvalidFormat)
      {
        throw new FormatException(this.ErrorMessage);
      }
      else if (this.Type == ParsingResultType.ErrorTypeNotSupported)
      {
        throw new FormatException(this.ErrorMessage);
      }

      return this.Value;
    }
  }

  internal static class ParsingHelper
  {
    public static ParsingResult Parse(this string str, Type type)
    {
      if (type == typeof(string))
      {
        return new ParsingResult(str);
      }

      // handel nullable types
      var nullableUnderyingType = Nullable.GetUnderlyingType(type);
      if (nullableUnderyingType != null)
      {
        if (string.IsNullOrEmpty(str) || str.Equals("null", StringComparison.InvariantCultureIgnoreCase))
        {
          return new ParsingResult(null);
        }

        type = nullableUnderyingType;
      }

      if (type.IsEnum)
      {
        try
        {
          var enumValue = Enum.Parse(type, str, true);
          if (enumValue != null)
          {
            return new ParsingResult(enumValue);
          }
        }
        catch
        {
          // handled below.
        }
      } 
      else if (type == typeof(int))
      {
        if (int.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
        {
          return new ParsingResult(value);
        }
      }
      else if (type == typeof(long))
      {
        if (long.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
        {
          return new ParsingResult(value);
        }
      }
      else if (type == typeof(double))
      {
        if (double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
        {
          return new ParsingResult(value);
        }
      }
      else if (type == typeof(decimal))
      {
        if (decimal.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
        {
          return new ParsingResult(value);
        }
      }
      else if (type == typeof(DateTimeOffset))
      {
        if (DateTimeOffset.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out var value))
        {
          return new ParsingResult(value);
        }
      }
      else if (type == typeof(DateTime))
      {
        if (DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out var value))
        {
          return new ParsingResult(value);
        }
      }
      else if (type == typeof(Period))
      {
        if (Period.TryParse(str, out var value))
        {
          return new ParsingResult(value);
        }
      }
      else
      {
        return new ParsingResult(ParsingResultType.ErrorTypeNotSupported, $"Parsing type '{type.Name}' is not supported yet.");
      }

      return new ParsingResult(ParsingResultType.ErrorInvalidFormat, $"Unable to parse '{str}' to type '{type.Name}'.");
    }
  }
}
