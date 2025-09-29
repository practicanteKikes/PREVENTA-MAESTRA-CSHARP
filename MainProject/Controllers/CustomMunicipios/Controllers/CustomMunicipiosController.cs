using MainProject.Attributes;
using MainProject.Controllers.CustomMunicipios.Models.MunicipiosAll.MainGetMunicipiosAllAsync.FnRes;
using MainProject.Controllers.CustomMunicipios.Models.MunicipiosFiltered.MainGetMunicipiosFilteredAsync;
using MainProject.Controllers.CustomMunicipios.Services;
using MainProject.Services.CustomHelper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MainProject.Controllers.CustomMunicipios.Controllers
{
    [ApiController]

    // Funciona con swagger
    [CustomRoute("[controller]")] // Ruta con prefijo usando Attributes appsettings.json: RoutePrefix = bn_maestras/api/v1/
    public class CustomMunicipiosController : ControllerBase
    {
        private readonly ICustomHelper _customHelper;
        private readonly ICustomMunicipios _customMunicipios; // define injected object

        // calculated
        private string _ls_calculated_controller_name = "";

        public CustomMunicipiosController(
            ICustomHelper argcustomHelperService,
            ICustomMunicipios argcustomMunicipiosService
        )
        {
            _customHelper = argcustomHelperService;
            _customMunicipios = argcustomMunicipiosService;
            _ls_calculated_controller_name = this.GetType().Name; // controller name
        }


        // Define a custom helper - ELN means (Error Line Number)
        private string CurrentControllerELN(int li_line_number)
        {
            return $"||{_ls_calculated_controller_name}|| Error line number(" + li_line_number + "): ";
        }


        // http://localhost:5255/bn_maestras/api/v1/CustomMunicipios/municipios
        [HttpGet]
        [Route("municipios")] // Must remove [action] before controller definition
        [SwaggerOperation(Summary = "Permite obtener la lista de |municipios| de forma filtrada y paginada.", Description = "")]
        [ProducesResponseType(typeof(FnResMainGetMunicipiosFiltered), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetMunicipiosFiltered(
              [FromQuery] int? limit = 2
            , [FromQuery] string? fec_registro_min = "1899-12-31" // 1900-12-31
            , [FromQuery] int? after_id = 0
        //, [FromQuery] string? zona = ""
        )
        {

            string ls_json = "{}";
            string result = "";

            try
            {
                result = await _customMunicipios.MainGetMunicipiosFilteredAsync(ls_json); // Pasamos ls_json para conservar estandar                

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    this.CurrentControllerELN(_customHelper.LineNumber()) + " MainGetMunicipiosFilteredAsync: " + ex.Message);
            }

            return StatusCode(HttpContext.Response.StatusCode, result); // return 200 response
        }





        // http://localhost:5255/bn_maestras/api/v1/CustomMunicipios/municipios_all
        [HttpGet]
        [Route("municipios_all")] // Must remove [action] before controller definition
        [SwaggerOperation(Summary = "[Prototipo] Permite obtener la lista de |municipios| de forma completa y sin paginar.", Description = "")]
        [ProducesResponseType(typeof(FnResMainGetMunicipiosAll), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetMunicipiosAll()
        {
            string ls_json = "{}";
            string result = "";

            try
            {
                result = await _customMunicipios.MainGetMunicipiosAllAsync(ls_json); // Pasamos ls_json para conservar estandar
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    this.CurrentControllerELN(_customHelper.LineNumber()) + " MainGetMunicipiosAllAsync: " + ex.Message);
            }

            return StatusCode(HttpContext.Response.StatusCode, result); // return 200 response
        }
    }
}
