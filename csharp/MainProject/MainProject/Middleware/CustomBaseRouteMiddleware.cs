namespace MainProject.Middleware
{
    // Middleware usado para rechazar aquellas url que no empiecen con: /kdavnotify-api/v1.0 - Definidas en appsettings.json
    // Evalumos y rechazamos aquellas rutas que contengan controllerName como 4 parametro: /api/version/controllerName
    // recuerda que las rutas siempre se reciben desde "/ruta/algomas/desdeaquievaluamos"
    public class CustomBaseRouteMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string baseRoute;
        private readonly string apiVersion;
        private IConfiguration _configuration;

        public CustomBaseRouteMiddleware(
            RequestDelegate next,
            IConfiguration argconfiguration)
        {
            _configuration = argconfiguration;
            this.baseRoute = _configuration["CustomApp:CustomBaseRoute"] ?? "kikes-externalprovider-api";
            this.apiVersion = _configuration["CustomApp:ApiVersion"] ?? "v1.0";

            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Your custom logic before the controller and action
            var requestPath = context.Request.Path;

            var pathSegments = requestPath.Value?.Split('/'); // "/kdavnotify-api/v1.0/CustomValidator/userList"
            //Console.WriteLine("tamanio segmentos: " + pathSegments.Count());


            // We Dont evaluate with default route like /WeatherForecast
            // We Dont evaluate with default route like /WeatherForecast/secondAction            

            // Evaluate when in these cases; // /"api"/"v1.0"/controller
            // Evaluate when in these cases; // /"api"/"v1.0"/controller/action           
            if (pathSegments is not null && pathSegments.Count() >= 4)
            {
                // Extract anyword0, anyword1 and anyword2 from the request path
                var anyword0 = pathSegments?.Length > 0 ? pathSegments[0] : null; // First param is empty string ""
                var anyword1 = pathSegments?.Length > 1 ? pathSegments[1] : null; // "apiname"
                var anyword2 = pathSegments?.Length > 2 ? pathSegments[2] : null; // "v1.0"

                // apiName validation required!
                if (anyword1 != this.baseRoute)
                {
                    //Console.WriteLine(anyword1 +" no existe base" + this.baseRoute); // queda grabada a fuego al compilar                    
                    context.Response.StatusCode = 404; // Set 404 status code                    
                    return; // Complete the request
                }

                // version api validation required!
                if (anyword2 != this.apiVersion)
                {
                    //Console.WriteLine(anyword2 + " no existe version: " + this.apiVersion); // queda grabada a fuego al compilar                    
                    context.Response.StatusCode = 404; // Set 404 status code                  
                    return; // Complete the request
                }
            }

            await _next(context);

            // Your custom logic continue to controller and action
        }

    }

    // Extension method to add the middleware to the pipeline
    public static class MyCustomMiddlewareExtensions
    {
        public static IApplicationBuilder UseMyCustomBaseRouteMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomBaseRouteMiddleware>();
        }
    }

}
