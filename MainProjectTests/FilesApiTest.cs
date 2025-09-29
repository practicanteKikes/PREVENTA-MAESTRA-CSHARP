using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;

namespace MainProjectTests
{
    public class FilesApiTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public FilesApiTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient(); // Inicializamos cliente or browser
        }


        // PRIMER TEST - Este test es para los developers.
        [Fact]
        public async Task GET_retrieves_weather_forecast()
        {
            HttpResponseMessage response = await _client.GetAsync("/weatherforecast");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            if (response.IsSuccessStatusCode) // ERROR IF NOT "exitosamente"
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                responseBody.Should().Contain("TemperatureC"); // Assert that the response contains the string "exitosamente"                
            }
        }


        

    }
}
