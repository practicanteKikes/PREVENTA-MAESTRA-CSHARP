using MainProject.Attributes;
using MainProject.Controllers.CmLineasProductos.Models.CmLineasProductosAll.MainGetCmLineasProductosAllAsync.FnRes;
using MainProject.Controllers.CmLineasProductos.Models.CmLineasProductosFiltered.MainGetCmLineasProductosFilteredAsync;
using MainProject.Controllers.CmLineasProductos.Services;
using MainProject.Services.CustomHelper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MainProject.Controllers.CmLineasProductos.Controllers
{
    [ApiController]

    // Funciona con swagger
    [CustomRoute("[controller]")] // Ruta con prefijo usando Attributes appsettings.json: RoutePrefix = bn_maestras/api/v1/
    public class CmLineasProductosController : ControllerBase
    {
        private readonly ICustomHelper _customHelper;
        private readonly ICmLineasProductos _CmLineasProductos; // define injected object

        // calculated
        private string _ls_calculated_controller_name = "";

        public CmLineasProductosController(
            ICustomHelper argcustomHelperService,
            ICmLineasProductos argCmLineasProductosService
        )
        {
            _customHelper = argcustomHelperService;
            _CmLineasProductos = argCmLineasProductosService;
            _ls_calculated_controller_name = this.GetType().Name; // controller name
        }


        // Define a custom helper - ELN means (Error Line Number)
        private string CurrentControllerELN(int li_line_number)
        {
            return $"||{_ls_calculated_controller_name}|| Error line number(" + li_line_number + "): ";
        }


        // http://localhost:5255/bn_maestras/api/v1/CmLineasProductos/lineas_productos
        [HttpGet]
        [Route("lineas_productos")] // Must remove [action] before controller definition
        [SwaggerOperation(Summary = "Permite obtener la lista de |lineas_productos| de forma filtrada y paginada.", Description = "")]
        [ProducesResponseType(typeof(FnResMainGetCmLineasProductosFiltered), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetCmLineasProductosFiltered(
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
                result = await _CmLineasProductos.MainGetCmLineasProductosFilteredAsync(ls_json); // Pasamos ls_json para conservar estandar                

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    this.CurrentControllerELN(_customHelper.LineNumber()) + " MainGetCmLineasProductosFilteredAsync: " + ex.Message);
            }

            return StatusCode(HttpContext.Response.StatusCode, result); // return 200 response
        }





        // http://localhost:5255/bn_maestras/api/v1/CmLineasProductos/lineas_productos_all
        [HttpGet]
        [Route("lineas_productos_all")] // Must remove [action] before controller definition
        [SwaggerOperation(Summary = "[Prototipo] Permite obtener la lista de |lineas_productos_all| de forma completa y sin paginar.", Description = "")]
        [ProducesResponseType(typeof(FnResMainGetCmLineasProductosAll), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetCmLineasProductosAll()
        {
            string ls_json = "{}";
            string result = "";

            try
            {
                result = await _CmLineasProductos.MainGetCmLineasProductosAllAsync(ls_json); // Pasamos ls_json para conservar estandar
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    this.CurrentControllerELN(_customHelper.LineNumber()) + " MainGetCmLineasProductosAllAsync: " + ex.Message);
            }

            return StatusCode(HttpContext.Response.StatusCode, result); // return 200 response
        }
    }
}
