namespace CurrencyMonitor.Models
{
    public class CurrencyComparison
    {
        public string CurrencyCode { get; set; }
        public decimal? NbuRate { get; set; }
        public decimal? PrivatBuy { get; set; }
        public decimal? PrivatSell { get; set; }
        public decimal? MonoBuy { get; set; }
        public decimal? MonoSell { get; set; }
    }

    public class ApiResponse
    {
        public string RequestDate { get; set; }
        public List<CurrencyComparison> Rates { get; set; }
    }
}