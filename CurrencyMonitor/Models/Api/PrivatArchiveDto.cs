using System.Collections.Generic;

namespace CurrencyMonitor.Models.Api
{
    public class PrivatArchiveDto
    {
        public string date { get; set; }
        public string bank { get; set; }
        public List<PrivatArchiveRate> exchangeRate { get; set; }
    }
}