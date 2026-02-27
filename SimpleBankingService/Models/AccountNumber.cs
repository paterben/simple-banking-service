namespace SimpleBankingService.Models;

/// <summary>
/// Simple value-type wrapper around account numbers, used to ensure consistent 16-digit representation.
/// </summary>
public readonly record struct AccountNumber
{
    public AccountNumber(long value)
    {
        if (value < 0) { throw new ArgumentOutOfRangeException(nameof(value), $"account number cannot be negative: {value}"); }
        Value = value;
    }

    public long Value { get; }

    public override string ToString()
    {
        return $"{Value:D16}";
    }

    /// <summary>
    /// Tries to parse an account number from a string.
    /// Supports any integer representation up to 16 digits long, i.e. the following
    /// string representations are supported and are parsed to the same account number:
    /// - "0000000000123456"
    /// - "123456"
    /// </summary>
    public static bool TryParse(string accountNumberStr, out AccountNumber accountNumber)
    {
        accountNumber = new AccountNumber(0);
        if (accountNumberStr.Length > 16)
        {
            return false;
        }
        bool parse = long.TryParse(accountNumberStr, out long accountNumberValue);
        if (!parse) return false;
        // We consider that 0 is not a valid account number.
        if (accountNumberValue <= 0) return false;
        accountNumber = new AccountNumber(accountNumberValue);
        return true;
    }
}
