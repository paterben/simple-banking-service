using System.CommandLine;

namespace SimpleBankingService;

/// <summary>
/// Simple banking service console application.
/// 
/// Currently contains a single command: processTransactions. See the <see cref="ProcessTransactions"/> class for details.
/// </summary>
public class Program
{
    public static async Task<int> Main(string[] args)
    {
        Option<FileInfo> accountBalancesFileOption = new("--accountBalances", "-a")
        {
            Description = "A CSV file containing current account balances.",
            Required = true
        };
        Option<FileInfo> transactionsFileOption = new("--transactions", "-t")
        {
            Description = "A CSV file of transactions to apply.",
            Required = true
        };
        Option<FileInfo> outputFileOption = new("--output", "-o")
        {
            Description = "Output CSV file. This will contain the new account balances after applying all transactions.",
            Required = true
        };

        RootCommand rootCommand = new("Simple banking service.");
        Command processTransactionsCommand = new("processTransactions", "Processes transactions. Loads current account balances and daily transactions to apply, and outputs new account balances.");
        processTransactionsCommand.Options.Add(accountBalancesFileOption);
        processTransactionsCommand.Options.Add(transactionsFileOption);
        processTransactionsCommand.Options.Add(outputFileOption);
        rootCommand.Subcommands.Add(processTransactionsCommand);

        processTransactionsCommand.SetAction(async (ParseResult parseResult, CancellationToken cancellationToken) =>
          {
              FileInfo accountBalancesFile = parseResult.GetValue(accountBalancesFileOption)!;
              FileInfo transactionsFile = parseResult.GetValue(transactionsFileOption)!;
              FileInfo outputFile = parseResult.GetValue(outputFileOption)!;
              await ProcessTransactions.RunAsync(accountBalancesFile, transactionsFile, outputFile);
              return 0;
          });

        ParseResult parseResult = rootCommand.Parse(args);
        return await parseResult.InvokeAsync();
    }
}
