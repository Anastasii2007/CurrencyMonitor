namespace CurrencyMonitor.Models;

public class ConversionResponse
{
    public string From { get; set; }
    public string To { get; set; }
    public decimal Amount { get; set; }
    public List<ConversionResult> Results { get; set; } = new();
}
