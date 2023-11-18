using System.Net.Http.Json;
using CacheManager.Core;

namespace App.WalletValue.Exchange;

class CoinGeckoExchangeClient : IExchangeClient
{
    private readonly HttpClient _httpClient;
    private readonly BaseCacheManager<decimal> _exchangeRateCache;

    public CoinGeckoExchangeClient(
        HttpClient httpClient,
        ICacheManagerConfiguration cacheConfig)
    {
        _httpClient = httpClient;
        _exchangeRateCache = new BaseCacheManager<decimal>(cacheConfig);
    }

    public async Task<decimal> GetExchangeRateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"coingecko:exchangerate:{date:yyyy-MM-dd}";
        if (_exchangeRateCache.Exists(cacheKey))
        {
            return _exchangeRateCache.Get(cacheKey);
        }

        var url = $"/api/v3/coins/bitcoin/history?date={date:dd-MM-yyyy}&localization=false";
        var history = await _httpClient.GetFromJsonAsync<CoinGeckoCoinHistoryResponse>(url, cancellationToken)
            ?? throw new Exception($"Failed fetching bitcoin history");
        var value = history.MarketData.CurrentPrice["usd"];

        _exchangeRateCache.Add(new CacheItem<decimal>(cacheKey, value).WithNoExpiration());
        return value;
    }
}
