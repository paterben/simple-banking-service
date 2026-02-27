using CsvHelper.TypeConversion;
using FluentAssertions;
using SimpleBankingService.IO;
using SimpleBankingService.Models;

namespace SimpleBankingService.UnitTests.IO;

[TestClass]
public sealed class AccountsParserTests
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
        Account account1 = new(accountNum1, 5000.00M);
        Account account2 = new(accountNum2, 10000.00M);
        Account account3 = new(accountNum3, 0M);
        Account account4 = new(accountNum4, 1.01M);
        Account account5 = new(accountNum5, 2M);
        Account account6 = new(accountNum6, 3M);

        IDictionary<AccountNumber, Account> accounts =
          await AccountsParser.ParseFromCsvAsync(new FileInfo(@"testdata\account_balances_valid.csv"));

        accounts.Count.Should().Be(6);
        accounts.Should().ContainKeys(accountNum1, accountNum2, accountNum3, accountNum4, accountNum5, accountNum6);
        accounts[accountNum1].Should().BeEquivalentTo(account1);
        accounts[accountNum2].Should().BeEquivalentTo(account2);
        accounts[accountNum3].Should().BeEquivalentTo(account3);
        accounts[accountNum4].Should().BeEquivalentTo(account4);
        accounts[accountNum5].Should().BeEquivalentTo(account5);
        accounts[accountNum6].Should().BeEquivalentTo(account6);
    }

    [TestMethod]
    public async Task ParseFromCsv_WhenEmpty_ReturnsNoAccounts()
    {
        IDictionary<AccountNumber, Account> accounts =
          await AccountsParser.ParseFromCsvAsync(new FileInfo(@"testdata\empty.csv"));
        accounts.Count.Should().Be(0);
    }

    [TestMethod]
    public async Task ParseFromCsv_WhenFileDoesNotExist_Throws()
    {
        var action = async () =>
          await AccountsParser.ParseFromCsvAsync(new FileInfo(@"testdata\nonexistent.csv"));
        await action.Should().ThrowAsync<FileNotFoundException>();
    }

    [TestMethod]
    public async Task ParseFromCsv_WhenInvalidAccountNumber_Throws()
    {
        var action = async () =>
          await AccountsParser.ParseFromCsvAsync(new FileInfo(@"testdata\account_balances_invalid_account_number.csv"));
        await action
          .Should().ThrowAsync<InvalidDataException>()
          .WithMessage("On line 2: Invalid AccountBalanceRecord.AccountNumber format: 111aaaaaaaaa1234");
    }

    [TestMethod]
    public async Task ParseFromCsv_WhenDuplicateAccountNumber_Throws()
    {
        var action = async () =>
          await AccountsParser.ParseFromCsvAsync(new FileInfo(@"testdata\account_balances_duplicate_account_number.csv"));
        await action
          .Should().ThrowAsync<InvalidDataException>()
          .WithMessage("On line 3: Duplicate account number: 0000000000000001");
    }

    [TestMethod]
    public async Task ParseFromCsv_WhenInvalidBalance_Throws()
    {
        var action = async () =>
          await AccountsParser.ParseFromCsvAsync(new FileInfo(@"testdata\account_balances_invalid_balance.csv"));
        await action
          .Should().ThrowAsync<TypeConverterException>()
          .WithMessage("*Text: 'aaa'*");
    }

    [TestMethod]
    public async Task ParseFromCsv_WhenNegativeBalance_Throws()
    {
        var action = async () =>
          await AccountsParser.ParseFromCsvAsync(new FileInfo(@"testdata\account_balances_negative_balance.csv"));
        await action
          .Should().ThrowAsync<InvalidDataException>()
          .WithMessage("On line 2: AccountBalanceRecord.Balance cannot be negative: -2.00");
    }
}
