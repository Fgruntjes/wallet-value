using System.Text.Json.Serialization;

namespace App.WalletValue.Exchange;

class CoinGeckoCoinHistoryResponseMarketData
{
    [JsonPropertyName("current_price")]
    public IDictionary<string, decimal> CurrentPrice { get; set; } = new Dictionary<string, decimal>();
}