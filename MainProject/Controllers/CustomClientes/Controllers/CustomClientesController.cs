using MainProject.Attributes;
using MainProject.Controllers.CustomClientes.Models.GetClientesAll.MainGetClientesAllAzync.FnRes;
using MainProject.Controllers.CustomClientes.Models.GetClientesFiltered.MainGetClientesFiltered.FnRes;
using MainProject.Controllers.CustomClientes.Services;
using MainProject.Services.CustomHelper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;


namespace MainProject.Controllers.CustomClientes.Controllers
{

    [ApiController]
    //[Authorize] // Oculto porque retornaremos un mensaje de error personalizado desde el service


    // Para swagger no es util
    // [Route("{anyword1}/{anyword2}/[controller]")] // Activate middleware de las rutas en program - Example: kdavnotify-api/v1.0/ appsettings.json

    // Require trabajo manual en cada controlador
    //[Route("kikes_bionegocios/v1.0/[controller]")] // Ruta escrita a mano: kdavnotify-api/v1.0/ appsettings.json    

    // Funciona con swagger
    [CustomRoute("[controller]")] // Ruta con prefijo usando Attributes appsettings.json: RoutePrefix = bn_maestras/api/v1/

    public class CustomClientesController : ControllerBase
    {
        // injected
        private readonly ICustomHelper _customHelper;
        private readonly ICustomClientes _customMaestrasService; // define injected object

        // calculated
        private string _ls_calculated_controller_name = "";


        // ========== CONSTRUCTOR ========= //
        public CustomClientesController(
            ICustomHelper argcustomHelperService,
            ICustomClientes argcustomFileManager
        )
        {
            _customHelper = argcustomHelperService;
            _customMaestrasService = argcustomFileManager;
            _ls_calculated_controller_name = this.GetType().Name; // controller name
        }




        // http://localhost:5255/kikes_bionegocios/v1.0/CustomMaestras/clientes
        [HttpGet]

        [Route("clientes")] // Must remove [action] before controller definition
        [SwaggerOperation(Summary = "Permite obtener la lista de clientes de forma filtrada y paginada.", Description = "")]
        
        [ProducesResponseType(typeof(ObjFnResMainGetClientesFiltered), StatusCodes.Status200OK)]

        //public async Task<ActionResult<string>> GetClientesFiltered()
        public async Task<ActionResult<string>> GetClientesFiltered(
            [FromQuery] int? limit = 2,
            [FromQuery] string? fec_registro_min = "1899-12-31", // 1900-12-31
            [FromQuery] int? after_id = 0,
            [FromQuery] string? zona = ""
        )
        {

            string ls_json = "{}";
            string result = "";

            try
            {
                result = await _customMaestrasService.MainGetClientesFilteredAsync(ls_json); // Pasamos ls_json para conservar estandar                

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CurrentControllerELN(_customHelper.LineNumber()) + " MainGetClientesFilteredAsync: " + ex.Message);
            }

            return StatusCode(HttpContext.Response.StatusCode, result); // return 200 response
        }


        // http://localhost:5255/kikes_bionegocios/v1.0/CustomMaestras/clientes_all
        [HttpGet]
        [Route("clientes_all")] // Must remove [action] before controller definition
        [SwaggerOperation(Summary = "[Prototipo] Permite obtener la lista de clientes de forma completa y sin paginar.", Description = "")]        
        [ProducesResponseType(typeof(ObjFnResMainGetClientesAll), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetClientesAll()
        {
            string ls_json = "{}";
            string result = "";

            try
            {
                result = await _customMaestrasService.MainGetClientesAllAsync(ls_json); // Pasamos ls_json para conservar estandar
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CurrentControllerELN(_customHelper.LineNumber()) + " MainGetClientesAllAsync: " + ex.Message);
            }

            return StatusCode(HttpContext.Response.StatusCode, result); // return 200 response
        }


        // Define a custom helper - ELN means (Error Line Number)
        private string CurrentControllerELN(int li_line_number)
        {
            return $"||{_ls_calculated_controller_name}|| Error line number(" + li_line_number + "): ";
        }
    }
}
