using System.Text.Json.Serialization;

namespace App.WalletValue.Blockchain;

public class BlockchainInfoInput
{
    [JsonPropertyName("prev_out")]
    public BlockchainInfoOutput PreviousOutput { get; set; } = new BlockchainInfoOutput();
}
