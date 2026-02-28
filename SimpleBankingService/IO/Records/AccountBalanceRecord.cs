using CsvHelper.Configuration.Attributes;

namespace SimpleBankingService.IO.Records;

/// <summary>
/// Raw account balance record used for reading from / writing to CSV.
/// </summary>
public record AccountBalanceRecord
{
    /// <summary>
    /// Raw account number string. Must be 16 digits or fewer.
    /// </summary>
    [Index(0)]
    public required string AccountNumber { get; set; }

    /// <summary>
    /// Balance of account. Must be >= 0.
    /// </summary>
    [Index(1)]
    public required decimal Balance { get; set; }
}
