using System.Text.Json.Serialization;

namespace App.WalletValue.Blockchain;

public class BlockchainInfoTransaction
{
    public string Hash { get; set; } = string.Empty;

    [JsonPropertyName("block_height")]
    public int BlockHeight { get; set; }

    public IList<BlockchainInfoInput> Inputs { get; set; } = new List<BlockchainInfoInput>();

    [JsonPropertyName("out")]
    public IList<BlockchainInfoOutput> Outputs { get; set; } = new List<BlockchainInfoOutput>();
}
