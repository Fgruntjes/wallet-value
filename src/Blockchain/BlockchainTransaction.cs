
namespace App.WalletValue.Blockchain;

class BlockchainTransaction : IBlockchainTransaction
{
    public string Id { get; }

    public DateTime Date { get; }

    public int BlockHeight { get; }

    public IList<IBlockchainTransactionUtxo> Inputs { get; }

    public IList<IBlockchainTransactionUtxo> Outputs { get; }

    public BlockchainTransaction(
        string idHex,
        DateTime date,
        int blockHeight,
        IList<IBlockchainTransactionUtxo> inputs,
        IList<IBlockchainTransactionUtxo> outputs)
    {
        Id = idHex;
        Date = date;
        BlockHeight = blockHeight;
        Inputs = inputs;
        Outputs = outputs;
    }
}