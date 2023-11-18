using App.WalletValue;
using App.WalletValue.Exchange;
using App.WalletValue.Wallet;
using App.WalletValue.WalletReturns;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

var serviceProvider = AppServiceProvider.Build();

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

var wallet = serviceProvider.GetRequiredService<IWallet>();
var returnService = serviceProvider.GetRequiredService<WalletReturnsService>();
var exchangeClient = serviceProvider.GetRequiredService<IExchangeClient>();
var returns = await returnService.GetReturnsAsync(wallet, cts.Token);

var table = new Table();
table.Border(TableBorder.MinimalHeavyHead);

// Add some columns
table.AddColumn(new TableColumn("Date")
    .Footer($"{returns.Date:yyyy-MM-dd}"));
table.AddColumn(new TableColumn("Exchange Rate")
    .Footer($"$ {returns.ExchangeRate:0}"));
table.AddColumn(new TableColumn("Transaction ₿"));
table.AddColumn(new TableColumn("Transaction $"));
table.AddColumn(new TableColumn("Total ₿")
    .Footer($"₿ {returns.BtcTotal:0.0000}"));
table.AddColumn(new TableColumn("Total $")
    .Footer($"$ {returns.FiatTotal:0}"));

foreach (var returnRow in returns.Rows)
{
    var fiatValue = returnRow.Fiat;
    table.AddRow(
        returnRow.Date.ToString("yyyy-MM-dd"),
        $"$ {returnRow.ExchangeRate:0}",
        returnRow.Sats > 0 ? $"[green]₿ {returnRow.Btc:0.0000}[/]" : $"[red]₿{returnRow.Btc:0.0000}[/]",
        returnRow.Sats > 0 ? $"[green]$ {returnRow.Fiat:0}[/]" : $"[red]${returnRow.Fiat:0}[/]",
        $"₿ {returnRow.BtcTotal:0.0000}",
        $"$ {returnRow.FiatTotal:0}");
}

AnsiConsole.Write(table);

AnsiConsole.MarkupLine($"Total bought Fiat: [bold]{returns.TotalBoughtFiat}[/]");