namespace CurrencyMonitor.Models;

public class ConversionResult
{
    public string BankName { get; set; }
    public decimal RateFrom { get; set; }
    public decimal RateTo { get; set; }
    public decimal TotalResult { get; set; }
}