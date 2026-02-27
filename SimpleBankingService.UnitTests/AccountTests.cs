using FluentAssertions;
using SimpleBankingService.Models;

namespace SimpleBankingService.UnitTests;

[TestClass]
public sealed class AccountTests
{
    [TestMethod]
    public void AccountPropertiesTest()
    {
        Account account = new(new AccountNumber(1), 100.00M);
        account.Number.Should().Be(new AccountNumber(1));
        account.Balance.Should().Be(100.00M);
    }

    [TestMethod]
    public void ToStringTest()
    {
        Account account = new(new AccountNumber(12345), 100.00M);
        account.ToString().Should().Be("Account: Number=0000000000012345, Balance=100.00");
    }

    [TestMethod]
    public void ConstructorNegativeBalanceThrowsTest()
    {
        var action = () => new Account(new AccountNumber(1), -1.00M);
        action
          .Should().Throw<ArgumentOutOfRangeException>()
          .WithMessage("account balance cannot be negative*");
    }

    [TestMethod]
    public void ApplyPositiveAmountToBalanceTest()
    {
        Account account = new(new AccountNumber(1), 100.00M);
        Assert.IsTrue(account.CanApplyToBalance(1.02M));
        account.ApplyToBalance(1.02M);
        account.Balance.Should().Be(101.02M);
    }

    [TestMethod]
    public void ApplyZeroToBalanceTest()
    {
        Account account = new(new AccountNumber(1), 100.00M);
        Assert.IsTrue(account.CanApplyToBalance(0M));
        account.ApplyToBalance(0M);
        account.Balance.Should().Be(100.00M);
    }

    [TestMethod]
    public void ApplyZeroToZeroBalanceTest()
    {
        Account account = new(new AccountNumber(1), 0M);
        Assert.IsTrue(account.CanApplyToBalance(0M));
        account.ApplyToBalance(0M);
        account.Balance.Should().Be(0M);
    }

    [TestMethod]
    public void ApplyNegativeAmountToSufficentBalanceTest()
    {
        Account account = new(new AccountNumber(1), 100.00M);
        Assert.IsTrue(account.CanApplyToBalance(-1.02M));
        account.ApplyToBalance(-1.02M);
        account.Balance.Should().Be(98.98M);
    }

    [TestMethod]
    public void ApplyNegativeAmountEntireBalanceTest()
    {
        Account account = new(new AccountNumber(1), 100.00M);
        Assert.IsTrue(account.CanApplyToBalance(-100.00M));
        account.ApplyToBalance(-100.00M);
        account.Balance.Should().Be(0M);
    }

    [TestMethod]
    public void ApplyNegativeAmountToInsufficentBalanceThrowsTest()
    {
        Account account = new(new AccountNumber(1), 100.00M);
        Assert.IsFalse(account.CanApplyToBalance(-100.01M));
        var action = () => account.ApplyToBalance(-100.01M);
        action
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*insufficient balance*");
        account.Balance.Should().Be(100.00M);
    }

    [TestMethod]
    public void ApplyMultipleAmountsTest()
    {
        Account account = new(new AccountNumber(1), 100.00M);
        account.ApplyToBalance(50.00M);
        account.ApplyToBalance(-120.00M);
        account.ApplyToBalance(-20.00M);
        account.ApplyToBalance(5.50M);
        account.Balance.Should().Be(15.50M);
    }

    [TestMethod]
    public void BalanceOverflowThrowsTest()
    {
        Account account = new(new AccountNumber(1), decimal.MaxValue - 10M);
        var action = () => account.ApplyToBalance(50.00M);
        action.Should().Throw<OverflowException>();
        account.Balance.Should().Be(decimal.MaxValue - 10M);
    }
}
