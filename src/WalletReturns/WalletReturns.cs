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
            decimal moveBuffer = 0;
            decimal totalBoughtFiat = 0;

            foreach (var row in Rows)
            {
                var rowFiat = row.Fiat;
                if (rowFiat > 0)
                {
                    if (moveBuffer < 0)
                    {
                        if (moveBuffer + rowFiat > 0)
                        {
                            totalBoughtFiat += moveBuffer + rowFiat;
                            moveBuffer = 0;
                        }
                        else
                        {
                            moveBuffer += rowFiat;
                        }
                    }
                    else
                    {
                        totalBoughtFiat += rowFiat;
                    }
                }
                else
                {
                    moveBuffer += rowFiat;
                }
            }

            return totalBoughtFiat;
        }
    }
}
