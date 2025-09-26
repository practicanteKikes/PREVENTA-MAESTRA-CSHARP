using MainProject.Attributes;
using MainProject.Controllers.CustomJwt.Services;
using Microsoft.AspNetCore.Mvc; // Required on create new controller
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace MainProject.Controllers.CustomJwt.Controllers
{
    // ===== SETTINGS BEFORE DECLARE CONTROLLER
    // servidor-cliente-api
    [ApiController]
    //[Route("{anyword1}/{anyword2}/[controller]")] // Example: kdavnotify-api/v1.0/ appsettings.json
    [CustomRoute("[controller]")] // Route prefix example "kdavnotify-api/v1.0/" defined in appsettings.json



    // ===== CREATING CONTROLLER
    public class CustomJwtController : ControllerBase
    {
        // DEFINE INTERNAL PROPERTIES OF THIS CLASS        
        private readonly ICustomJwt _customJwt; // define injected object

        // DEFINE CONSTRUCTOR AND INITIALIZE PROPERTIES
        public CustomJwtController(ICustomJwt argCustomJwt)
        {
            _customJwt = argCustomJwt;
        }




        // ===== CREATE ENDPOINTS
        // =====




        // FAKE ENDPOINT: Nos dice si un token es valido (no realiza busqueda en base de datos) y si tiene parametro id mayor que cero
        // GET localhost:5255/bn_maestras/api/v1/CustomJwt/TokenStatus
        //[HttpPost]
        //[Route("token_status")] 
        //[ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public ActionResult<string> TokenStatus()
        //{
        //    try
        //    {
        //        var result = _customJwt.TokenStatus();
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}




        // FAKE ENDPOINT: Creamos un token con un array de datos (se simula resultados de una consulta)
        // POST http://localhost:5206/kikesdav-notify-apiv1.0/CustomJwt/TokenCreate
        //[HttpPost]
        //[Route("token_create")]
        //[ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]

        //public ActionResult<string> TokenCreate(object paramPostmanObj)
        //{
        //    // Required line beacuse ToString by definition may be result in null
        //    string? ls_json_or_null = paramPostmanObj.ToString();
        //    string ls_json = ls_json_or_null ?? "";

        //    try
        //    {
        //        var result = _customJwt.TokenCreate(ls_json);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}



        // FAKE ENDPOINT: Simula los resultados de consulta de bd
        // GET http://localhost:5206/kikesdav-notify-apiv1.0/CustomJwt/EntityUsers
        //[HttpPost]
        //[Route("entity_users")]
        //[ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]

        //public ActionResult<string> EntityUsers()
        //{
        //    try
        //    {
        //        var result = _customJwt.EntityUsers();
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}




        // POST localhost:5206/bn_maestras/api/v1/CustomJwt/get_token
        [HttpPost]
        [Route("create_token")]
        [SwaggerOperation(Summary = "Permite obtener un Token JWT. [Por ahora solo disponible en POSTMAN]", Description = "")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<string> TokenCreateDatabase()
        {


            // Read variables from header and create json structure
            string ls_aux_json = "";
            try
            {
                ls_aux_json = _customJwt.ReadUserPasswordFromHeaders();

                // Incoming request have any error?? then bad request
                switch (HttpContext.Response.StatusCode)
                {
                    case (int)HttpStatusCode.BadRequest:
                        return BadRequest(ls_aux_json); // Bad request (400)
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    " CusJwtController ReadUserPassHeaders: " + ex.Message);
            }




            // With json structure create token searching in db
            string result = "";
            try
            {
                result = _customJwt.TokenCreateDatabase(ls_aux_json);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    " CusJwtController TokenCreateDatabase: " + ex.Message);
            }


            // All ok to here
            return Ok(result); // return 200 response
        }




        // GET http://localhost:5206/kikesdav-notify-apiv1.0/CustomJwt/TokenStatusDatabase
        [HttpPost]
        [Route("tokenStatusDatabase")] // Server dell no importa mayusculas
        [SwaggerOperation(Summary = "Permite consultar si hay algun error en el Token JWT generado anteriormente.", Description = "")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public ActionResult<string> TokenStatusDatabase()
        {
            try
            {
                var result = _customJwt.TokenStatusDatabase();
                //Console.WriteLine("||CustomJwtController|| TokenStatusDatabase - status code after executed service: " + HttpContext.Response.StatusCode);

                switch (HttpContext.Response.StatusCode)
                {
                    case (int)HttpStatusCode.Unauthorized:
                        {
                            return Unauthorized(result);
                        }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }




        




    }
}
