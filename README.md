# Simple banking service

Contains a simple implementation of a banking service.

TODO better description.

## Cloning the repo

To clone, run:

```shell
git clone https://github.com/paterben/simple-banking-service.git
```

## Building and running

Building the repo requires the .NET 10 SDK, available at https://dotnet.microsoft.com/en-us/download/dotnet/10.0.

You can open the solution in Visual Studio (untested) or VSCode, or build and run it from the top-level directory:

```shell
dotnet run --project SimpleBankingService -- --accountBalances data\example_account_balances.csv --transactions data\example_transactions.csv --output output.csv
```

To see all possible command-line arguments, run:

```shell
dotnet run --project SimpleBankingService -- --help
```

## TODO

*   End to end tests
*   Improve README.md: features, assumptions, future work
