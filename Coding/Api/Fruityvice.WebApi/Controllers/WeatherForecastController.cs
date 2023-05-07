using Fruityvice.Data;
using Fruityvice.Service;
using Microsoft.AspNetCore.Mvc;

namespace Fruityvice.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IFruityviceService _fruityviceService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
            IFruityviceService fruityviceService)
        {
            _logger = logger;
            _fruityviceService = fruityviceService;
        }

        [HttpGet]
        public async Task<List<Fruit>> Get()
        {
            return await _fruityviceService.GetAllFruit(0, 100);
        }

        //[HttpGet]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = Random.Shared.Next(-20, 55),
        //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}
    }
}