namespace CurrencyMonitor.Models.Api;

public class PrivatBankyRate
{
	public string ccy { get; set; }
	public string base_ccy { get; set; }
	public decimal buy { get; set; }
	public decimal sale { get; set; }
}
