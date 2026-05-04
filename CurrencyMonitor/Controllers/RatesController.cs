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
        
        [HttpGet("convert")]
        public async Task<ActionResult<ConversionResponse>> Convert(
            [FromQuery] string from = "USD", 
            [FromQuery] string to = "UAH", 
            [FromQuery] decimal amount = 100)
        {
            if (amount <= 0)
            {
                return BadRequest("Сума має бути більшою за нуль.");
            }

            var result = await _aggregator.ConvertCurrencyAsync(from, to, amount);

            if (result.Results.Count == 0)
            {
                Console.WriteLine("Абіба");
                return NotFound("Не вдалося знайти курси для вказаних валют у жодному банку.");
            }
            return Ok(result);
        }
    }
}