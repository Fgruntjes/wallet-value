using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Logging;

namespace App.WalletValue.Wallet;

class WasabiWallet : IWallet
{
    private readonly string _walletPath;
    private readonly ILogger<WasabiWallet> _logger;

    public WasabiWallet(ILoggerFactory logger, string walletPath = "~/.walletwasabi")
    {
        _walletPath = PathUtils.Resolve(walletPath);
        _logger = logger.CreateLogger<WasabiWallet>();
    }

    public async IAsyncEnumerable<IWalletTransaction> GetTransactionsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var transactionFile in WalletTransactionFiles())
        {
            await foreach (var transaction in ReadTransactionsAsync(transactionFile, cancellationToken))
            {
                yield return transaction;
            }
        }
    }

    public async IAsyncEnumerable<IWalletAddress> GetAddressesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var addressFile in WalletAddressFiles())
        {
            var wallet = await ReadWalletAsync(addressFile, cancellationToken);
            foreach (var pubKey in wallet.HdPubKeys)
            {
                yield return new WalletAddress(pubKey.PubKey);
            }
        }
    }

    private IEnumerable<string> WalletTransactionFiles()
    {
        var matcher = new Matcher();
        matcher.AddIncludePatterns(new[] { "*/Transactions.dat" });

        return matcher.GetResultsInFullPath($"{_walletPath}/client/BitcoinStore/Main/ConfirmedTransactions");
    }

    private async IAsyncEnumerable<IWalletTransaction> ReadTransactionsAsync(
        string transactionFile,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var allLines = await File.ReadAllLinesAsync(transactionFile, cancellationToken);
        foreach (var line in allLines)
        {
            var transaction = ParseTransaction(line);
            if (transaction != null)
            {
                yield return transaction;
            }
        }
    }

    private IWalletTransaction? ParseTransaction(string line)
    {
        // From https://github.com/zkSNACKs/WalletWasabi/blob/24843a7840e05056077ad46664a48f62ffb70c4f/WalletWasabi/Blockchain/Transactions/SmartTransaction.cs#L538
        var parts = line.Split(':', StringSplitOptions.None).Select(x => x.Trim()).ToArray();
        if (line.Length < 6)
        {
            _logger.LogInformation("Skipping line '{line}', too short.", line);
            return null;
        }

        var transactionString = parts[0];
        var labelString = parts[5];

        return new WalletTransaction(transactionString, transactionString, labelString);
    }

    private IEnumerable<string> WalletAddressFiles()
    {
        var matcher = new Matcher();
        matcher.AddIncludePatterns(new[] { "*.json" });

        return matcher.GetResultsInFullPath($"{_walletPath}/client/Wallets");
    }

    private static async Task<WasabiWalletInfo> ReadWalletAsync(string walletFile, CancellationToken cancellationToken)
    {
        var fileStream = File.OpenRead(walletFile);
        var parseOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var walletObject = await JsonSerializer.DeserializeAsync<WasabiWalletInfo>(
            fileStream,
            parseOptions,
            cancellationToken);

        return walletObject ?? throw new Exception($"Failed to deserialize wallet file {walletFile}");
    }
}
