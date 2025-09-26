using MainProject.Attributes;
using MainProject.Controllers.CurrentProject.Models.FnRes;
using MainProject.Controllers.CurrentProject.Services;
using MainProject.Services.CustomHelper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

// Import the System.Collections.Generic namespace for List<T>

namespace MainProject.Controllers.CurrentProject.Controllers
{
    [ApiController]
    //[Authorize] // Oculto porque retornaremos un mensaje de error personalizado desde el service
    //[Route("kikes_bionegocios/v1.0/[controller]")] // Example: kdavnotify-api/v1.0/ appsettings.json
    [CustomRoute("[controller]")] // Example: kdavnotify-api/v1.0/ appsettings.json
    public class CurrentProjectController : ControllerBase
    {        
        private readonly ICurrentProject _currentProjectService; // tiene los metodos de la interfaz principal
        private readonly ICustomHelper _customHelper;

        public CurrentProjectController(            
            ICurrentProject argcurrentProjectService,
            ICustomHelper argcustomHelperService
        )
        {           
            _currentProjectService = argcurrentProjectService;
            _customHelper = argcustomHelperService;
        }



        // POST http://localhost:5206/KikesReceivesDaviAlert/CurrentProject/userList
        [HttpGet]
        [Route("status")] // Must remove [action] before controller definition        
        [SwaggerOperation(Summary = "Consulta si el servicio [web/api] está corriendo y respondiendo.", Description = "")]
        [ProducesResponseType(typeof(ObjFnResMainProjectPublishedDateTime), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]        
        public ActionResult<string> ProjectPublishedDateTime()
        {
            string ls_input_json = "{ \"emptyProperty\": \"some strings or another values\" }";
            string ls_service_json_response = "";
            try
            {                                              
                ls_service_json_response = _currentProjectService.MainProjectPublishedDateTime(ls_input_json);                
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    " CurrentProjectController ProjectPublishedDateTime Error: " + ex.Message);
            }
            
            return StatusCode(HttpContext.Response.StatusCode, ls_service_json_response); // Generalmente es 200
        }




        // localhost:5255/bn_maestras/api/v1/CurrentProject/status_database
        [HttpGet]

        [Route("get_record")]
        [SwaggerOperation(Summary = "Consulta si el servicio [web/api] tiene acceso a la bd.",
            Description = "Obtiene un registro de la base de datos. Los resultados no retornan informacion sensible.")]    
        //[ProducesResponseType(typeof(ObjFnResMainGetUserFromDb), StatusCodes.Status200OK)]

        //public async Task<ActionResult<string>> GetUserFromDb()
        public async Task<ActionResult<string>> GetUserFromDb()
        {
            string ls_json = "{}";
            string result = "";

            try
            {
                result = await _currentProjectService.MainGetUserFromDbAsync(ls_json); // Pasamos ls_json para conservar estandar                

            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    this.CurrentControllerELN(_customHelper.LineNumber()) + " MainGetUserFromDbAsync: " + ex.Message);
            }

            return this.StatusCode(HttpContext.Response.StatusCode, result); // return 200 response
        }


        // Define a custom helper - ELN means (Error Line Number)
        private string CurrentControllerELN(int li_line_number)
        {
            return "||CurrentProjectController|| Error line number(" + li_line_number + "): ";
        }




    }
}
