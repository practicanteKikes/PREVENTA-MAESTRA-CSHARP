using MainProject.Services.CustomDatabaseOutput.Models.FnRes;
using MainProject.Services.CustomDatabaseOutput.Models.Json;
using MainProject.Services.CustomHelper;
using MainProject.Services.CustomSqlserver.Models.EFModel;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json;

namespace MainProject.Services.CustomSqlserver.Impl
{
    public class CustomSqlserverService : ICustomSqlserver
    {
        // INYECTED
        private readonly ICustomHelper _customHelper;

        // para una proxima vez, eliges el nombre estandar _context ^^
        private readonly MSSQLINBANCOSDbContext _dbContextINBANCOS; // SqlServerContext

        // CALCULATED
        private readonly string? _currentServiceName;
        private readonly JsonSerializerOptions _currentJsonOptions;

        public CustomSqlserverService(
            ICustomHelper argcustomHelperService,
            MSSQLINBANCOSDbContext argdbContextInbancos
        )
        {
            _customHelper = argcustomHelperService;
            _dbContextINBANCOS = argdbContextInbancos; // se instancia el objeto Data context

            _currentServiceName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name; // set the current class name
            _currentJsonOptions = _customHelper.getJsonSerializeOptions();
        }




        public async Task<ObjFnResFindFirstOrDefaultByNit?> FindFirstOrDefaultByNit(string ls_nit)
        {
            // Retornaremos esto
            ObjFnResFindFirstOrDefaultByNit objFnRes = new ObjFnResFindFirstOrDefaultByNit()
            {
                Success = false,
                Message = "",
                Data = new List<VwOccidenteTercerosOutputJsonModel>()
            };

            // AUX VARIABLES
            VwOccidenteTercerosOutputJsonModel terceroJsn = new();
            string lsFnPropertyName = "";
            string ls_exception_error = "";


            // get row using EntityFramework sql server
            VwOccidenteTercerosAutAth? terceroSqlServer = null;
            try
            {
                terceroSqlServer = await _dbContextINBANCOS.VwOccidenteTercerosAthAuts
                                            // buscamos con like
                                            //.Where(u => EF.Functions.Like(u.Nit, $"%{ls_nit}%"))
                                            //.FirstOrDefaultAsync();

                                            // buscamos de forma exacta
                                            .FirstOrDefaultAsync(u => u.Nit == ls_nit);
            }
            catch (Exception ex)
            {
                //throw new Exception("bd error"); // No lo manejamos así porque no sabriamos el motivo del error...

                // SQL ERR                
                lsFnPropertyName = $"No fue posible consultar los datos para el tercero: {ls_nit}";
                objFnRes.Message = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + lsFnPropertyName + ".  Mensaje: " + $"Error al realizar la consulta tercero: {ex.Message}";
                objFnRes.Success = false; // Solo retornamos false cuando no fue posible consultar
                return objFnRes;
            }

            // NOT FOUND
            if (terceroSqlServer is null)
            {
                lsFnPropertyName = ls_nit;
                objFnRes.Message = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): El el tercero no está creado. (" + lsFnPropertyName + ")";
                objFnRes.Success = true; // Consulta fue bien pero no se encontró registros
                return objFnRes;
            }

            // FOUND
            if (terceroSqlServer is not null)
            {

                // CAST TO EXPECTED JSONMODEL
                VwOccidenteTercerosOutputJsonModel jsonObjectReceived; // Force to be an object
                VwOccidenteTercerosOutputJsonModel? rootConsolidatedQueryJsonModelOrNull = null; // necesary variable to try catch to cast postman json object            
                try
                {
                    string ls_terceros_json = JsonSerializer.Serialize(terceroSqlServer, _currentJsonOptions);
                    rootConsolidatedQueryJsonModelOrNull = JsonSerializer.Deserialize<VwOccidenteTercerosOutputJsonModel>(ls_terceros_json, _currentJsonOptions);
                }
                catch (JsonException e)
                {
                    //[0380] Datos errados               
                    ls_exception_error = "Error in convirtiendo objeto terceroSqlServer to object JSON: ";
                    ls_exception_error += e.Message;


                    objFnRes.Success = false;
                    objFnRes.Message = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_exception_error;
                    return objFnRes;
                }
                jsonObjectReceived = rootConsolidatedQueryJsonModelOrNull ?? new VwOccidenteTercerosOutputJsonModel();


                // SET DATA BEFORE RETURN
                objFnRes.Success = true;
                objFnRes.Data.Add(jsonObjectReceived);
            }


            // RETURN ROW LIKE ADATA
            return objFnRes;
        }
        
        
        
        public async Task<ObjFnResBancosConveniosByConvenio?> BancosConveniosByConvenioAsync(string ls_convenio)
        {
            // Retornaremos esto
            ObjFnResBancosConveniosByConvenio objFnRes = new ObjFnResBancosConveniosByConvenio()
            {
                Success = false,
                Message = "",
                Data = new List<BancoConvenioOutputJsonModel>()
            };

            // AUX VARIABLES
            BancoConvenioOutputJsonModel terceroJsn = new();
            string lsFnPropertyName = "";
            string ls_exception_error = "";


            // get row using EntityFramework sql server
            BancoConvenioEF? rowSqlServer = null;
            try
            {
                rowSqlServer = await _dbContextINBANCOS.BancosConveniosEfs
                                            // sql like mode %%
                                            //.Where(u => EF.Functions.Like(u.Cod_convenio_recaudo, $"%{ls_convenio}%") /* && u.Cod_banconal == "23" */ ) // First condition                                            
                                            
                                            // sql equal mode =
                                            .Where(u => u.Cod_convenio_recaudo == $"{ls_convenio}") // First condition                                           
                                            .Where(u => u.Cod_banconal == "23") // 23 corresponde a occidente
                                            .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                //throw new Exception("bd error"); // No lo manejamos así porque no sabriamos el motivo del error...

                // SQL ERR                
                lsFnPropertyName = $"No fue posible consultar los datos para el convenio: {ls_convenio}";
                objFnRes.Message = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + lsFnPropertyName + ".  Mensaje: " + $"Error al realizar la consulta tercero: {ex.Message}";
                objFnRes.Success = false; // Solo retornamos false cuando no fue posible consultar
                return objFnRes;
            }

            // NOT FOUND
            if (rowSqlServer is null)
            {
                lsFnPropertyName = ls_convenio;
                objFnRes.Message = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): El convenio no está creado. (" + lsFnPropertyName + ")";
                objFnRes.Success = true; // Consulta fue bien pero no se encontró registros
                return objFnRes;
            }

            // FOUND
            if (rowSqlServer is not null)
            {

                // CAST TO EXPECTED JSONMODEL
                BancoConvenioOutputJsonModel jsonObjectReceived; // Force to be an object
                BancoConvenioOutputJsonModel? rootConsolidatedQueryJsonModelOrNull = null; // necesary variable to try catch to cast postman json object            
                try
                {
                    string ls_terceros_json = JsonSerializer.Serialize(rowSqlServer, _currentJsonOptions);
                    rootConsolidatedQueryJsonModelOrNull = JsonSerializer.Deserialize<BancoConvenioOutputJsonModel>(ls_terceros_json, _currentJsonOptions);
                }
                catch (JsonException e)
                {
                    //[0380] Datos errados               
                    ls_exception_error = "Error in convirtiendo objeto convenioSqlServer to object JSON: ";
                    ls_exception_error += e.Message;


                    objFnRes.Success = false;
                    objFnRes.Message = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_exception_error;
                    return objFnRes;
                }
                jsonObjectReceived = rootConsolidatedQueryJsonModelOrNull ?? new BancoConvenioOutputJsonModel();


                // SET DATA BEFORE RETURN
                objFnRes.Success = true;
                objFnRes.Data.Add(jsonObjectReceived);
            }


            // RETURN ROW LIKE ADATA
            return objFnRes;
        }




        public async Task<ObjFnResLoadDbInfoOutput> LoadDbInfoAsync()
        {
            ObjFnResLoadDbInfoOutput objFnRes = new()
            {
                Success = false,
                Message = "",
                Data = new List<DatabaseInfoOutputJsonModel>()
            };

            // AUX VARIABLES
            DatabaseInfoOutputJsonModel dbInfoJsn = new();
            string lsFnPropertyName = "";
            string ls_exception_error = "";

            string query = "SELECT DB_NAME() AS db_name";


            // get row using EntityFramework sql server
            DatabaseInfoEF? modelSqlServer = null;
            try
            {
                modelSqlServer = await _dbContextINBANCOS.DatabaseInfoEFs
                    .FromSqlRaw(query)
                    .OrderBy(x => x.Db_name) // Replace "Id" with a unique column or property
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                //throw new Exception("bd error"); // No lo manejamos así porque no sabriamos el motivo del error...

                // SQL ERR                
                lsFnPropertyName = $"No fue posible consultar db_name:";
                objFnRes.Message = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + lsFnPropertyName + ".  Mensaje: " + $"Error al realizar la consulta interna DbInfo: {ex.Message}";
                objFnRes.Success = false; // Solo retornamos false cuando no fue posible consultar
                return objFnRes;
            }

            // NOT FOUND
            if (modelSqlServer is null)
            {
                lsFnPropertyName = "db_name";
                objFnRes.Message = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): No se ha creado la fila. (" + lsFnPropertyName + ")";
                objFnRes.Success = true; // Consulta fue bien pero no se encontró registros
                return objFnRes;
            }

            // FOUND
            if (modelSqlServer is not null)
            {
                // CAST TO EXPECTED JSONMODEL
                DatabaseInfoOutputJsonModel jsonObjectReceived; // Force to be an object
                DatabaseInfoOutputJsonModel? rootConsolidatedQueryJsonModelOrNull = null; // necesary variable to try catch to cast postman json object            
                try
                {
                    string ls_terceros_json = JsonSerializer.Serialize(modelSqlServer, _currentJsonOptions);
                    rootConsolidatedQueryJsonModelOrNull = JsonSerializer.Deserialize<DatabaseInfoOutputJsonModel>(ls_terceros_json, _currentJsonOptions);
                }
                catch (JsonException e)
                {
                    //[0380] Datos errados               
                    ls_exception_error = "Error in convirtiendo objeto sqlserverInfoDb to object JSON: ";
                    ls_exception_error += e.Message;


                    objFnRes.Success = false;
                    objFnRes.Message = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_exception_error;
                    return objFnRes;
                }
                jsonObjectReceived = rootConsolidatedQueryJsonModelOrNull ?? new DatabaseInfoOutputJsonModel();


                // SET DATA BEFORE RETURN
                objFnRes.Success = true;
                objFnRes.Data.Add(jsonObjectReceived);
            }

            return objFnRes;
        }






        //endservice
    }
}
