using FileHelpers;

namespace Bcp.Infrastructure.FileParsing.Converters;

public class MoneyConverter : ConverterBase
{
    public override object StringToField(string from) =>
        decimal.TryParse(from, out var value) ? value / 100m : 0m;
}

