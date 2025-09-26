using MainProject.Controllers.CustomWeatherForecast.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MainProject.Controllers.CustomWeatherForecast
{
    // Dado que no tiene una ruta personalizada definida ej: [CustomRoute("[controller]")] // api/v99/Controlador/Action
    // Partimos desde el controlador ej "dominio:puerto/ControllerName"
    // Y por defecto retorna el primer metodo GET de respuesta. Retorna dicho metodo como principal


    [ApiController]
    [Route("[controller]")] // Acceso por controlador y retorn default primer method GET
    public class WeatherForecastController : ControllerBase
    {
        // PRIVATE PROPERTIES
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly ILogger<WeatherForecastController> _logger;


        // CUSTOM CONSTRUCTOR
        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }




        // localhost:5255/WeatherForecast
        [HttpGet]
        [SwaggerOperation(Summary = "Endpoint raiz que indica que el servicio [web/api] está corriendo.", Description = "")]
        public IEnumerable<WeatherForecast> Get()
        {            
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }




    }
}
