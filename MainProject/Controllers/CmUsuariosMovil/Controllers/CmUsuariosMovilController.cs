using MainProject.Attributes;
using MainProject.Controllers.CmUsuariosMovil.Models.CmUsuariosMovilAll.MainGetCmUsuariosMovilAllAsync.FnRes;
using MainProject.Controllers.CmUsuariosMovil.Models.CmUsuariosMovilFiltered.MainGetCmUsuariosMovilFilteredAsync;
using MainProject.Controllers.CmUsuariosMovil.Services;
using MainProject.Services.CustomHelper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MainProject.Controllers.CmUsuariosMovil.Controllers
{
    [ApiController]

    // Funciona con swagger
    [CustomRoute("[controller]")] // Ruta con prefijo usando Attributes appsettings.json: RoutePrefix = bn_maestras/api/v1/
    public class CmUsuariosMovilController : ControllerBase
    {
        private readonly ICustomHelper _customHelper;
        private readonly ICmUsuariosMovil _CmUsuariosMovil; // define injected object

        // calculated
        private string _ls_calculated_controller_name = "";

        public CmUsuariosMovilController(
            ICustomHelper argcustomHelperService,
            ICmUsuariosMovil argCmUsuariosMovilService
        )
        {
            _customHelper = argcustomHelperService;
            _CmUsuariosMovil = argCmUsuariosMovilService;
            _ls_calculated_controller_name = this.GetType().Name; // controller name
        }


        // Define a custom helper - ELN means (Error Line Number)
        private string CurrentControllerELN(int li_line_number)
        {
            return $"||{_ls_calculated_controller_name}|| Error line number(" + li_line_number + "): ";
        }


        // http://localhost:5255/bn_maestras/api/v1/CmUsuariosMovil/usuarios_movil
        [HttpGet]
        [Route("usuarios_movil")] // Must remove [action] before controller definition
        [SwaggerOperation(Summary = "Permite obtener la lista de |usuarios_movil| de forma filtrada y paginada.", Description = "")]
        [ProducesResponseType(typeof(FnResMainGetCmUsuariosMovilFiltered), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetCmUsuariosMovilFiltered(
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
                result = await _CmUsuariosMovil.MainGetCmUsuariosMovilFilteredAsync(ls_json); // Pasamos ls_json para conservar estandar                

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    this.CurrentControllerELN(_customHelper.LineNumber()) + " MainGetCmUsuariosMovilFilteredAsync: " + ex.Message);
            }

            return StatusCode(HttpContext.Response.StatusCode, result); // return 200 response
        }





        // http://localhost:5255/bn_maestras/api/v1/CmUsuariosMovil/usuarios_movil_all
        [HttpGet]
        [Route("usuarios_movil_all")] // Must remove [action] before controller definition
        [SwaggerOperation(Summary = "[Prototipo] Permite obtener la lista de |usuarios_movil_all| de forma completa y sin paginar.", Description = "")]
        [ProducesResponseType(typeof(FnResMainGetCmUsuariosMovilAll), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetCmUsuariosMovilAll()
        {
            string ls_json = "{}";
            string result = "";

            try
            {
                result = await _CmUsuariosMovil.MainGetCmUsuariosMovilAllAsync(ls_json); // Pasamos ls_json para conservar estandar
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    this.CurrentControllerELN(_customHelper.LineNumber()) + " MainGetCmUsuariosMovilAllAsync: " + ex.Message);
            }

            return StatusCode(HttpContext.Response.StatusCode, result); // return 200 response
        }
    }
}
