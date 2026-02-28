namespace SimpleBankingService.Models;

/// <summary>
/// Core account representation.
/// </summary>
public class Account
{
    public Account(AccountNumber number, decimal balance)
    {
        if (balance < 0) { throw new ArgumentOutOfRangeException(nameof(balance), $"account balance cannot be negative: {balance}"); }
        Number = number;
        Balance = balance;
    }

    /// <summary>
    /// The account number.
    /// </summary>
    public AccountNumber Number { get; private set; }

    /// <summary>
    /// The running balance of the account. Must always be >= 0.
    /// </summary>
    public decimal Balance { get; private set; }

    /// <summary>
    /// Checks whether <see cref="ApplyToBalance"/> can safely be called.
    /// </summary>
    public bool CanApplyToBalance(decimal amount)
    {
        // Any positive amount can be applied.
        // Negative amounts can only be applied if the resulting balance would be >= 0.
        return amount >= 0 || Balance >= -amount;
    }

    /// <summary>
    /// Applies (adds) the amount to the account balance.
    /// Throws if the amount is negative and the account has insufficient balance.
    /// </summary>
    public decimal ApplyToBalance(decimal amount)
    {
        if (!CanApplyToBalance(amount))
        {
            throw new InvalidOperationException($"ApplyToBalance: account {Number} has insufficient balance to subtract {-amount}: current balance {Balance}");
        }
        // Make sure that an OverflowException is thrown in case of arithmetic overflow.
        checked { Balance += amount; }
        return Balance;
    }

    public override string ToString()
    {
        return $"Account: Number={Number}, Balance={Balance}";
    }
}
