using System.Net;
using App.WalletValue.Blockchain;
using App.WalletValue.Exchange;
using App.WalletValue.Wallet;
using App.WalletValue.WalletReturns;
using CacheManager.Core;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace App.WalletValue;

static class AppServiceProvider
{
    public static IServiceProvider Build()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddScoped<IWallet, WasabiWallet>();
        services.AddScoped<IBlockchainClient, BlockchainInfoClient>();
        services.AddScoped<WalletReturnsService>();
        services.AddBlockChainInfoClient();
        services.AddCoinGeckoClient();
        services.AddDiskCache();

        return services.BuildServiceProvider();
    }

    private static void AddBlockChainInfoClient(this ServiceCollection services)
    {
        services.AddHttpClient<IBlockchainClient, BlockchainInfoClient>(client =>
        {
            client.BaseAddress = new Uri("https://blockchain.info/");
        })
            .AddPolicyHandler(GetRateLimitPolicy())
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());
    }

    private static void AddCoinGeckoClient(this ServiceCollection services)
    {
        services.AddHttpClient<IExchangeClient, CoinGeckoExchangeClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.coingecko.com/");
        })
            .AddPolicyHandler(GetRateLimitPolicy())
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRateLimitPolicy()
    {
        return Policy
            .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 1,
                sleepDurationProvider: (retryAttempt, response, context) =>
                {
                    var retryAfter = response.Result?.Headers?.RetryAfter?.Delta;
                    if (retryAfter is null)
                    {
                        return TimeSpan.FromSeconds(1);
                    }

                    return retryAfter.Value;
                },
                onRetryAsync: (response, timespan, retryAttempt, context) => Task.CompletedTask
            );
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
    }

    private static void AddDiskCache(this ServiceCollection services)
    {
        services.AddSingleton(services =>
        {
            return new ConfigurationBuilder()
                .WithJsonSerializer()
                .WithFileCacheHandle(PathUtils.Resolve("~/.cache/walletvalue"))
                .Build();
        });
    }
}
