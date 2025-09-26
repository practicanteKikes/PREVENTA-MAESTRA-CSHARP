using MainProject.Attributes;
using MainProject.Controllers.CmProductos.Models.CmProductosAll.CmProductosMainGetAllAsync;
using MainProject.Controllers.CmProductos.Models.CmProductosFiltered.CmProductosMainGetFilteredAsync;
using MainProject.Controllers.CmProductos.Services;
using MainProject.Services.CustomHelper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MainProject.Controllers.CmProductos.Controllers
{
    [ApiController]

    // Funciona con swagger
    [CustomRoute("[controller]")] // Ruta con prefijo usando Attributes appsettings.json: RoutePrefix = bn_maestras/api/v1/
    public class CmProductosController : ControllerBase
    {
        private readonly ICustomHelper _customHelper;
        private readonly ICmProductos _CmProductos; // define injected object

        // calculated
        private string _ls_calculated_controller_name = "";

        public CmProductosController(
            ICustomHelper argcustomHelperService,
            ICmProductos argCmProductosService
        )
        {
            _customHelper = argcustomHelperService;
            _CmProductos = argCmProductosService;
            _ls_calculated_controller_name = this.GetType().Name; // controller name
        }


        // Define a custom helper - ELN means (Error Line Number)
        private string CurrentControllerELN(int li_line_number)
        {
            return $"||{_ls_calculated_controller_name}|| Error line number(" + li_line_number + "): ";
        }


        // http://localhost:5255/bn_maestras/api/v1/CmProductos/productos
        [HttpGet]
        [Route("productos")] // Must remove [action] before controller definition
        [SwaggerOperation(Summary = "Permite obtener la lista de |productos| de forma filtrada y paginada.", Description = "")]
        [ProducesResponseType(typeof(CmProductosFnResMainGetFiltered), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> CmProductosGetFiltered(
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
                result = await _CmProductos.CmProductosMainGetFilteredAsync(ls_json); // Pasamos ls_json para conservar estandar                

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    this.CurrentControllerELN(_customHelper.LineNumber()) + " CmProductosMainGetFilteredAsync: " + ex.Message);
            }

            return StatusCode(HttpContext.Response.StatusCode, result); // return 200 response
        }





        // http://localhost:5255/bn_maestras/api/v1/CmProductos/productos_all
        [HttpGet]
        [Route("productos_all")] // Must remove [action] before controller definition
        [SwaggerOperation(Summary = "[Prototipo] Permite obtener la lista de |productos_all| de forma completa y sin paginar.", Description = "")]
        [ProducesResponseType(typeof(CmProductosFnResMainGetAll), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> CmProductosGetAll()
        {
            string ls_json = "{}";
            string result = "";

            try
            {
                result = await _CmProductos.CmProductosMainGetAllAsync(ls_json); // Pasamos ls_json para conservar estandar
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    this.CurrentControllerELN(_customHelper.LineNumber()) + " CmProductosMainGetAllAsync: " + ex.Message);
            }

            return StatusCode(HttpContext.Response.StatusCode, result); // return 200 response
        }
    }
}
