using System.Text.Json.Serialization;

namespace App.WalletValue.Blockchain;

public class BlockchainInfoOutput : IBlockchainTransactionUtxo
{
    [JsonPropertyName("addr")]
    public string Address { get; set; } = string.Empty;

    public long Value { get; set; }
}
