namespace CurrencyMonitor.Models.Api;

public class MonoRate
{
    public int currencyCodeA { get; set; }
    public int currencyCodeB { get; set; }
    public long date { get; set; }      
    public decimal? rateBuy { get; set; }
    public decimal? rateSell { get; set; }
    public decimal? rateCross { get; set; }
}