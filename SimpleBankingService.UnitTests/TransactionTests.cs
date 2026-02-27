using FluentAssertions;
using SimpleBankingService.Models;

namespace SimpleBankingService.UnitTests;

[TestClass]
public sealed class TransactionTests
{
    [TestMethod]
    public void Constructor_Works()
    {
        Transaction transaction = new(new AccountNumber(1), new AccountNumber(2), 100.00M);
        transaction.FromAccount.Should().Be(new AccountNumber(1));
        transaction.ToAccount.Should().Be(new AccountNumber(2));
        transaction.Amount.Should().Be(100.00M);
    }

    [TestMethod]
    public void ToString_Works()
    {
        Transaction transaction = new(new AccountNumber(1), new AccountNumber(2), 100.00M);
        transaction.ToString().Should().Be("Transaction: FromAccount=0000000000000001, ToAccount=0000000000000002, Amount=100.00");
    }

    [TestMethod]
    public void Constructor_WhenNegativeAmount_Throws()
    {
        var action = () => new Transaction(new AccountNumber(1), new AccountNumber(2), -100.00M);
        action
          .Should().Throw<ArgumentOutOfRangeException>()
          .WithMessage("transaction amount cannot be negative*");
    }

    [TestMethod]
    public void Constructor_WhenZeroAmount_Works()
    {
        Transaction transaction = new(new AccountNumber(1), new AccountNumber(2), 0.00M);
        transaction.FromAccount.Should().Be(new AccountNumber(1));
        transaction.ToAccount.Should().Be(new AccountNumber(2));
        transaction.Amount.Should().Be(0.00M);
    }
}
