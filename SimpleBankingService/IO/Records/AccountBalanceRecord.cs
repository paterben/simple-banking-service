using CsvHelper.Configuration.Attributes;

namespace SimpleBankingService.IO.Records;

// Raw account balance record used for reading from / writing to CSV.
public record AccountBalanceRecord
{
    // Account number. Must be 16 digits or less.
    [Index(0)]
    public required string AccountNumber { get; set; }

    // Balance of account. Must be >= 0.
    [Index(1)]
    public required decimal Balance { get; set; }
}
