namespace App.WalletValue.Blockchain;

interface IBlockchainClient
{
    public Task<IBlockchainTransaction> GetTransactionAsync(string id, CancellationToken cancellationToken = default);
}
