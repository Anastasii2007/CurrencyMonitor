using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyMonitor.Models;
using CurrencyMonitor.Models.Api;
using CurrencyMonitor.Services.Api;

namespace CurrencyMonitor.Services
{
    public class CurrencyAggregatorService
    {
        private readonly NbuService _nbuService;
        private readonly MonoService _monoService;
        private readonly PrivatService _privatService;

        public CurrencyAggregatorService(
            NbuService nbuService, 
            MonoService monoService, 
            PrivatService privatService)
        {
            _nbuService = nbuService;
            _monoService = monoService;
            _privatService = privatService;
        }

        public async Task<List<CurrencyComparison>> GetRatesAsync(List<string> userCurrencies)
        {
            List<NbuRate> nbuRates = await _nbuService.GetRatesAsync();
            List<MonoRate> monoRates = await _monoService.GetRatesAsync();
            List<PrivatArchiveRate> privatRates = await _privatService.GetUnifiedRatesAsync();

            List<string> codes;

            if (userCurrencies != null && userCurrencies.Count > 0)
            {
                codes = userCurrencies;
            }
            else
            {
                codes = new List<string>();
                foreach (NbuRate rate in nbuRates)
                {
                    codes.Add(rate.cc);
                }
            }
            

            List<CurrencyComparison> result = new List<CurrencyComparison>();

            int isoCode;
            foreach (string currency in codes)
            {
                CurrencyComparison comparison = new CurrencyComparison();
                comparison.CurrencyCode = currency;

                NbuRate nbuMatch = nbuRates.Find(r => r.cc == currency);
                if (nbuMatch != null)
                {
                    comparison.NbuRate = nbuMatch.rate;
                    isoCode = nbuMatch.r030; 

                    MonoRate monoMatch = monoRates.Find(r => r.currencyCodeA == isoCode && r.currencyCodeB == 980);
                    if (monoMatch != null)
                    {
                        comparison.MonoBuy = monoMatch.rateBuy > 0 ? monoMatch.rateBuy : null;
                        comparison.MonoSell = monoMatch.rateSell > 0 ? monoMatch.rateSell : null;
                    }
                }

                PrivatArchiveRate privatMatch = privatRates.Find(r => r.currency == currency);
                if (privatMatch != null)
                {
                    comparison.PrivatBuy = privatMatch.purchaseRate > 0 ? privatMatch.purchaseRate : null;
                    comparison.PrivatSell = privatMatch.saleRate > 0 ? privatMatch.saleRate : null;
                }

                if (comparison.NbuRate == null && comparison.PrivatBuy == null && comparison.MonoBuy == null)
                {
                    continue; 
                }

                result.Add(comparison);
            }

            return result;
        }
    }
}