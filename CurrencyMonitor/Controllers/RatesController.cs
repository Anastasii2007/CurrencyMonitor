using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyMonitor.Models;
using CurrencyMonitor.Services;
using System;

namespace CurrencyMonitor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatesController : ControllerBase
    {
        private CurrencyAggregatorService _aggregator;

        public RatesController(CurrencyAggregatorService aggregator)
        {
            _aggregator = aggregator;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetRates([FromQuery] List<string> currencies)
        {
            List<string> searchList = currencies;
            
            if (searchList == null || searchList.Count == 0)
            {
                searchList = new List<string>(); 
            }

            List<CurrencyComparison> rates = await _aggregator.GetRatesAsync(searchList);

            ApiResponse response = new ApiResponse();
            response.RequestDate = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            response.Rates = rates;

            return Ok(response);
        }
    }
}