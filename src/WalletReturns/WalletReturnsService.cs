using App.WalletValue.Blockchain;
using App.WalletValue.Exchange;
using App.WalletValue.Wallet;

namespace App.WalletValue.WalletReturns;

class WalletReturnsService
{
    private readonly IBlockchainClient _blockchainClient;
    private readonly IExchangeClient _exchangeClient;

    public WalletReturnsService(IBlockchainClient blockchainClient, IExchangeClient exchangeClient)
    {
        _blockchainClient = blockchainClient;
        _exchangeClient = exchangeClient;
    }

    public async Task<WalletReturns> GetReturnsAsync(IWallet wallet, CancellationToken cancellationToken = default)
    {
        var transactions = await wallet
            .GetTransactionsAsync(cancellationToken)
            .ToListAsync(cancellationToken);

        var addresses = await wallet
            .GetAddressesAsync(cancellationToken)
            .ToListAsync(cancellationToken);

        var rows = new List<WalletReturnsRow>();
        await Task.WhenAll(transactions.Select(async transaction =>
        {
            var transactionInfo = await _blockchainClient.GetTransactionAsync(transaction.Id, cancellationToken);
            var sats = transactionInfo.GetTransactionValue(addresses);

            rows.Add(new WalletReturnsRow
            {
                Id = transaction.Id,
                Date = transactionInfo.Date,
                BlockHeight = transactionInfo.BlockHeight,
                Note = transaction.Note,
                Sats = sats,
                SatsTotal = sats + rows.Where(r => r.BlockHeight < transactionInfo.BlockHeight).Sum(r => r.Sats),
                ExchangeRate = await _exchangeClient.GetExchangeRateAsync(transactionInfo.Date, cancellationToken)
            });
        }));

        // Sorts by block height
        rows.Sort((a, b) => a.BlockHeight - b.BlockHeight);

        var now = DateTime.Now;
        var exchangeRate = await _exchangeClient.GetExchangeRateAsync(now, cancellationToken);

        return new WalletReturns
        {
            Date = now,
            SatsTotal = rows.Sum(r => r.Sats),
            ExchangeRate = exchangeRate,
            Rows = rows
        };
    }
}
