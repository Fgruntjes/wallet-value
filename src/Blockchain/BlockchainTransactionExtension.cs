using App.WalletValue.Wallet;
using NBitcoin;

namespace App.WalletValue.Blockchain;

static class BlockchainInfoTransactionExtension
{
    public static long GetTransactionValue(
        this IBlockchainTransaction transaction,
        IList<IWalletAddress> target)
    {
        long value = 0;
        var targetAddressMap = GetAddressMap(target);

        foreach (var input in transaction.Inputs)
        {
            if (targetAddressMap.ContainsKey(input.Address))
            {
                value -= input.Value;
            }
        }

        foreach (var output in transaction.Outputs)
        {
            if (targetAddressMap.ContainsKey(output.Address))
            {
                value += output.Value;
            }
        }

        return value;
    }

    private static IDictionary<string, IWalletAddress> GetAddressMap(IList<IWalletAddress> target)
    {
        var addressMap = new Dictionary<string, IWalletAddress>();

        foreach (var address in target)
        {
            var publicKey = new PubKey(address.PubKey);
            var legacyAddress = publicKey.GetAddress(ScriptPubKeyType.Legacy, Network.Main);
            var segwitAddress = publicKey.GetAddress(ScriptPubKeyType.Segwit, Network.Main);
            var segwitP2SH = publicKey.GetAddress(ScriptPubKeyType.SegwitP2SH, Network.Main);
            var taprootBIP86 = publicKey.GetAddress(ScriptPubKeyType.TaprootBIP86, Network.Main);

            addressMap[legacyAddress.ToString()] = address;
            addressMap[segwitAddress.ToString()] = address;
            addressMap[segwitP2SH.ToString()] = address;
            addressMap[taprootBIP86.ToString()] = address;
        }

        return addressMap;
    }
}