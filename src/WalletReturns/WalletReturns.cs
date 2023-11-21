namespace App.WalletValue.WalletReturns;

class WalletReturns
{
    public DateTime Date { get; set; }

    public decimal ExchangeRate { get; set; }

    public long SatsTotal { get; set; }

    public decimal BtcTotal => (decimal)SatsTotal / 100000000;

    public decimal FiatTotal => BtcTotal * ExchangeRate;

    public List<WalletReturnsRow> Rows { get; set; } = new List<WalletReturnsRow>();

    public decimal TotalBoughtFiat
    {
        get
        {
            decimal moveBufferSats = 0;
            decimal totalBoughtFiat = 0;

            foreach (var row in Rows)
            {
                if (row.Sats > 0)
                {
                    if (moveBufferSats < 0)
                    {
                        if (moveBufferSats + row.Sats > 0)
                        {
                            totalBoughtFiat += (moveBufferSats + row.Sats) / 100000000 * row.ExchangeRate;
                            moveBufferSats = 0;
                        }
                        else
                        {
                            moveBufferSats += row.Sats;
                        }
                    }
                    else
                    {
                        totalBoughtFiat += row.Fiat;
                    }
                }
                else
                {
                    moveBufferSats += row.Sats;
                }
            }

            return totalBoughtFiat;
        }
    }
}
