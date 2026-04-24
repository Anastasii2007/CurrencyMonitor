namespace CurrencyMonitor.Models.Api;

public class PrivatArchiveRate
{
    public string baseCurrency{get; set;}
    public string currency{get; set;}
    public decimal saleRateNB{get; set;}
    public decimal purchaseRateNB{get; set;}
    public decimal saleRate{get; set;}
    public decimal purchaseRate{get; set;}
    
}