using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using SimpleBankingService.IO.Records;
using SimpleBankingService.Models;

namespace SimpleBankingService.IO;

public class AccountsWriter
{
    /// <summary>
    /// Writes accounts and balances to a CSV file.
    /// </summary>
    /// <param name="accounts">The list of accounts and their balances.</param>
    /// <param name="outputAccountBalancesFile">Output path for the CSV file which will contain account balances in <see cref="AccountBalanceRecord"/> format.</param>
    public static async Task WriteToCsvAsync(IList<Account> accounts, FileInfo outputAccountBalancesFile)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
        };

        IEnumerable<AccountBalanceRecord> accountBalanceRecords =
          accounts.Select(account => new AccountBalanceRecord()
          {
              AccountNumber = account.Number.ToString(),
              Balance = account.Balance
          });

        using StreamWriter writer = outputAccountBalancesFile.CreateText();
        using CsvWriter csv = new(writer, config);
        await csv.WriteRecordsAsync(accountBalanceRecords);
    }
}
