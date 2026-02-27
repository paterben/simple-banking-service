using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using SimpleBankingService.IO.Records;
using SimpleBankingService.Models;

namespace SimpleBankingService.IO;

public class TransactionsParser
{
    /// <summary>
    /// Parses transactions from a CSV file.
    /// Validates that account numbers and balances are in the correct format and that all referenced accounts exist.
    /// </summary>
    /// <returns>The list of transactions.</returns>
    /// <param name="accounts">The list of known accounts.</param>
    /// <param name="transactionsFile">The CSV file containing transactions in <see cref="TransactionRecord"/> format.</param>
    public static async Task<IList<Transaction>> ParseFromCsvAsync(HashSet<AccountNumber> accounts, FileInfo transactionsFile)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            TrimOptions = TrimOptions.Trim, // trim whitespace
        };

        List<Transaction> transactions = [];

        using StreamReader reader = transactionsFile.OpenText();
        using CsvReader csv = new(reader, config);
        for (int line = 1; await csv.ReadAsync(); line++)
        {
            var transactionRecord = csv.GetRecord<TransactionRecord>();
            Transaction transaction = ValidateAndConvertTransaction(accounts, transactionRecord, line);
            transactions.Add(transaction);
        }
        return transactions;
    }

    private static Transaction ValidateAndConvertTransaction(HashSet<AccountNumber> accounts, TransactionRecord transactionRecord, int line)
    {
        if (!AccountNumber.TryParse(transactionRecord.FromAccount, out AccountNumber fromAccount))
        {
            throw new InvalidDataException($"On line {line}: Invalid TransactionRecord.FromAccount format: {transactionRecord.FromAccount}");
        }
        if (!AccountNumber.TryParse(transactionRecord.ToAccount, out AccountNumber toAccount))
        {
            throw new InvalidDataException($"On line {line}: Invalid TransactionRecord.ToAccount format: {transactionRecord.ToAccount}");
        }
        if (fromAccount == toAccount)
        {
            throw new InvalidDataException($"On line {line}: TransactionRecord.FromAccount and TransactionRecord.ToAccount cannot be the same account: {fromAccount}");
        }
        if (!accounts.Contains(fromAccount))
        {
            throw new InvalidDataException($"On line {line}: Unknown TransactionRecord.FromAccount (account does not exist): {fromAccount}");
        }
        if (!accounts.Contains(toAccount))
        {
            throw new InvalidDataException($"On line {line}: Unknown TransactionRecord.ToAccount (account does not exist): {toAccount}");
        }
        if (transactionRecord.Amount < 0)
        {
            throw new InvalidDataException($"On line {line}: TransactionRecord.Amount cannot be negative: {transactionRecord.Amount}");
        }

        Transaction transaction = new(fromAccount, toAccount, transactionRecord.Amount);
        return transaction;
    }
}
