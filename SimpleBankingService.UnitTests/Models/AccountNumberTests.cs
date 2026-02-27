using FluentAssertions;
using SimpleBankingService.Models;

namespace SimpleBankingService.UnitTests.Models;

[TestClass]
public sealed class AccountNumberTests
{
    [TestMethod]
    public void AreEqual_WhenSameValue_ReturnsTrue()
    {
        AccountNumber accountNumber1 = new(1);
        AccountNumber accountNumber2 = new(1);
        Assert.AreEqual(accountNumber1, accountNumber2);
    }

    [TestMethod]
    public void AreEqual_WhenDifferentValue_ReturnsFalse()
    {
        AccountNumber accountNumber1 = new(1);
        AccountNumber accountNumber2 = new(2);
        Assert.AreNotEqual(accountNumber1, accountNumber2);
    }

    [TestMethod]
    public void Constructor_WhenNegativeAccountNumber_Throws()
    {
        var action = () => new AccountNumber(-1);
        action
          .Should().Throw<ArgumentOutOfRangeException>()
          .WithMessage("account number cannot be negative*");
    }

    [TestMethod]
    public void ToString_Works()
    {
        new AccountNumber(12345).ToString().Should().Be("0000000000012345");
        new AccountNumber(1234567890123456).ToString().Should().Be("1234567890123456");
        new AccountNumber(9999999999999999).ToString().Should().Be("9999999999999999");
    }

    [TestMethod]
    public void TryParse_WhenValidAccountNumber_Works()
    {
        Assert.IsTrue(AccountNumber.TryParse("12345", out AccountNumber accountNumber));
        accountNumber.Value.Should().Be(12345);
        Assert.IsTrue(AccountNumber.TryParse("0012345", out accountNumber));
        accountNumber.Value.Should().Be(12345);
        Assert.IsTrue(AccountNumber.TryParse("0000000000012345", out accountNumber));
        accountNumber.Value.Should().Be(12345);
        Assert.IsTrue(AccountNumber.TryParse("9999999999999999", out accountNumber));
        accountNumber.Value.Should().Be(9999999999999999);
        Assert.IsTrue(AccountNumber.TryParse("1", out accountNumber));
        accountNumber.Value.Should().Be(1);
    }

    [TestMethod]
    public void TryParse_WhenNegativeAccountNumber_ReturnsFalse()
    {
        Assert.IsFalse(AccountNumber.TryParse("-1", out _));
        Assert.IsFalse(AccountNumber.TryParse("-12345", out _));
        Assert.IsFalse(AccountNumber.TryParse("-1234567890123456", out _));
        Assert.IsFalse(AccountNumber.TryParse("-9999999999999999", out _));
        Assert.IsFalse(AccountNumber.TryParse("0", out _));
        Assert.IsFalse(AccountNumber.TryParse("-0", out _));
    }

    [TestMethod]
    public void TryParse_WhenTooManyDigitsAccountNumber_ReturnsFalse()
    {
        Assert.IsFalse(AccountNumber.TryParse("10000000000000000", out _));
        Assert.IsFalse(AccountNumber.TryParse("12345678901234567", out _));
        Assert.IsFalse(AccountNumber.TryParse("00000000000000001", out _));
    }

    [TestMethod]
    public void TryParse_WhenBadFormatAccountNumber_ReturnsFalse()
    {
        Assert.IsFalse(AccountNumber.TryParse("", out _));
        Assert.IsFalse(AccountNumber.TryParse("1234aa", out _));
        Assert.IsFalse(AccountNumber.TryParse("--1234", out _));
        Assert.IsFalse(AccountNumber.TryParse("0x1234", out _));
    }
}
