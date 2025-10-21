using Bcp.Infrastructure.FileParsing.Converters;
using FileHelpers;

namespace Bcp.Infrastructure.Models;

[FixedLengthRecord(FixedMode.AllowLessChars)]
public class TransactionRecord
{
    [FieldFixedLength(1)]
    public int Type;

    [FieldFixedLength(8)]
    [FieldConverter(typeof(DateOnlyConverter), "yyyyMMdd")]
    public DateOnly Date;

    [FieldFixedLength(10)]
    [FieldConverter(typeof(MoneyConverter))]
    public decimal Value;

    [FieldFixedLength(11)]
    public required string CPF;

    [FieldFixedLength(12)]
    public required string Card;

    [FieldFixedLength(6)]
    [FieldConverter(typeof(TimeConverter))]
    public TimeSpan Time;

    [FieldFixedLength(14)]
    [FieldTrim(TrimMode.Both)]
    public required string StoreOwner;

    [FieldFixedLength(19)]
    [FieldTrim(TrimMode.Both)]
    public required string StoreName;
}
