namespace App.WalletValue.Wallet;

class WalletAddress : IWalletAddress
{
    public string PubKey { get; }

    public WalletAddress(string pubKey)
    {
        PubKey = pubKey;
    }
}