namespace App.WalletValue.Wallet;

interface IWalletTransaction
{
    public string Id { get; }
    public string WalletId { get; }
    public string? Note { get; }
}
