using MainProject.Attributes;
using MainProject.Controllers.CustomTiposNegocio.Models.GetTiposNegocioAll.MainGetTiposNegocioAllAsync.FnRes;
using MainProject.Controllers.CustomTiposNegocio.Models.GetTiposNegocioFiltered.MainGetTiposNegocioFiltered.FnRes;
using MainProject.Controllers.CustomTiposNegocio.Services;
using MainProject.Services.CustomHelper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MainProject.Controllers.CustomTiposNegocio.Controllers
{
    [ApiController]

    // Funciona con swagger
    [CustomRoute("[controller]")] // Ruta con prefijo usando Attributes appsettings.json: RoutePrefix = bn_maestras/api/v1/
    public class CustomTiposNegocioController : ControllerBase
    {        
        private readonly ICustomHelper _customHelper;
        private readonly ICustomTiposNegocio _customTiposNegocio; // define injected object

        // calculated
        private string _ls_calculated_controller_name = "";

        public CustomTiposNegocioController(
            ICustomHelper argcustomHelperService,
            ICustomTiposNegocio argcustomTiposNegocioService
        )
        {
            _customHelper = argcustomHelperService;
            _customTiposNegocio = argcustomTiposNegocioService;
            _ls_calculated_controller_name = this.GetType().Name; // controller name
        }


        // Define a custom helper - ELN means (Error Line Number)
        private string CurrentControllerELN(int li_line_number)
        {
            return $"||{_ls_calculated_controller_name}|| Error line number(" + li_line_number + "): ";
        }


        // http://localhost:5255/bn_maestras/api/v1/CustomTiposNegocio/tipos_negocio
        [HttpGet]
        [Route("tipos_negocio")] // Must remove [action] before controller definition
        [SwaggerOperation(Summary = "Permite obtener la lista de tipos negocio de forma filtrada y paginada.", Description = "")]        
        [ProducesResponseType(typeof(ObjFnResMainGetTiposNegocioFiltered), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetTiposNegocioFiltered(
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
                result = await _customTiposNegocio.MainGetTiposNegocioFilteredAsync(ls_json); // Pasamos ls_json para conservar estandar                

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    this.CurrentControllerELN(_customHelper.LineNumber()) + " MainGetTiposNegocioFilteredAsync: " + ex.Message);
            }

            return StatusCode(HttpContext.Response.StatusCode, result); // return 200 response
        }





        // http://localhost:5255/bn_maestras/api/v1/CustomTiposNegocio/tipos_negocio_all
        [HttpGet]
        [Route("tipos_negocio_all")] // Must remove [action] before controller definition
        [SwaggerOperation(Summary = "[Prototipo] Permite obtener la lista de tipos negocio de forma completa y sin paginar.", Description = "")]
        [ProducesResponseType(typeof(FnResMainGetTiposNegocioAll), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetTiposNegocioAll()
        {
            string ls_json = "{}";
            string result = "";

            try
            {
                result = await _customTiposNegocio.MainGetTiposNegocioAllAsync(ls_json); // Pasamos ls_json para conservar estandar
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    this.CurrentControllerELN(_customHelper.LineNumber()) + " MainGetTiposNegocioAllAsync: " + ex.Message);
            }

            return StatusCode(HttpContext.Response.StatusCode, result); // return 200 response
        }
    }
}
