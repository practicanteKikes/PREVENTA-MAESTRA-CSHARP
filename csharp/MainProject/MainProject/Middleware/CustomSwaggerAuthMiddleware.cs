using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace MainProject.Middleware
{
    public class CustomSwaggerAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public CustomSwaggerAuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {            
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                // Validate cookie
                var authResult = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                // User is logged by cookie - then continue
                if (authResult.Succeeded)
                {
                    await _next(context); // Continua con el siguiente middleware
                    return; // End of current code middleware
                }

                string ls_base_uri = _configuration.GetValue<string>("CustomApp:RoutePrefix");
                string ls_redirect = $"/{ls_base_uri}CustomSwagger/login";
                context.Response.Redirect(ls_redirect);
                return;
            }

            // Para las demás rutas simplemente continuamos
            await _next(context); // Continua con el siguiente middleware
            // return; // aqui es opcional ya que no existe mas codigo despues de esta linea
        }
    }
}
