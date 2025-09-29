using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MainProject.Attributes;
using MainProject.Services.CustomHelper;
using MainProject.Controllers.CustomSwagger.Services;

namespace MainProject.Controllers.CustomSwagger.Controllers
{
    [ApiController]
    [CustomRoute("[controller]")] // Added RoutePrefix in appsettings.json: bn_maestras/api/v1/
    //[Route("kikes_bionegocios/v1.0/[controller]")] // Example: kdavnotify-api/v1.0/ appsettings.json

    [ApiExplorerSettings(IgnoreApi = true)] // Esto ocultará el controlador en Swagger
    public class CustomSwaggerController : Controller
    {        
        private readonly ICustomHelper _customHelper;
        private readonly ICustomSwagger _customSwaggerService; // define injected object
        private readonly IWebHostEnvironment _env;


        // ===== CUSTOM CONSTRUCTOR
        public CustomSwaggerController(            
            ICustomHelper argcustomHelperService,
            ICustomSwagger argcustomSwaggerService,
            IWebHostEnvironment argenv
        )
        {            
            _customHelper = argcustomHelperService;
            _customSwaggerService = argcustomSwaggerService;
            _env = argenv;

        }




        // GET localhost:5255/bn_maestras/api/v1/CustomSwagger/login
        [HttpGet]
        [Route("login", Name = "SwaggerLoginFrontendRoute")] // Must remove [action] before controller definition
        //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)] // Si activas, no puedes retornar mensaje personalizado

        // IActionResult Explicacion: Permite retornar diferentes tipos de respuestas, como JSON, archivos, redirecciones, etc.
        public async Task<IActionResult> IndexAsync()
        {
            var authResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //Console.WriteLine("aut resull");
            //Console.WriteLine(JsonSerializer.Serialize(authResult.Succeeded));                       

            if (
                authResult.Succeeded
                //User.Identity.IsAuthenticated
                    //&& User.Identity.Name == "jose"
            )
            {
                // Devuelve la información del clima
                //return Ok(new { Forecast = "Sunny", Temperature = 25 });
                return Redirect("/swagger/index.html");
            }
                        
            // Some data to view
            ViewData["ls_backend_url_login"] = Url.RouteUrl("SwaggerLoginBackendRoute");
            ViewData["ls_email_devmode"] = "";
            ViewData["ls_password_devmode"] = "";


            // Nos ahorramos la pereza de escribir las credenciales en ambiente de desarrollo
            if (_env.IsDevelopment())            
            {
                ViewData["ls_email_devmode"] = "swaggerbnmaster@kikes.com.co";
                ViewData["ls_password_devmode"] = "k1k3s";
            }
            

            return View("~/Controllers/CustomSwagger/Views/LoginSwagger/Login.cshtml");

        }




        // POST localhost:5255/bn_maestras/api/v1/CustomSwagger/login
        [HttpPost]        
        [Route("login", Name = "SwaggerLoginBackendRoute")]
        // IActionResult : Permite retornar diferentes tipos de respuestas, como JSON, archivos, redirecciones, etc.
        public async Task<ActionResult> LoginAsync()
        {
            string ls_json = "{}";
            string result = "";

            // Creamos una cookie de session iniciada en el navegador del cliente
            try
            {
                result = await _customSwaggerService.MainCreateCookieLoginDatabaseAsync(ls_json); // Pasamos ls_json para conservar estandar 
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    this.CurrentControllerELN(_customHelper.LineNumber()) + " MainLoginAsync: " + ex.Message);
            }


            // 200 - Le dejamos acceder a vistas swagger
            if (HttpContext.Response.StatusCode == StatusCodes.Status200OK)
            {
                // Redirige a Swagger si las credenciales son correctas
                return Redirect("/swagger/index.html");
            }


            // 404 - Redireccionamos al formulario para un segundo intento de email + password
            if (HttpContext.Response.StatusCode == StatusCodes.Status404NotFound)
            {
                // Si las credenciales son incorrectas, muestra un mensaje de error
                ViewBag.ErrorMessage = "Invalid username or password";
                return View("~/Controllers/CustomSwagger/Views/LoginSwagger/Login.cshtml");
            }
            

            // En caso de error 500 mostramos error
            return this.StatusCode(HttpContext.Response.StatusCode, result); // return 200 response
        }




        // GET localhost:5255/bn_maestras/api/v1/CustomSwagger/logout
        [HttpGet]
        [Route("logout", Name = "SwaggerLogoutFrontendRoute")]
        public async Task<IActionResult> LogoutAsync()
        {
            // Cierra la sesión del usuario eliminando la cookie de autenticación
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);            

            string ls_login_url = Url.RouteUrl("SwaggerLoginBackendRoute") ?? "SwaggerLoginBackendRoute-Not-Found";
            return Redirect(ls_login_url);
        }




        // Private custom helper - ELN means (Error Line Number)
        private string CurrentControllerELN(int li_line_number)
        {
            return "||CustomMaestrasController|| Error line number(" + li_line_number + "): ";
        }




    }
}
