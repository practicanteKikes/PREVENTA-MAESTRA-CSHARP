using DWNet.Data;
using MainProject.Attributes;
using MainProject.Controllers.CmZonasBio.Models.CmZonasBioAll.CmZonasBioMainGetAllAsync;
using MainProject.Controllers.CmZonasBio.Models.CmZonasBioFiltered.CmZonasBioMainGetFilteredAsync;
using MainProject.Controllers.CmZonasBio.Services;
using MainProject.Services.CustomHelper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MainProject.Controllers.CmZonasBio.Controllers
{
    [ApiController]

    // Funciona con swagger
    [CustomRoute("[controller]")] // Ruta con prefijo usando Attributes appsettings.json: RoutePrefix = bn_maestras/api/v1/
    public class CmZonasBioController : ControllerBase
    {
        private readonly ICustomHelper _customHelper;
        private readonly ICmZonasBio _CmZonasBio; // define injected object

        // calculated
        private string _ls_calculated_controller_name = "";

        public CmZonasBioController(
            ICustomHelper argcustomHelperService,
            ICmZonasBio argCmZonasBioService
        )
        {
            _customHelper = argcustomHelperService;
            _CmZonasBio = argCmZonasBioService;
            _ls_calculated_controller_name = this.GetType().Name; // controller name
        }


        // Define a custom helper - ELN means (Error Line Number)
        private string CurrentControllerELN(int li_line_number)
        {
            return $"||{_ls_calculated_controller_name}|| Error line number(" + li_line_number + "): ";
        }


        // http://localhost:5255/bn_maestras/api/v1/CmZonasBio/zonas_bio
        [HttpGet]
        [Route("zonas_bio")] // Must remove [action] before controller definition
        [SwaggerOperation(Summary = "Permite obtener la lista de |zonas_bio| de forma filtrada y paginada.", Description = "")]
        [ProducesResponseType(typeof(CmZonasBioFnResMainGetFiltered), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> CmZonasBioGetFiltered(
              [FromQuery] int? limit = 2
            , [FromQuery] string? fec_registro_min = "1899-12-31" // 1900-12-31
            , [FromQuery] int? after_id = 0
            , [FromQuery] string? id_zona= ""
        )
        {

            string ls_json = "{}";
            string result = "";

            try
            {
                result = await _CmZonasBio.CmZonasBioMainGetFilteredAsync(id_zona); // Pasamos ls_json para conservar estandar                

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    this.CurrentControllerELN(_customHelper.LineNumber()) + " CmZonasBioMainGetFilteredAsync: " + ex.Message);
            }

            return StatusCode(HttpContext.Response.StatusCode, result); // return 200 response
        }





        // http://localhost:5255/bn_maestras/api/v1/CmZonasBio/zonas_bio_all
        [HttpGet]
        [Route("zonas_bio_all")] // Must remove [action] before controller definition
        [SwaggerOperation(Summary = "[Prototipo] Permite obtener la lista de |zonas_bio_all| de forma completa y sin paginar.", Description = "")]
        [ProducesResponseType(typeof(CmZonasBioFnResMainGetAll), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> CmZonasBioGetAll()
        {
            string ls_json = "{}";
            string result = "";

            try
            {
                result = await _CmZonasBio.CmZonasBioMainGetAllAsync(ls_json); // Pasamos ls_json para conservar estandar
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    this.CurrentControllerELN(_customHelper.LineNumber()) + " CmZonasBioMainGetAllAsync: " + ex.Message);
            }

            return StatusCode(HttpContext.Response.StatusCode, result);//return 200 response

         

            
            }
        }





        }

        
        



    

        
        



    
    

