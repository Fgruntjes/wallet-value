namespace App.WalletValue.Wallet;

class WasabiWalletInfo
{
    public IList<WasabiWalletInfoAddress> HdPubKeys { get; set; } = new List<WasabiWalletInfoAddress>();
}
