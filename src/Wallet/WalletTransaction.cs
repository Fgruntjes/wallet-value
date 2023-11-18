namespace App.WalletValue.Wallet;

internal class WalletTransaction : IWalletTransaction
{
    public string Id { get; }

    public string WalletId { get; }

    public string? Note { get; }

    public WalletTransaction(string id, string walletId, string? note)
    {
        Id = id;
        WalletId = walletId;
        Note = note;
    }
}
