using System.CommandLine;
using SimpleBankingService.IO;
using SimpleBankingService.Models;

namespace SimpleBankingService;

/// <summary>
/// Simple console application that takes a CSV file of account balances and a CSV file of transactions,
/// applies the transactions in order, and outputs a CSV file containing new account balances.
/// </summary>
public class Program
{
    public static async Task<int> Main(string[] args)
    {
        Option<FileInfo> accountBalancesFileOption = new("--accountBalances")
        {
            Description = "A CSV file containing current account balances.",
            Required = true
        };
        Option<FileInfo> transactionsFileOption = new("--transactions")
        {
            Description = "A CSV file of transactions to apply.",
            Required = true
        };
        Option<FileInfo> outputFileOption = new("--output")
        {
            Description = "Output CSV file. This will contain the new account balances after applying all transactions.",
            Required = true
        };

        RootCommand rootCommand = new("Loads current account balances and daily transactions to apply, and outputs new account balances.");
        rootCommand.Options.Add(accountBalancesFileOption);
        rootCommand.Options.Add(transactionsFileOption);
        rootCommand.Options.Add(outputFileOption);

        rootCommand.SetAction(async (ParseResult parseResult, CancellationToken cancellationToken) =>
          {
              FileInfo accountBalancesFile = parseResult.GetValue(accountBalancesFileOption)!;
              FileInfo transactionsFile = parseResult.GetValue(transactionsFileOption)!;
              FileInfo outputFile = parseResult.GetValue(outputFileOption)!;
              return await DoRootCommandAsync(accountBalancesFile, transactionsFile, outputFile);
          });

        ParseResult parseResult = rootCommand.Parse(args);
        return await parseResult.InvokeAsync();
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

    public static async Task<int> DoRootCommandAsync(FileInfo accountBalancesFile, FileInfo transactionsFile, FileInfo outputFile)
    {
        IDictionary<AccountNumber, Account> accounts = await ParseAccountsAsync(accountBalancesFile);
        HashSet<AccountNumber> accountNumbers = [.. accounts.Keys];

        IList<Transaction> transactions = await ParseTransactionsAsync(accountNumbers, transactionsFile);

        TransactionProcessor.ApplyTransactionsToAccounts(accounts, transactions);

        await WriteAccountsAsync(accounts, outputFile);

        return 0;
    }
}
