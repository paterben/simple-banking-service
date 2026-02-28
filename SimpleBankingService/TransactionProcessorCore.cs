using SimpleBankingService.Models;

namespace SimpleBankingService;

public class TransactionProcessorCore
{
    /// <summary>
    /// Applies simple transaction processing rules:
    /// - Applies transactions in order.
    /// - Updates account balances according to each transaction.
    /// - Fails if any transaction would cause any account's balance to go negative, even temporarily.
    /// </summary>
    /// <param name="accounts">The list of accounts and balances. Balances will be updated during processing.</param>
    /// <param name="transactions">The list of transactions to apply.</param>
    public static void ApplyTransactionsToAccounts(IDictionary<AccountNumber, Account> accounts, IList<Transaction> transactions)
    {
        foreach (Transaction transaction in transactions)
        {
            // This will throw if the from or to accounts don't exist.
            Account fromAccount = accounts[transaction.FromAccount];
            Account toAccount = accounts[transaction.ToAccount];
            // This will throw if the account has insufficient funds.
            fromAccount.ApplyToBalance(-transaction.Amount);
            toAccount.ApplyToBalance(transaction.Amount);
        }
    }
}
