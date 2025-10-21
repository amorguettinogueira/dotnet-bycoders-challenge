using FileHelpers;

namespace Bcp.Infrastructure.FileParsing.Converters;

public class TimeConverter : ConverterBase
{
    public override object StringToField(string from) =>
        TimeSpan.TryParseExact(from, "hhmmss", null, out var time) ? time : TimeSpan.Zero;
}
