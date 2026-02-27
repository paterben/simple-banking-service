using CsvHelper.Configuration.Attributes;

namespace SimpleBankingService.IO.Records;

// Raw transaction record used for reading from / writing to CSV.
public record TransactionRecord
{
    // Originating account number. Must be 16 digits or less.
    [Index(0)]
    public required string FromAccount { get; set; }

    // Destination account number. Must be 16 digits or less.
    [Index(1)]
    public required string ToAccount { get; set; }

    // Transaction amount. Must be >= 0.
    [Index(2)]
    public required decimal Amount { get; init; }
}
