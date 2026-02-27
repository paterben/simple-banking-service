using FluentAssertions;

namespace SimpleBankingService.FunctionalTests;

[TestClass]
public sealed class ProgramTests
{
    public TestContext TestContext { get; set; }

    [TestMethod]
    public async Task DoRootCommandAsync_OutputsExpectedCsv()
    {
        await Program.DoRootCommandAsync(new FileInfo(@"data\account_balances.csv"), new FileInfo(@"data\transactions.csv"), new FileInfo(@"data\output.csv"));

        string fileContents = await File.ReadAllTextAsync(@"data\output.csv", TestContext.CancellationToken);
        string expectedFileContents = await File.ReadAllTextAsync(@"data\expected_output.csv", TestContext.CancellationToken);

        fileContents.Should().Be(expectedFileContents);
    }
}
