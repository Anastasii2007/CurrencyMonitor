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

        public async Task<ConversionResponse> ConvertCurrencyAsync(string from, string to, decimal amount)
        {
            var nbuRates = await _nbuService.GetRatesAsync();
            var monoRates = await _monoService.GetRatesAsync();
            var privatRates = await _privatService.GetUnifiedRatesAsync();

            var response = new ConversionResponse
            {
                From = from.ToUpper(),
                To = to.ToUpper(),
                Amount = amount
            };

            var nbuFrom = nbuRates.Find(r => r.cc == from.ToUpper())?.rate;
            var nbuTo = nbuRates.Find(r => r.cc == to.ToUpper())?.rate;
            
            decimal nbuCaltulate = Calculate(from, to, amount, nbuFrom, nbuTo);
            
            response.Results.Add(new ConversionResult
            {
                BankName = "NBU",
                RateFrom = GetRate(from, nbuFrom),
                RateTo = GetRate(to ,nbuTo),
                TotalResult = nbuCaltulate
            });

            int fromCode = nbuRates.Find(r => r.cc == from.ToUpper())?.r030 ?? 0;
            int toCode = nbuRates.Find(r => r.cc == to.ToUpper())?.r030 ?? 0;

            decimal? monoFrom = monoRates.Find(r => r.currencyCodeA == fromCode && r.currencyCodeB == 980)?.rateBuy;
            decimal? monoTo = monoRates.Find(r => r.currencyCodeA == toCode && r.currencyCodeB == 980)?.rateBuy;
            
            decimal monoCaltulate = Calculate(from, to, amount, monoFrom, monoTo);
            
            response.Results.Add(new ConversionResult
            {
                BankName = "Monobank",
                RateFrom = GetRate(from, monoFrom),
                RateTo = GetRate(to ,monoTo),
                TotalResult = monoCaltulate
            });

            decimal? privatFrom = privatRates.Find(r => r.currency == from.ToUpper())?.purchaseRate;
            decimal? privatTo = privatRates.Find(r => r.currency == to.ToUpper())?.purchaseRate;
            
            decimal privateCaltulate = Calculate(from, to, amount, privatFrom, privatTo);
            response.Results.Add(new ConversionResult
            {
                BankName = "PrivatBank",
                RateFrom = GetRate(from, privatFrom),
                RateTo = GetRate(to ,privatTo),
                TotalResult = privateCaltulate
            });
            
            return response;
        }
        private decimal GetRate(string currency, decimal? rate)
        {
            if (currency.ToUpper() == "UAH")
            {
                return 1;
            }

            if (rate.HasValue)
            {
                return rate.Value;
            }

            return 0;
        }
        private decimal Calculate(
            string from,
            string to,
            decimal amount,
            decimal? rateFrom,
            decimal? rateTo)
        {
            decimal effectiveFrom;
            decimal effectiveTo;
            
            if (from.ToUpper() == "UAH")
            {
                effectiveFrom = 1;
            }
            else
            {
                if (rateFrom.HasValue)
                {
                    effectiveFrom = rateFrom.Value;
                }
                else
                {
                    return 0;
                }
            }

            if (to.ToUpper() == "UAH")
            {
                effectiveTo = 1;
            }
            else
            {
                if (rateTo.HasValue)
                {
                    effectiveTo = rateTo.Value;
                }
                else
                {
                    return 0;
                }
            }

            if (effectiveFrom <= 0 || effectiveTo <= 0)
            {
                return 0;
            }

            decimal total = (amount * effectiveFrom) / effectiveTo;
            return Math.Round(total, 2);
        }
    }
}