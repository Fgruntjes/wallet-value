namespace App.WalletValue.Exchange;

interface IExchangeClient
{
    public Task<decimal> GetExchangeRateAsync(DateTime date, CancellationToken cancellationToken = default);
}
