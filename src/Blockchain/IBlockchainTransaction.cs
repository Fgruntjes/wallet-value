namespace App.WalletValue.Blockchain;

public interface IBlockchainTransaction
{
    public string Id { get; }
    public DateTime Date { get; }
    public int BlockHeight { get; }
    public IList<IBlockchainTransactionUtxo> Inputs { get; }
    public IList<IBlockchainTransactionUtxo> Outputs { get; }
}
