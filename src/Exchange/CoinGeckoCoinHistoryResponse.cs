using System.Text.Json.Serialization;

namespace App.WalletValue.Exchange;

class CoinGeckoCoinHistoryResponse
{
    [JsonPropertyName("market_data")]
    public CoinGeckoCoinHistoryResponseMarketData MarketData { get; set; } = default!;
}
