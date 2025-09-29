using MainProject.Attributes;
using MainProject.Controllers.CmMotivosNc.Models.CmMotivosNcAll.CmMotivosNcMainGetAllAsync;
using MainProject.Controllers.CmMotivosNc.Models.CmMotivosNcFiltered.CmMotivosNcMainGetFilteredAsync;
using MainProject.Controllers.CmMotivosNc.Services;
using MainProject.Services.CustomHelper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MainProject.Controllers.CmMotivosNc.Controllers
{
    [ApiController]

    // Funciona con swagger
    [CustomRoute("[controller]")] // Ruta con prefijo usando Attributes appsettings.json: RoutePrefix = bn_maestras/api/v1/
    public class CmMotivosNcController : ControllerBase
    {
        private readonly ICustomHelper _customHelper;
        private readonly ICmMotivosNc _CmMotivosNc; // define injected object

        // calculated
        private string _ls_calculated_controller_name = "";

        public CmMotivosNcController(
            ICustomHelper argcustomHelperService,
            ICmMotivosNc argCmMotivosNcService
        )
        {
            _customHelper = argcustomHelperService;
            _CmMotivosNc = argCmMotivosNcService;
            _ls_calculated_controller_name = this.GetType().Name; // controller name
        }


        // Define a custom helper - ELN means (Error Line Number)
        private string CurrentControllerELN(int li_line_number)
        {
            return $"||{_ls_calculated_controller_name}|| Error line number(" + li_line_number + "): ";
        }


        // http://localhost:5255/bn_maestras/api/v1/CmMotivosNc/motivos_nc
        [HttpGet]
        [Route("motivos_nc")] // Must remove [action] before controller definition
        [SwaggerOperation(Summary = "Permite obtener la lista de |motivos_nc| de forma filtrada y paginada.", Description = "")]
        [ProducesResponseType(typeof(CmMotivosNcFnResMainGetFiltered), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> CmMotivosNcGetFiltered(
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
                result = await _CmMotivosNc.CmMotivosNcMainGetFilteredAsync(ls_json); // Pasamos ls_json para conservar estandar                

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    this.CurrentControllerELN(_customHelper.LineNumber()) + " CmMotivosNcMainGetFilteredAsync: " + ex.Message);
            }

            return StatusCode(HttpContext.Response.StatusCode, result); // return 200 response
        }





        // http://localhost:5255/bn_maestras/api/v1/CmMotivosNc/motivos_nc_all
        [HttpGet]
        [Route("motivos_nc_all")] // Must remove [action] before controller definition
        [SwaggerOperation(Summary = "[Prototipo] Permite obtener la lista de |motivos_nc_all| de forma completa y sin paginar.", Description = "")]
        [ProducesResponseType(typeof(CmMotivosNcFnResMainGetAll), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> CmMotivosNcGetAll()
        {
            string ls_json = "{}";
            string result = "";

            try
            {
                result = await _CmMotivosNc.CmMotivosNcMainGetAllAsync(ls_json); // Pasamos ls_json para conservar estandar
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    this.CurrentControllerELN(_customHelper.LineNumber()) + " CmMotivosNcMainGetAllAsync: " + ex.Message);
            }

            return StatusCode(HttpContext.Response.StatusCode, result); // return 200 response
        }
    }
}
