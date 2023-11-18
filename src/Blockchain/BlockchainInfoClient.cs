using System.Net.Http.Json;
using CacheManager.Core;

namespace App.WalletValue.Blockchain;

class BlockchainInfoClient : IBlockchainClient
{
    private readonly HttpClient _httpClient;
    private readonly BaseCacheManager<BlockchainInfoTransaction> _transactionInfoCache;
    private readonly BaseCacheManager<BlockchainInfoBlockResult> _blockInfoCache;

    public BlockchainInfoClient(HttpClient httpClient, ICacheManagerConfiguration cacheConfig)
    {
        _httpClient = httpClient;
        _transactionInfoCache = new BaseCacheManager<BlockchainInfoTransaction>(cacheConfig);
        _blockInfoCache = new BaseCacheManager<BlockchainInfoBlockResult>(cacheConfig);
    }

    public async Task<IBlockchainTransaction> GetTransactionAsync(string transactionId, CancellationToken cancellationToken = default)
    {
        var transaction = await FetchTransactionAsync(transactionId, cancellationToken);
        var blockInfo = await FetchBlockAsync(transaction.BlockHeight, cancellationToken);
        var transactionDate = DateTimeOffset.FromUnixTimeSeconds(blockInfo.Time).DateTime;

        var inputs = transaction.Inputs
            .Select(input => input.PreviousOutput as IBlockchainTransactionUtxo)
            .ToList();

        var outputs = transaction.Outputs
            .Select(output => output as IBlockchainTransactionUtxo)
            .ToList();

        return new BlockchainTransaction(
            transactionId,
            transactionDate,
            transaction.BlockHeight,
            inputs,
            outputs);
    }

    private async Task<BlockchainInfoTransaction> FetchTransactionAsync(string transactionId, CancellationToken cancellationToken)
    {
        var cacheKey = $"bcinfo:transaction:{transactionId}";
        if (_transactionInfoCache.Exists(cacheKey))
        {
            return _transactionInfoCache.Get(cacheKey);
        }

        var transaction = await _httpClient.GetFromJsonAsync<BlockchainInfoTransaction>($"/rawtx/{transactionId}", cancellationToken)
            ?? throw new Exception($"Transaction {transactionId} not found");

        _transactionInfoCache.Add(new CacheItem<BlockchainInfoTransaction>(cacheKey, transaction).WithNoExpiration());
        return transaction;
    }

    private async Task<BlockchainInfoBlockResult> FetchBlockAsync(long blockIndex, CancellationToken cancellationToken)
    {
        var cacheKey = $"bcinfo:block:{blockIndex}";
        if (_blockInfoCache.Exists(cacheKey))
        {
            return _blockInfoCache.Get(cacheKey);
        }

        var blockInfo = await _httpClient.GetFromJsonAsync<BlockchainInfoBlockResult>($"/rawblock/{blockIndex}", cancellationToken)
            ?? throw new Exception($"Block {blockIndex} not found");

        _blockInfoCache.Add(new CacheItem<BlockchainInfoBlockResult>(cacheKey, blockInfo).WithNoExpiration());
        return blockInfo;
    }
}
