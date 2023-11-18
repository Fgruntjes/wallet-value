namespace App.WalletValue.Blockchain;

public interface IBlockchainTransactionUtxo
{
    public string Address { get; }
    public long Value { get; }
}
