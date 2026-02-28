using SimpleBankingService.IO;
using SimpleBankingService.Models;

namespace SimpleBankingService;

/// <summary>
/// Implements the processTransactions subcommand.
/// </summary>
public class ProcessTransactions
{
    /// <summary>
    /// Runs the processTransactions subcommand.
    /// This subcommand takes a CSV file of account balances and a CSV file of transactions,
    /// applies the transactions in order, and outputs a CSV file containing new account balances.
    /// </summary>
    /// <param name="accountBalancesFile">The CSV file containing account balances in <see cref="AccountBalanceRecord"/> format.</param>
    /// <param name="transactionsFile">The CSV file containing transactions in <see cref="TransactionRecord"/> format.</param>
    /// <param name="outputFile">Output path for the CSV file which will contain account balances in <see cref="AccountBalanceRecord"/> format.</param>
    public static async Task RunAsync(FileInfo accountBalancesFile, FileInfo transactionsFile, FileInfo outputFile)
    {
        IDictionary<AccountNumber, Account> accounts = await ParseAccountsAsync(accountBalancesFile);
        HashSet<AccountNumber> accountNumbers = [.. accounts.Keys];

        IList<Transaction> transactions = await ParseTransactionsAsync(accountNumbers, transactionsFile);

        TransactionProcessorCore.ApplyTransactionsToAccounts(accounts, transactions);

        await WriteAccountsAsync(accounts, outputFile);
    }

    private static async Task<IDictionary<AccountNumber, Account>> ParseAccountsAsync(FileInfo accountBalancesFile)
    {
        Console.WriteLine($"Loading account balances from {accountBalancesFile.FullName}...");
        IDictionary<AccountNumber, Account> accounts = await AccountsParser.ParseFromCsvAsync(accountBalancesFile);
        Console.WriteLine($"Num accounts: {accounts.Count}");
        Console.WriteLine($"Total balance: {accounts.Sum(account => account.Value.Balance)}");
        foreach (Account account in accounts.Values)
        {
            Console.WriteLine(account);
        }
        return accounts;
    }

    private static async Task<IList<Transaction>> ParseTransactionsAsync(HashSet<AccountNumber> accounts, FileInfo transactionsFile)
    {
        Console.WriteLine($"Loading transactions from {transactionsFile.FullName}...");
        IList<Transaction> transactions = await TransactionsParser.ParseFromCsvAsync(accounts, transactionsFile);
        Console.WriteLine($"Num transactions: {transactions.Count}");
        Console.WriteLine($"Total amount: {transactions.Sum(ab => ab.Amount)}");
        foreach (Transaction transaction in transactions)
        {
            Console.WriteLine(transaction);
        }
        return transactions;
    }
    private static async Task WriteAccountsAsync(IDictionary<AccountNumber, Account> accounts, FileInfo outputFile)
    {
        Console.WriteLine($"Writing new account balances to {outputFile.FullName}...");
        await AccountsWriter.WriteToCsvAsync(accounts.Values.ToList(), outputFile);
        Console.WriteLine($"Num accounts: {accounts.Count}");
        Console.WriteLine($"Total balance: {accounts.Sum(account => account.Value.Balance)}");
        foreach (Account account in accounts.Values)
        {
            Console.WriteLine(account);
        }
    }
}
