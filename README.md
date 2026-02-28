# Simple banking service

Implementation of a simple banking service, allowing to keep track of account balances and apply transactions between accounts.

## Features

The service is built as a console app which currently contains a single command: `processTransactions`. This command reads in two CSV files, one representing current account balances, and another representing transactions to apply. It applies the transactions in order and outputs a new CSV file with the updated account balances. This is intended to be used e.g. for daily batch processing of client account transactions.

See [example account balances](SimpleBankingService.FunctionalTests/data/account_balances.csv), [example transactions](SimpleBankingService.FunctionalTests/data/transactions.csv) and [example output](SimpleBankingService.FunctionalTests/data/expected_output.csv).

Note: The input CSV files must not contain CSV headers.

The service will print detailed error messages for most failure scenarios, e.g. if a transaction is made to/from an unknown account, in case of formatting errors, or in case a transaction cannot be applied due to insufficient account balance.

## Assumptions and limitations

*   Account numbers are integers up to 16 digits long. They can be written in short form (e.g. `12345`) or canonical form (e.g. `0000000000012345`) or anything in between. These all represent the same account.
*   All transaction and account balance amounts must be non-negative (>= 0). Transactions must be between two different accounts.
*   Accounts can never go into negative balance, even temporarily. Any transaction causing an account to go into negative balance will cause processing to fail and an error message will be displayed.
*   There is no concept of account currency. All accounts are deemed to be in the same currency.
*   Account balance precision is limited by C#'s `decimal` type (which is purpose-built for monetary and financial calculations).

## Cloning the repo

To clone, run:

```shell
git clone https://github.com/paterben/simple-banking-service.git
```

## Building and running

Building the repo requires the .NET 10 SDK, available at https://dotnet.microsoft.com/en-us/download/dotnet/10.0.

You can open the solution in Visual Studio (untested) or VSCode.

To build and run from the top-level directory, using example data:

```shell
dotnet run --project SimpleBankingService -- processTransactions --accountBalances SimpleBankingService.FunctionalTests\data\account_balances.csv --transactions SimpleBankingService.FunctionalTests\data\transactions.csv -o output.csv
```

To see all possible command-line arguments, run:

```shell
dotnet run --project SimpleBankingService -- processTransactions --help
```

You can run all tests with:

```shell
dotnet test
```

## Possible improvements

Here are a few possible directions for improvement:

*   Support other file formats e.g. JSON or connecting to a SQL database.
*   Add a date component to transactions and accounts to keep track of up to when transactions have been processed and to prevent duplicate processing.
*   Add a client concept to support keeping track of accounts for multiple clients.
*   Support limitations to balance precision (e.g. allow cents precision only).
*   Add a currency concept and support for multiple currencies.
*   Support for more advanced transaction processing rules. For example:
    *   Allow transactions that temporarily put an account into negative balance as long as this is remedied by subsequent transactions in the same batch.
    *   Allow rejecting individual transactions instead of failing the whole processing. Keep track of rejected transactions in a separate file or table.
*   Add an API to process transactions in real time instead of in batches.

## AI statement

I did not use any AI tooling assistance during implementation (apart from AI results from normal search engine queries).
