namespace SimpleBankingService.Models;

/// <summary>
/// Core transaction representation.
/// </summary>
public class Transaction
{
    public Transaction(AccountNumber fromAccount, AccountNumber toAccount, decimal amount)
    {
        if (amount < 0) { throw new ArgumentOutOfRangeException(nameof(amount), $"transaction amount cannot be negative: {amount}"); }
        FromAccount = fromAccount;
        ToAccount = toAccount;
        Amount = amount;
    }

    /// <summary>
    /// Account the amount will be debited from.
    /// </summary>
    public AccountNumber FromAccount { get; }

    /// <summary>
    /// Account the amount will be credited to.
    /// </summary>
    public AccountNumber ToAccount { get; }

    /// <summary>
    /// Transaction amount. Must always be >= 0.
    /// </summary>
    public decimal Amount { get; }

    public override string ToString()
    {
        return $"Transaction: FromAccount={FromAccount}, ToAccount={ToAccount}, Amount={Amount}";
    }
}
