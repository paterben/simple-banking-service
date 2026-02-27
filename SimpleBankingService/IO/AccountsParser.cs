using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using SimpleBankingService.IO.Records;
using SimpleBankingService.Models;

namespace SimpleBankingService.IO;

public class AccountsParser
{
    /// <summary>
    /// Parses accounts and balances from a CSV file.
    /// Validates that account numbers and balances are in the correct format and that no duplicate accounts are present.
    /// </summary>
    /// <returns>A dictionary of accounts keyed by account number.</returns>
    /// <param name="accountBalancesFile">The CSV file containing account balances in <see cref="AccountBalanceRecord"/> format.</param>
    public static async Task<IDictionary<AccountNumber, Account>> ParseFromCsvAsync(FileInfo accountBalancesFile)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            TrimOptions = TrimOptions.Trim, // trim whitespace
        };

        Dictionary<AccountNumber, Account> accounts = [];

        using StreamReader reader = accountBalancesFile.OpenText();
        using CsvReader csv = new(reader, config);
        for (int line = 1; await csv.ReadAsync(); line++)
        {
            var accountBalanceRecord = csv.GetRecord<AccountBalanceRecord>();
            Account account = ValidateAndConvertRecord(accountBalanceRecord, line);
            if (accounts.ContainsKey(account.Number))
            {
                throw new InvalidDataException($"On line {line}: Duplicate account number: {account.Number}");
            }
            accounts.Add(account.Number, account);
        }
        return accounts;
    }

    private static Account ValidateAndConvertRecord(AccountBalanceRecord accountBalanceRecord, int line)
    {
        if (!AccountNumber.TryParse(accountBalanceRecord.AccountNumber, out AccountNumber accountNumber))
        {
            throw new InvalidDataException($"On line {line}: Invalid AccountBalanceRecord.AccountNumber format: {accountBalanceRecord.AccountNumber}");
        }
        if (accountBalanceRecord.Balance < 0)
        {
            throw new InvalidDataException($"On line {line}: AccountBalanceRecord.Balance cannot be negative: {accountBalanceRecord.Balance}");
        }

        Account account = new(accountNumber, accountBalanceRecord.Balance);
        return account;
    }
}
