using FluentAssertions;
using SimpleBankingService.Models;

namespace SimpleBankingService.UnitTests;

[TestClass]
public sealed class TransactionProcessorCoreTests
{
    [TestMethod]
    public void ApplyTransactionsToAccounts_WhenValidTransactions_Works()
    {
        AccountNumber accountNum1 = new(1);
        AccountNumber accountNum2 = new(2);
        AccountNumber accountNum3 = new(3);
        AccountNumber accountNum4 = new(4);
        Account account1 = new(accountNum1, 100.00M);
        Account account2 = new(accountNum2, 200.00M);
        Account account3 = new(accountNum3, 0.00M);
        Account account4 = new(accountNum4, 1000.00M);
        Dictionary<AccountNumber, Account> accounts = new() {
            { accountNum1, account1 },
            { accountNum2, account2 },
            { accountNum3, account3 },
            { accountNum4, account4 },
        };
        List<Transaction> transactions = [
            new Transaction(accountNum1, accountNum2, 50.00M),
            new Transaction(accountNum2, accountNum3, 250.00M),
            new Transaction(accountNum4, accountNum3, 700.00M),
            new Transaction(accountNum3, accountNum2, 650.00M),
        ];

        TransactionProcessorCore.ApplyTransactionsToAccounts(accounts, transactions);

        account1.Balance.Should().Be(50.00M);
        account2.Balance.Should().Be(650.00M);
        account3.Balance.Should().Be(300.00M);
        account4.Balance.Should().Be(300.00M);
    }

    [TestMethod]
    public void ApplyTransactionsToAccounts_WhenUnknownAccount_Throws()
    {
        AccountNumber accountNum1 = new(1);
        Account account1 = new(accountNum1, 100.00M);
        Dictionary<AccountNumber, Account> accounts = new() {
            { accountNum1, account1 },
        };
        List<Transaction> transactions = [
            new Transaction(accountNum1, new(2), 50.00M),
        ];

        var action = () => TransactionProcessorCore.ApplyTransactionsToAccounts(accounts, transactions);
        action
          .Should().Throw<KeyNotFoundException>()
          .WithMessage("*The given key '0000000000000002' was not present*");
    }

    [TestMethod]
    // Note: In this test, if all transactions were applied, the ending balance of each account would be positive.
    // However, each transaction is applied in order, and the second transaction causes account 2's balance to
    // temporarily go below 0, so the processing fails.
    public void ApplyTransactionsToAccounts_WhenInsufficientBalance_Throws()
    {
        AccountNumber accountNum1 = new(1);
        AccountNumber accountNum2 = new(2);
        Account account1 = new(accountNum1, 100.00M);
        Account account2 = new(accountNum2, 200.00M);
        Dictionary<AccountNumber, Account> accounts = new() {
            { accountNum1, account1 },
            { accountNum2, account2 },
        };
        List<Transaction> transactions = [
            new Transaction(accountNum2, accountNum1, 30.00M),
            new Transaction(accountNum1, accountNum2, 150.00M),
            new Transaction(accountNum2, accountNum1, 150.00M),
        ];

        var action = () => TransactionProcessorCore.ApplyTransactionsToAccounts(accounts, transactions);
        action
          .Should().Throw<InvalidOperationException>()
          .WithMessage("ApplyToBalance: account 0000000000000001 has insufficient balance to subtract 150.00: current balance 130.00");
    }
}
