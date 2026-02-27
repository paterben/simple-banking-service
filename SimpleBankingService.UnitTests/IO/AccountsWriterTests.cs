using FluentAssertions;
using SimpleBankingService.IO;
using SimpleBankingService.Models;

namespace SimpleBankingService.UnitTests;

[TestClass]
public sealed class AccountsWriterTests
{
    public TestContext TestContext { get; set; }

    [TestMethod]
    public async Task WriteToCsvAsync_Works()
    {
        Account account1 = new(new(1), 5000.00M);
        Account account2 = new(new(2), 10000.50M);
        Account account3 = new(new(3), 0.00M);
        Account account4 = new(new(1234), 1.01M);
        Account account5 = new(new(5), 2.00M);
        Account account6 = new(new(6), 3M);
        List<Account> accounts = [account1, account2, account3, account4, account5, account6];

        await AccountsWriter.WriteToCsvAsync(accounts, new FileInfo(@"testdata\output_accounts.csv"));

        string fileContents = await File.ReadAllTextAsync(@"testdata\output_accounts.csv", TestContext.CancellationToken);
        string expectedFileContents = await File.ReadAllTextAsync(@"testdata\expected_output_accounts.csv", TestContext.CancellationToken);

        fileContents.Should().Be(expectedFileContents);
    }

}
