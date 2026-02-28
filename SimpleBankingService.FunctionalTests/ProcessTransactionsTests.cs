using FluentAssertions;

namespace SimpleBankingService.FunctionalTests;

[TestClass]
public sealed class ProcessTransactionsTests
{
    public TestContext TestContext { get; set; }

    [TestMethod]
    public async Task RunAsync_OutputsExpectedCsv()
    {
        await ProcessTransactions.RunAsync(new FileInfo(@"data\account_balances.csv"), new FileInfo(@"data\transactions.csv"), new FileInfo(@"data\output.csv"));

        string fileContents = await File.ReadAllTextAsync(@"data\output.csv", TestContext.CancellationToken);
        string expectedFileContents = await File.ReadAllTextAsync(@"data\expected_output.csv", TestContext.CancellationToken);

        fileContents.Should().Be(expectedFileContents);
    }

    [TestMethod]
    public async Task RunAsync_WhenFailingTransactions_Throws()
    {
        var action = async () =>
          await ProcessTransactions.RunAsync(new FileInfo(@"data\account_balances.csv"), new FileInfo(@"data\failing_transactions.csv"), new FileInfo(@"data\output.csv"));
        await action
          .Should().ThrowAsync<InvalidOperationException>()
          .WithMessage("ApplyToBalance: account 1111234522226789 has insufficient balance to subtract 5000.00: current balance 4500.00");
    }
}
