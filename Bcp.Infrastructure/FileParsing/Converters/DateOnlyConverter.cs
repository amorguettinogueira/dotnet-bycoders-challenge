using FileHelpers;
using System.Globalization;

namespace Bcp.Infrastructure.FileParsing.Converters;

/// <summary>
/// FileHelpers converter for DateOnly values.
/// Defaults to format "yyyyMMdd" but supports custom format via attribute args.
/// </summary>
public class DateOnlyConverter(string format) : ConverterBase
{
    private readonly string _format = string.IsNullOrWhiteSpace(format) ? "yyyyMMdd" : format;

    public DateOnlyConverter() : this("yyyyMMdd") { }

    public override object StringToField(string from)
    {
        if (string.IsNullOrWhiteSpace(from))
        {
            return default(DateOnly);
        }

        return DateOnly.TryParseExact(from.Trim(), _format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
            ? date
            : default;
    }

    public override string FieldToString(object? fieldValue) =>
        fieldValue is DateOnly d
            ? d.ToString(_format, CultureInfo.InvariantCulture)
            : string.Empty;
}
