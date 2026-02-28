using CsvHelper.Configuration.Attributes;

namespace SimpleBankingService.IO.Records;

/// <summary>
/// Raw transaction record used for reading from / writing to CSV.
/// </summary>
public record TransactionRecord
{
    /// <summary>
    /// Raw originating account number string. Must be 16 digits or fewer.
    /// </summary>
    [Index(0)]
    public required string FromAccount { get; set; }

    /// <summary>
    /// Raw destination account number string. Must be 16 digits or fewer.
    /// </summary>
    [Index(1)]
    public required string ToAccount { get; set; }

    /// <summary>
    /// Transaction amount. Must be >= 0.
    /// </summary>
    [Index(2)]
    public required decimal Amount { get; init; }
}
