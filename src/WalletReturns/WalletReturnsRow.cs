namespace App.WalletValue.WalletReturns;

class WalletReturnsRow
{
    public string Id { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public int BlockHeight { get; set; }

    public string? Note { get; set; }

    public long Sats { get; set; }
    public decimal Btc => (decimal)Sats / 100000000;

    public decimal ExchangeRate { get; set; }

    public decimal Fiat => Btc * ExchangeRate;

    public long SatsTotal { get; set; }

    public decimal BtcTotal => (decimal)SatsTotal / 100000000;

    public decimal FiatTotal => BtcTotal * ExchangeRate;
}