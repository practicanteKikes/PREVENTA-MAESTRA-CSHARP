using MainProject.Attributes;
using MainProject.Controllers.CustomDepartamentos.Models.DepartamentosAll.MainGetDepartamentosAllAsync.FnRes;
using MainProject.Controllers.CustomDepartamentos.Models.DepartamentosFiltered.MainGetDepartamentosFilteredAsync;
using MainProject.Controllers.CustomDepartamentos.Services;
using MainProject.Services.CustomHelper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MainProject.Controllers.CustomDepartamentos.Controllers
{
    [ApiController]

    // Funciona con swagger
    [CustomRoute("[controller]")] // Ruta con prefijo usando Attributes appsettings.json: RoutePrefix = bn_maestras/api/v1/
    public class CustomDepartamentosController : ControllerBase
    {
        private readonly ICustomHelper _customHelper;
        private readonly ICustomDepartamentos _customDepartamentos; // define injected object

        // calculated
        private string _ls_calculated_controller_name = "";

        public CustomDepartamentosController(
            ICustomHelper argcustomHelperService,
            ICustomDepartamentos argcustomDepartamentosService
        )
        {
            _customHelper = argcustomHelperService;
            _customDepartamentos = argcustomDepartamentosService;
            _ls_calculated_controller_name = this.GetType().Name; // controller name
        }


        // Define a custom helper - ELN means (Error Line Number)
        private string CurrentControllerELN(int li_line_number)
        {
            return $"||{_ls_calculated_controller_name}|| Error line number(" + li_line_number + "): ";
        }


        // http://localhost:5255/bn_maestras/api/v1/CustomDepartamentos/tipos_negocio
        [HttpGet]
        [Route("departamentos")] // Must remove [action] before controller definition
        [SwaggerOperation(Summary = "Permite obtener la lista de |departamentos| de forma filtrada y paginada.", Description = "")]
        [ProducesResponseType(typeof(FnResMainGetDepartamentosFiltered), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetDepartamentosFiltered(
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
                result = await _customDepartamentos.MainGetDepartamentosFilteredAsync(ls_json); // Pasamos ls_json para conservar estandar                

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    this.CurrentControllerELN(_customHelper.LineNumber()) + " MainGetDepartamentosFilteredAsync: " + ex.Message);
            }

            return StatusCode(HttpContext.Response.StatusCode, result); // return 200 response
        }





        // http://localhost:5255/bn_maestras/api/v1/CustomDepartamentos/departamentos_all
        [HttpGet]
        [Route("departamentos_all")] // Must remove [action] before controller definition
        [SwaggerOperation(Summary = "[Prototipo] Permite obtener la lista de |departamentos| de forma completa y sin paginar.", Description = "")]
        [ProducesResponseType(typeof(FnResMainGetDepartamentosAll), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetDepartamentosAll()
        {
            string ls_json = "{}";
            string result = "";

            try
            {
                result = await _customDepartamentos.MainGetDepartamentosAllAsync(ls_json); // Pasamos ls_json para conservar estandar
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    this.CurrentControllerELN(_customHelper.LineNumber()) + " MainGetDepartamentosAllAsync: " + ex.Message);
            }

            return StatusCode(HttpContext.Response.StatusCode, result); // return 200 response
        }
    }
}
