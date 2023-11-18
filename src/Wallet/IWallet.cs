namespace App.WalletValue.Wallet;

interface IWallet
{
    IAsyncEnumerable<IWalletTransaction> GetTransactionsAsync(CancellationToken cancellationToken = default);

    IAsyncEnumerable<IWalletAddress> GetAddressesAsync(CancellationToken cancellationToken = default);
}
