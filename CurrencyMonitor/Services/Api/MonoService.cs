using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CurrencyMonitor.Models.Api;

namespace CurrencyMonitor.Services.Api
{
    public class MonoService
    {
        private HttpClient _httpClient;

        public MonoService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<MonoRate>> GetRatesAsync()
        {
            string url = "https://api.monobank.ua/bank/currency";
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                List<MonoRate> rates = JsonSerializer.Deserialize<List<MonoRate>>(json);
                return rates;
            }
            else
            {
                return new List<MonoRate>();
            }
        }
    }
}