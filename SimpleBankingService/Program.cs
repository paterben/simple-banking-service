using System.CommandLine;

namespace SimpleBankingService;

public class Program
{
    public static int Main(string[] args)
    {
        var rootCommand = new RootCommand("Prints Hello World.");

        rootCommand.SetAction(parseResult =>
          {
              Console.WriteLine("Hello, World!");
              return 0;
          });

        ParseResult parseResult = rootCommand.Parse(args);
        return parseResult.Invoke();
    }
}
