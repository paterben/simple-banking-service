using CsvHelper.TypeConversion;
using FluentAssertions;
using SimpleBankingService.IO;
using SimpleBankingService.Models;

namespace SimpleBankingService.UnitTests.IO;

[TestClass]
public sealed class TransactionsParserTests
{
    [TestMethod]
    public async Task ParseFromCsv_WhenValidData_Works()
    {
        AccountNumber accountNum1 = new(1);
        AccountNumber accountNum2 = new(2);
        AccountNumber accountNum3 = new(3);
        AccountNumber accountNum4 = new(4);
        AccountNumber accountNum5 = new(5);
        AccountNumber accountNum6 = new(6);
        HashSet<AccountNumber> accounts = [accountNum1, accountNum2, accountNum3, accountNum4, accountNum5, accountNum6];

        IList<Transaction> transactions =
          await TransactionsParser.ParseFromCsvAsync(accounts, new FileInfo(@"testdata\transactions_valid.csv"));

        Transaction transaction1 = new(accountNum1, accountNum2, 1000.00M);
        Transaction transaction2 = new(accountNum4, accountNum2, 300.50M);
        Transaction transaction3 = new(accountNum2, accountNum3, 200.00M);
        Transaction transaction4 = new(accountNum1, accountNum2, 10000.00M);
        Transaction transaction5 = new(accountNum5, accountNum1, 0.00M);
        Transaction transaction6 = new(accountNum3, accountNum1, 3.00M);

        Transaction[] expectedTransactions = [transaction1, transaction2, transaction3, transaction4, transaction5, transaction6];
        transactions.Should().BeEquivalentTo(expectedTransactions);
    }

    [TestMethod]
    public async Task ParseFromCsv_WhenEmpty_ReturnsNoTransactions()
    {
        HashSet<AccountNumber> accounts = [new(1)];
        IList<Transaction> transactions =
          await TransactionsParser.ParseFromCsvAsync(accounts, new FileInfo(@"testdata\empty.csv"));
        transactions.Count.Should().Be(0);
    }

    [TestMethod]
    public async Task ParseFromCsv_WhenFileDoesNotExist_Throws()
    {
        HashSet<AccountNumber> accounts = [new(1)];
        var action = async () =>
           await TransactionsParser.ParseFromCsvAsync(accounts, new FileInfo(@"testdata\nonexistent.csv"));
        await action.Should().ThrowAsync<FileNotFoundException>();
    }

    [TestMethod]
    public async Task ParseFromCsv_WhenInvalidFromAccount_Throws()
    {
        HashSet<AccountNumber> accounts = [new(1), new(2), new(3), new(4)];
        var action = async () =>
          await TransactionsParser.ParseFromCsvAsync(accounts, new FileInfo(@"testdata\transactions_invalid_fromaccount.csv"));
        await action
          .Should().ThrowAsync<InvalidDataException>()
          .WithMessage("On line 2: Invalid TransactionRecord.FromAccount format: 111aaaaaaaaa1234");
    }

    [TestMethod]
    public async Task ParseFromCsv_WhenInvalidToAccount_Throws()
    {
        HashSet<AccountNumber> accounts = [new(1), new(2), new(3), new(4)];
        var action = async () =>
          await TransactionsParser.ParseFromCsvAsync(accounts, new FileInfo(@"testdata\transactions_invalid_toaccount.csv"));
        await action
          .Should().ThrowAsync<InvalidDataException>()
          .WithMessage("On line 2: Invalid TransactionRecord.ToAccount format: 111aaaaaaaaa1234");
    }

    [TestMethod]
    public async Task ParseFromCsv_WhenUnknownFromAccount_Throws()
    {
        HashSet<AccountNumber> accounts = [new(1), new(2), new(3), new(4)];
        var action = async () =>
          await TransactionsParser.ParseFromCsvAsync(accounts, new FileInfo(@"testdata\transactions_unknown_fromaccount.csv"));
        await action
          .Should().ThrowAsync<InvalidDataException>()
          .WithMessage("On line 2: Unknown TransactionRecord.FromAccount (account does not exist): 1234567890123456");
    }

    [TestMethod]
    public async Task ParseFromCsv_WhenUnknownToAccount_Throws()
    {
        HashSet<AccountNumber> accounts = [new(1), new(2), new(3), new(4)];
        var action = async () =>
          await TransactionsParser.ParseFromCsvAsync(accounts, new FileInfo(@"testdata\transactions_unknown_toaccount.csv"));
        await action
          .Should().ThrowAsync<InvalidDataException>()
          .WithMessage("On line 2: Unknown TransactionRecord.ToAccount (account does not exist): 1234567890123456");
    }

    [TestMethod]
    public async Task ParseFromCsv_WhenSameFromAndToAccount_Throws()
    {
        HashSet<AccountNumber> accounts = [new(1), new(2), new(3), new(4)];
        var action = async () =>
          await TransactionsParser.ParseFromCsvAsync(accounts, new FileInfo(@"testdata\transactions_same_from_and_to_account.csv"));
        await action
          .Should().ThrowAsync<InvalidDataException>()
          .WithMessage("On line 2: TransactionRecord.FromAccount and TransactionRecord.ToAccount cannot be the same account: 0000000000000004");
    }

    [TestMethod]
    public async Task ParseFromCsv_WhenInvalidAmount_Throws()
    {
        HashSet<AccountNumber> accounts = [new(1), new(2), new(3), new(4)];
        var action = async () =>
          await TransactionsParser.ParseFromCsvAsync(accounts, new FileInfo(@"testdata\transactions_invalid_amount.csv"));
        await action
          .Should().ThrowAsync<TypeConverterException>()
          .WithMessage("*Text: 'abcdef'*");
    }

    [TestMethod]
    public async Task ParseFromCsv_WhenNegativeAmount_Throws()
    {
        HashSet<AccountNumber> accounts = [new(1), new(2), new(3), new(4)];
        var action = async () =>
          await TransactionsParser.ParseFromCsvAsync(accounts, new FileInfo(@"testdata\transactions_negative_amount.csv"));
        await action
          .Should().ThrowAsync<InvalidDataException>()
          .WithMessage("On line 2: TransactionRecord.Amount cannot be negative: -2.00");
    }
}
