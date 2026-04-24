using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CurrencyMonitor.Models.Api;

namespace CurrencyMonitor.Services.Api
{
    public class NbuService
    {
        private HttpClient _httpClient;

        public NbuService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<NbuRate>> GetRatesAsync()
        {
            string url = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json";
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                List<NbuRate> rates = JsonSerializer.Deserialize<List<NbuRate>>(json);
                return rates;
            }
            else
            {
                return new List<NbuRate>();
            }
        }
    }
}