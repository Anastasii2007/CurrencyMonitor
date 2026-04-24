using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CurrencyMonitor.Models.Api;

namespace CurrencyMonitor.Services.Api
{
    public class PrivatService
    {
        private HttpClient _httpClient;
        private JsonSerializerOptions _jsonOptions;

        public PrivatService()
        {
            _httpClient = new HttpClient();
            _jsonOptions = new JsonSerializerOptions();
            _jsonOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
        }

        public async Task<List<PrivatArchiveRate>> GetUnifiedRatesAsync()
        {
            List<PrivatArchiveRate> allRates = new List<PrivatArchiveRate>();
            
            DateTime today = DateTime.Now;
            string formattedDate = today.ToString("dd.MM.yyyy");
            string archiveUrl = "https://api.privatbank.ua/p24api/exchange_rates?json&date=" + formattedDate;
            
            HttpResponseMessage archiveResponse = await _httpClient.GetAsync(archiveUrl);
            
            if (archiveResponse.IsSuccessStatusCode)
            {
                string archiveJson = await archiveResponse.Content.ReadAsStringAsync();
                PrivatArchiveDto archiveData = JsonSerializer.Deserialize<PrivatArchiveDto>(archiveJson, _jsonOptions);
                
                if (archiveData != null)
                {
                    if (archiveData.exchangeRate != null)
                    {
                        allRates = archiveData.exchangeRate;
                    }
                }
            }

            string cashUrl = "https://api.privatbank.ua/p24api/pubinfo?exchange&coursid=5";
            HttpResponseMessage cashResponse = await _httpClient.GetAsync(cashUrl);
            
            if (cashResponse.IsSuccessStatusCode)
            {
                string cashJson = await cashResponse.Content.ReadAsStringAsync();
                List<PrivatBankyRate> cashRates = JsonSerializer.Deserialize<List<PrivatBankyRate>>(cashJson, _jsonOptions);
                
                if (cashRates != null)
                {
                    foreach (PrivatBankyRate cashItem in cashRates)
                    {
                        PrivatArchiveRate existingRate = allRates.Find(r => r.currency == cashItem.ccy);
                        
                        if (existingRate != null)
                        {
                            existingRate.purchaseRate = cashItem.buy; 
                            existingRate.saleRate = cashItem.sale;    
                        }
                    }
                }
            }

            return allRates;
        }
    }
}