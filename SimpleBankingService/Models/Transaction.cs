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

    public AccountNumber FromAccount { get; }

    public AccountNumber ToAccount { get; }

    public decimal Amount { get; }

    public override string ToString()
    {
        return $"Transaction: FromAccount={FromAccount}, ToAccount={ToAccount}, Amount={Amount}";
    }
}
