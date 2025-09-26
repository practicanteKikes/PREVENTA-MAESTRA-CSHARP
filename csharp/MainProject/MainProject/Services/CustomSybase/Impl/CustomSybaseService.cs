using MainProject.Services.CustomHelper;
using MainProject.Services.CustomSybase.Models.CmMotivosNcQuery.CmMotivosNcQueryAllAzync;
using MainProject.Services.CustomSybase.Models.CmMotivosNcQuery.CmMotivosNcQueryFilteredAzync;
using MainProject.Services.CustomSybase.Models.CmProductosQuery.CmProductosQueryAllAzync;
using MainProject.Services.CustomSybase.Models.CmProductosQuery.CmProductosQueryFilteredAzync;
using MainProject.Services.CustomSybase.Models.CmZonasBioQuery.CmZonasBioQueryAllAzync;
using MainProject.Services.CustomSybase.Models.CmZonasBioQuery.CmZonasBioQueryFilteredAzync;
using MainProject.Services.CustomSybase.Models.LoadDbInfo.FnRes;
using MainProject.Services.CustomSybase.Models.LoadDbInfo.Json;
using MainProject.Services.CustomSybase.Models.LoadInformationSchemaByTableName.FnRes;
using MainProject.Services.CustomSybase.Models.LoadInformationSchemaByTableName.Json;
using MainProject.Services.CustomSybase.Models.QueryAAAModelDefaultFnResFTRowsAzync;
using MainProject.Services.CustomSybase.Models.QueryClientesAllAzync.FnRes;
using MainProject.Services.CustomSybase.Models.QueryClientesFilteredAzync.FnRes;
using MainProject.Services.CustomSybase.Models.QueryClientesFilteredTotalRowsAzync.FnRes;
using MainProject.Services.CustomSybase.Models.QueryCmLineasProductos.All;
using MainProject.Services.CustomSybase.Models.QueryCmLineasProductos.Filtered;
using MainProject.Services.CustomSybase.Models.QueryCmUsuariosMovil.All;
using MainProject.Services.CustomSybase.Models.QueryCmUsuariosMovil.Filtered;
using MainProject.Services.CustomSybase.Models.QueryDepartamentosAllAzync;
using MainProject.Services.CustomSybase.Models.QueryDepartamentosFilteredAzync;
using MainProject.Services.CustomSybase.Models.QueryMunicipiosAllAzync;
using MainProject.Services.CustomSybase.Models.QueryMunicipiosFilteredAzync;
using MainProject.Services.CustomSybase.Models.QueryTiposNegocioAllAzync;
using MainProject.Services.CustomSybase.Models.QueryTiposNegocioFilteredAzync.FnRes;
using MainProject.Services.CustomSybase.Models.QueryTiposNegocioFilteredTotalRowsAzync.FnRes;
using MainProject.Services.CustomSybase.Models.QueryUsersFilteredAzync.FnRes;
using MainProject.Services.CustomSybase.Models.SnapModel;
using MainProject.Services.CustomSybase.Models.SnapModelFiltered;
using PowerScript.Bridge;
using SnapObjects.Data;
using System.Reflection;
using System.Text.Json;

namespace MainProject.Services.CustomSybase.Impl
{
    public class CustomSybaseService : ICustomSybase
    {
        private readonly ICustomHelper _customHelper;

        // usamos _dataContext como nombre de variable (estandar Appeon)
        // Porque sabemos que estamos en sybase y accediendo por snapobjects y sqlmodelmapper
        private readonly ODBCBionegociosDataContext _dataContext;
        private readonly string? _currentServiceName;
        private readonly string _LS_DMY_MILITARY = "dd/MM/yyyy HH:mm:ss";
        private readonly JsonSerializerOptions _currentJsonOptions;

        // INJECTED
        private IConfiguration _configuration; // Leeremos las tablas o vistas usadas


        // ========== CONSTRUCTOR ========= //
        public CustomSybaseService(
            ICustomHelper argcustomHelperService,
            ODBCBionegociosDataContext argdataContext,
            IConfiguration argconfiguration
        )
        {
            _customHelper = argcustomHelperService;
            _dataContext = argdataContext;
            _currentServiceName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name; // set the current class name
            _currentJsonOptions = _customHelper.getJsonSerializeOptions();
            _configuration = argconfiguration;
        }

        public int InsertDepositNotification<T>(T newRecord)
        {
            // Your logic to insert the deposit notification
            if (newRecord is RecaudoSnapModel clonedModel)
            {
                return InsertDepositNotificationSnapModel(clonedModel);
            }

            Console.WriteLine($"Unsupported record type: {typeof(T)}");
            return 0; // Return a failure indicator
        }



        // USING ORM - SnapObjects SqlModelMapper entity
        private int InsertDepositNotificationSnapModel(RecaudoSnapModel newRecaudoModel)
        {
            int insertedId = 0;
            try
            {
                // Insert
                var dbResult = _dataContext.SqlModelMapper.TrackCreate(newRecaudoModel)
                .SaveChanges();
            }
            catch (Exception ex)
            {
                string errorMsg = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): Error al insertar el SqlModelMapper RecaudoSnapModel by reference1  (" + newRecaudoModel.Id_cliente + ") " + $"Error: {ex.Message}";
                throw new Exception(errorMsg);
            }

            return insertedId = newRecaudoModel.Id;
        }



        //public ObjFnResLoadUserByEmailPassword LoadUserByEmailPassword(string ls_email, string ls_password)
        //{
        //    ObjFnResLoadUserByEmailPassword objFnRes = new ObjFnResLoadUserByEmailPassword()
        //    {
        //        Success = false,
        //        Message = "",
        //        Data = new List<ClienteJsonModel>()
        //    };


        //    ClienteJsonModel userJsn = new();
        //    string lsFnPropertyName = "";

        //    // Get row using sqlmodelmapper sybase ase
        //    UserSnapByEmailPassword? userAse = new UserSnapByEmailPassword();
        //    try
        //    {
        //        userAse = _dataContext.SqlModelMapper.Load<UserSnapByEmailPassword>(ls_email, ls_password)
        //            .FirstOrDefault();
        //    }
        //    // RESPONSE DB ERROR
        //    catch (Exception ex)
        //    {
        //        lsFnPropertyName = $"No fue posible consultar los datos para el usuario: {ls_email} {ls_password}";
        //        objFnRes.Message = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + lsFnPropertyName + ".  Mensaje: " + ex.Message;
        //        objFnRes.Success = false;
        //        return objFnRes;
        //    }

        //    // RESPONSE NOT FOUND
        //    if (userAse is null)
        //    {
        //        lsFnPropertyName = ls_email;
        //        objFnRes.Message = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): El el usuario no está creado. (" + lsFnPropertyName + ")";
        //        objFnRes.Success = true;
        //        return objFnRes;
        //    }

        //    // RESPONSE FOUND
        //    if (userAse is not null)
        //    {
        //        objFnRes.Success = true;

        //        //userJsn.Id = userAse.Id.ToString();
        //        userJsn.User = userAse.User;
        //        userJsn.Password = userAse.Password;
        //        userJsn.Email = userAse.Email;
        //        userJsn.Estado = userAse.Estado;

        //        objFnRes.Data.Add(userJsn);
        //    }

        //    return objFnRes;
        //}



        public ObjFnResLoadDbInfoInput LoadDbInfo()
        {
            ObjFnResLoadDbInfoInput objFnRes = new()
            {
                Success = false,
                Message = "",
                Data = new List<DatabaseInfoInputJsonModel>()
            };


            DatabaseInfoInputJsonModel modelJsn = new();
            string sql = "select 1";
            string lsFnPropertyName = "";

            // Get row using sqlExecutor sybase ase                        
            sql = @"SELECT DB_NAME() as db_name";


            SqlContext sqlContext;
            sqlContext = _dataContext.CreateSqlContext(sql);

            var result = _dataContext.SqlExecutor.SelectOne(sqlContext.SqlText, out SqlResult sqlResultObject, sqlContext.Parms);

            switch (sqlResultObject.SqlCode)
            {
                case -1: // sql query error                                        
                    lsFnPropertyName = "No fue posible obtener db_name: ";
                    objFnRes.Message = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + lsFnPropertyName + ".  Mensaje: " + sqlResultObject.ErrorText.ToString();
                    break;

                case 0: // sql query is OK                                        
                    objFnRes.Success = true;

                    modelJsn.Db_name = result.GetValue<string>("db_name");

                    objFnRes.Data.Add(modelJsn);
                    break;

                case 100: // sql record not found                                        
                    lsFnPropertyName = "db_name";
                    objFnRes.Message = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): columna no está creado. (" + lsFnPropertyName + ")";
                    break;

                default: // set error by default
                    lsFnPropertyName = "db_name";
                    objFnRes.Message = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): Table (" + lsFnPropertyName + ") can not be consulted.";
                    break;
            }

            return objFnRes;
        }




        // Custom method
        public int InsertDepositNotifyLog<T>(T newRecord)
        {
            // Your logic to insert the deposit notification
            if (newRecord is RecaudoLogSnapModel clonedModel)
            {
                return this.InsertDepositNotifyLogSnapModel(clonedModel);
            }

            Console.WriteLine($"Unsupported record type: {typeof(T)}");
            return 0; // Return a failure indicator
        }




        // INSERT ROW LOG USING ORM - SnapObjects SqlModelMapper entity
        private int InsertDepositNotifyLogSnapModel(RecaudoLogSnapModel newRecaudoLogModel)
        {
            int insertedId = 0;
            try
            {
                // Insert
                var dbResult = _dataContext.SqlModelMapper.TrackCreate(newRecaudoLogModel)
                .SaveChanges();
            }
            catch (Exception ex)
            {
                string errorMsg = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): Error al insertar el SqlModelMapper RecaudoLogSnapModel by input  (retornar nombre method from response service??) " + $"Error: {ex.Message}";
                throw new Exception(errorMsg);
            }

            return insertedId = newRecaudoLogModel.Id;
        }




        public ObjFnResLoadInformationSchemaByTableName LoadInformationSchemaByTableName(string ls_table_name)
        {
            ObjFnResLoadInformationSchemaByTableName objFnRes = new ObjFnResLoadInformationSchemaByTableName()
            {
                Success = false,
                Message = "",
                Data = new List<InformationSchemaJsonModel>() // Voy aquiii
            };


            InformationSchemaJsonModel rowModelJsn = new();
            string lsFnPropertyName = "";

            // Get row using sqlmodelmapper sybase ase
            InformationSchemaSnapByTableName? informationSchemaAse = new InformationSchemaSnapByTableName();
            try
            {
                informationSchemaAse = _dataContext.SqlModelMapper.Load<InformationSchemaSnapByTableName>(ls_table_name)
                    .FirstOrDefault();
            }
            // RESPONSE DB ERROR
            catch (Exception ex)
            {
                lsFnPropertyName = $"No fue posible encontrar la fila en el InformationSchema ASE (sysobjects) - Tabla buscada: {ls_table_name}";
                objFnRes.Message = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + lsFnPropertyName + ".  Mensaje: " + ex.Message;
                objFnRes.Success = false;
                return objFnRes;
            }

            // RESPONSE NOT FOUND
            if (informationSchemaAse is null)
            {
                lsFnPropertyName = ls_table_name;
                objFnRes.Message = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): La tabla no está creada en la bd. (" + lsFnPropertyName + ")";
                objFnRes.Success = true;
                return objFnRes;
            }

            // RESPONSE FOUND
            if (informationSchemaAse is not null)
            {
                objFnRes.Success = true;

                rowModelJsn.Type = informationSchemaAse.Type;
                rowModelJsn.Name = informationSchemaAse.Name;

                objFnRes.Data.Add(rowModelJsn);
            }

            return objFnRes;
        }



        public ODBCBionegociosDataContext GetConnection()
        {
            return _dataContext;
        }



        // ===== CLIENTES
        public async Task<string> QueryClientesAllAzync()
        {
            ObjFnResBdQueryClientesAllAzync objFnRes = new ObjFnResBdQueryClientesAllAzync()
            {
                Success = false,
                Message = "",
                Data = new List<ClienteSnapModel>()
            };

            string ls_settingsjson_bionegocios_clientes_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_clientes_table");
            string ls_aux_error_message = "";
            List<ClienteSnapModel> clienteAseList = new List<ClienteSnapModel>();


            ////////////////////////////////////////////
            // QUERY USING SQLEXECUTOR SCALAR SQLMANUALLY
            //string sqlQuery = @"
            //    select top 1
            //     id
            //    from test_incusan3.dba.vw_clientes_full_BIO
            //";
            //List<object> minimalRequiredModelParams = new List<object> { };            

            //// Is required | Convert objectList to objectArray
            //object[] paramsArrayObject = minimalRequiredModelParams.ToArray();

            //try
            //{
            //    var li_total_rows = await _dataContext.SqlExecutor.ScalarAsync<string>(sqlQuery, paramsArrayObject);
            //    Console.WriteLine("registros clientes total by sql hardcode: ");
            //    Console.WriteLine(JsonSerializer.Serialize(li_total_rows));
            //}
            //catch (Exception ex)
            //{
            //    var ls_catch_err = $"No fue posible realizar la consulta a: la vista baby";
            //    //objFnRes.Ksuccess = false;
            //    //objFnRes.Kmessage = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_catch_err + ".  Mensaje: " + ex.Message;
            //    return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            //}
            ////////////////////////////////////////////



            // GET ROWS using sqlmodelmapper sybase ase            
            try
            {
                clienteAseList = (await _dataContext.SqlModelMapper.LoadAsync<ClienteSnapModel>()).ToList(); // Load no retornará null - Sino un tipo Inumerable                
                                                                                                             //Console.WriteLine("registros clientes all: ");
                                                                                                             //Console.WriteLine(JsonSerializer.Serialize(clienteAseList));

            }
            // RESPONSE DB ERROR
            catch (Exception ex)
            {
                ls_aux_error_message = $"No fue posible realizar la consulta a: " + ls_settingsjson_bionegocios_clientes_table;
                objFnRes.Message = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_aux_error_message + ".  Mensaje: " + ex.Message;
                objFnRes.Success = false;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Success = true;
            objFnRes.Data = clienteAseList;

            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }

        public async Task<string> QueryClientesFilteredAzync(Dictionary<string, object> la_params)
        {
            ObjFnResBdQueryClientesFilteredAzync objFnRes = new ObjFnResBdQueryClientesFilteredAzync()
            {
                Success = false,
                Message = "",
                Data = new List<ClienteSnapByFiltered>()
            };



            // define an array of parameters expected to be received. [ like an asociative array but in csharp is a dictionary ]
            Dictionary<string, object> la_params_default = new Dictionary<string, object>
            {
                { "li_after_id", -1 },
                { "ls_fec_registro_min", "" }, // 1800-12-31
                { "ls_zona", "" },
                { "li_limit", 50 }
            };
            Dictionary<string, object> ldictionary_mergedResult = _customHelper.MergeArrays(la_params_default, la_params); // merge the params


            // SET MERGED VALUES
            int li_after_id = _customHelper.getMergedPropertyInt("li_after_id", ldictionary_mergedResult) ?? -1;
            string ls_fec_registro_min = _customHelper.getMergedPropertyString("ls_fec_registro_min", ldictionary_mergedResult) ?? "1900-12-31";
            int li_limit = _customHelper.getMergedPropertyInt("li_limit", ldictionary_mergedResult) ?? 50;
            string ls_zona = _customHelper.getMergedPropertyString("ls_zona", ldictionary_mergedResult) ?? "";
            //Console.WriteLine(JsonSerializer.Serialize(ldictionary_mergedResult));


            string ls_settingsjson_bionegocios_clientes_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_clientes_table");
            string ls_aux_error_message = "";
            List<ClienteSnapByFiltered> clienteAseList = new List<ClienteSnapByFiltered>();


            // QUERY USING SNAPOBJECTS SQLMODELMAPPER AND EXTRA BUILDER
            var Builder = _dataContext.SqlModelMapper.GetQueryBuilder<ClienteSnapByFiltered>();

            List<object> minimalRequiredModelParams = new List<object> { li_after_id, ls_fec_registro_min };
            if (ls_zona.Length > 0)
            {
                Builder.AndWhere("zona", SqlBinaryOperator.Equals, SqlBuilder.Parameter<string>("zonaArgument"));
                minimalRequiredModelParams.Add(ls_zona);
            }

            // Is required | Convert objectList to objectArray
            object[] paramsArrayObject = minimalRequiredModelParams.ToArray();
            try
            {
                clienteAseList = (await Builder.LoadByPageAsync(0, li_limit, paramsArrayObject)).ToList();
            }
            // RESPONSE DB ERROR
            catch (Exception ex)
            {
                ls_aux_error_message = $"No fue posible realizar la consulta a: " + ls_settingsjson_bionegocios_clientes_table;
                objFnRes.Message = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_aux_error_message + ".  Mensaje: " + ex.Message;
                objFnRes.Success = false;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Success = true;
            objFnRes.Data = clienteAseList;
            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }

        public async Task<string> QueryClientesFilteredTotalRowsAzync(Dictionary<string, object> la_params)
        {
            ObjFnResQueryClientesFilTotalRowsAzync objFnRes = new ObjFnResQueryClientesFilTotalRowsAzync()
            {
                Success = false,
                Message = "",
                Data = new List<int>()
            };

            // define an array of parameters expected to be received. [ like an asociative array but in csharp is a dictionary ]
            Dictionary<string, object> la_params_default = new Dictionary<string, object>
            {
                { "li_after_id", -1 },
                { "ls_fec_registro_min", "" }, // 1800-12-31
                { "ls_zona", "" },
                { "li_limit", 50 }
            };
            Dictionary<string, object> ldictionary_mergedResult = _customHelper.MergeArrays(la_params_default, la_params); // merge the params


            int li_after_id = _customHelper.getMergedPropertyInt("li_after_id", ldictionary_mergedResult) ?? -1;
            string ls_fec_registro_min = _customHelper.getMergedPropertyString("ls_fec_registro_min", ldictionary_mergedResult) ?? "1900-12-31";
            int li_limit = _customHelper.getMergedPropertyInt("li_limit", ldictionary_mergedResult) ?? 50;
            string ls_zona = _customHelper.getMergedPropertyString("ls_zona", ldictionary_mergedResult) ?? "";
            //Console.WriteLine(JsonSerializer.Serialize(ldictionary_mergedResult));


            string ls_settingsjson_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_clientes_table");
            string ls_aux_error_message = "";
            int li_total_rows = 0;


            // QUERY USING SQLEXECUTOR SCALAR SQLMANUALLY
            string sqlQuery = "select COUNT(*) as total_query_rows FROM dba." + ls_settingsjson_table;
            List<object> minimalRequiredModelParams = new List<object> { };


            sqlQuery += " where id > :afteridArgument";
            minimalRequiredModelParams.Add(li_after_id);

            if (ls_fec_registro_min.Length > 0)
            {
                sqlQuery += " and fec_registro >= :fecregistroArgument";
                minimalRequiredModelParams.Add(ls_fec_registro_min);
            }

            if (ls_zona.Length > 0)
            {
                sqlQuery += " and zona= :zonaArgument";
                minimalRequiredModelParams.Add(ls_zona);
            }

            // Is required | Convert objectList to objectArray
            object[] paramsArrayObject = minimalRequiredModelParams.ToArray();

            try
            {
                // Totalizado por modelo - pero sin poder agregar mas parametros
                //li_total_rows = _dataContext.SqlModelMapper.Count<ClienteSnapByFiltered>(li_after_id, ls_fec_registro_min);               

                // Totalizado por sqlManually
                li_total_rows = await _dataContext.SqlExecutor.ScalarAsync<int>(sqlQuery, paramsArrayObject);
            }
            // RESPONSE DB ERROR
            catch (Exception ex)
            {
                ls_aux_error_message = $"No fue posible realizar la consulta a: " + ls_settingsjson_table;
                objFnRes.Message = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_aux_error_message + ".  Mensaje: " + ex.Message;
                objFnRes.Success = false;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Success = true;
            objFnRes.Data.Add(li_total_rows);

            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }
        // CLIENTES END



        public async Task<string> QueryUsersFilteredAzync(Dictionary<string, object> la_params)
        {
            ObjFnResBdQueryUsuariosFilteredAzync objFnRes = new ObjFnResBdQueryUsuariosFilteredAzync()
            {
                Success = false,
                Message = "",
                Data = new List<UsuarioSnapFilteredCProject>()
            };



            // define an array of parameters expected to be received. [ like an asociative array but in csharp is a dictionary ]
            Dictionary<string, object> la_params_default = new Dictionary<string, object>
            {
                { "li_limit", 1 }
            };
            Dictionary<string, object> ldictionary_mergedResult = _customHelper.MergeArrays(la_params_default, la_params); // merge the params


            // SET MERGED VALUES            
            int li_limit = _customHelper.getMergedPropertyInt("li_limit", ldictionary_mergedResult) ?? 50;
            //Console.WriteLine(JsonSerializer.Serialize(ldictionary_mergedResult));


            //string ls_settingsjson_bionegocios_clientes_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_clientes_table");
            string ls_settingsjson_bionegocios_clientes_table = "usuarios";
            string ls_aux_error_message = "";
            List<UsuarioSnapFilteredCProject> clienteAseList = new List<UsuarioSnapFilteredCProject>();


            // QUERY USING SNAPOBJECTS SQLMODELMAPPER AND EXTRA BUILDER
            var Builder = _dataContext.SqlModelMapper.GetQueryBuilder<UsuarioSnapFilteredCProject>();

            List<object> minimalRequiredModelParams = new List<object> { }; // No required params actualmente
                                                                            //if (ls_zona.Length > 0)
                                                                            //{
                                                                            //    Builder.AndWhere("zona", SqlBinaryOperator.Equals, SqlBuilder.Parameter<string>("zonaArgument"));
                                                                            //    minimalRequiredModelParams.Add(ls_zona);
                                                                            //}


            object[] paramsArrayObject = minimalRequiredModelParams.ToArray(); // Convert objectList to objectArray
            try
            {
                clienteAseList = (await Builder.LoadByPageAsync(0, li_limit, paramsArrayObject)).ToList();
            }
            // RESPONSE DB ERROR
            catch (Exception ex)
            {
                ls_aux_error_message = $"No fue posible realizar la consulta a: " + ls_settingsjson_bionegocios_clientes_table;
                objFnRes.Message = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_aux_error_message + ".  Mensaje: " + ex.Message;
                objFnRes.Success = false;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Success = true;
            objFnRes.Data = clienteAseList;
            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }


        // ===== TIPOS NEGOCIO
        public async Task<string> QueryTiposNegocioFilteredAzync(Dictionary<string, object> la_params)
        {
            // INICIALIZAMOS VALORES DEFECTO DE RESPUESTA
            FnResQueryTiposNegocioFilteredAzync objFnRes = new()
            {
                Ksuccess = false,
                Kmessage = "",
                Kdata = new List<TiposNegocioFilteredSnapModel>()
            };

            // INICIALIZAMOS VARIABLES DE ERROR
            string ls_settingsjson_bionegocios_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_tipos_negocio_table");
            string ls_err_msg = "";


            // ARRAY MERGE
            Dictionary<string, object> la_params_default = new Dictionary<string, object>
            {
                { "li_after_id", -1 },
                { "ls_fec_registro_min", "" }, // 1800-12-31
                { "li_limit", 50 }
            };
            Dictionary<string, object> ldictionary_mergedResult = _customHelper.MergeArrays(la_params_default, la_params); // merge the params


            // GET MERGED VALUES
            int li_after_id = _customHelper.getMergedPropertyInt("li_after_id", ldictionary_mergedResult) ?? -1;
            string ls_fec_registro_min = _customHelper.getMergedPropertyString("ls_fec_registro_min", ldictionary_mergedResult) ?? "1900-12-31";
            int li_limit = _customHelper.getMergedPropertyInt("li_limit", ldictionary_mergedResult) ?? 50;


            // ===== QUERY USING SNAPOBJECTS SQLMODELMAPPER AND EXTRA BUILDER                                    
            var Builder = _dataContext.SqlModelMapper.GetQueryBuilder<TiposNegocioFilteredSnapModel>(); // Order by "id" is defined in currentModel
            List<object> minimalRequiredModelParams = new List<object> { };


            Builder.Where("id", SqlBinaryOperator.GreaterThan, SqlBuilder.Parameter<int>("idArgument"));
            minimalRequiredModelParams.Add(li_after_id);

            if (ls_fec_registro_min.Length > 0)
            {
                Builder.AndWhere("fec_registro", SqlBinaryOperator.GreaterThanOrEquals, SqlBuilder.Parameter<string>("fecregistroArgument"));
                minimalRequiredModelParams.Add(ls_fec_registro_min);
            }


            // Casteamos los parametros a formato array pues es requerido
            object[] paramsArrayObject = minimalRequiredModelParams.ToArray();
            List<TiposNegocioFilteredSnapModel> la_registros_ase = new List<TiposNegocioFilteredSnapModel>();


            try
            {
                la_registros_ase = (await Builder.LoadByPageAsync(0, li_limit, paramsArrayObject)).ToList();
            }
            catch (Exception ex)
            {
                ls_err_msg = $"No fue posible realizar la consulta a: " + ls_settingsjson_bionegocios_table;

                objFnRes.Ksuccess = false;
                objFnRes.Kmessage = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_err_msg + ".  Mensaje: " + ex.Message;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Ksuccess = true;
            objFnRes.Kmessage = "Query realizada con exito.";
            objFnRes.Kdata = la_registros_ase;
            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }


        public async Task<string> QueryTiposNegocioFilteredTotalRowsAzync(Dictionary<string, object> la_params)
        {
            FnResQueryTiposNegocioFilteredTotalRowsAzync objFnRes = new()
            {
                Ksuccess = false,
                Kmessage = "",
                Kdata = new List<int>()
            };

            // define an array of parameters expected to be received. [ like an asociative array but in csharp is a dictionary ]
            Dictionary<string, object> la_params_default = new Dictionary<string, object>
            {
                { "li_after_id", -1 },
                { "ls_fec_registro_min", "" }, // 1800-12-31
                { "li_limit", 50 }
            };
            Dictionary<string, object> ldictionary_mergedResult = _customHelper.MergeArrays(la_params_default, la_params); // merge the params


            int li_after_id = _customHelper.getMergedPropertyInt("li_after_id", ldictionary_mergedResult) ?? -1;
            string ls_fec_registro_min = _customHelper.getMergedPropertyString("ls_fec_registro_min", ldictionary_mergedResult) ?? "1900-12-31";
            int li_limit = _customHelper.getMergedPropertyInt("li_limit", ldictionary_mergedResult) ?? 50;
            //Console.WriteLine(JsonSerializer.Serialize(ldictionary_mergedResult));


            string ls_settingsjson_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_tipos_negocio_table");
            string ls_err_msg = "";
            int li_total_rows = 0;


            // QUERY USING SQLEXECUTOR SCALAR SQLMANUALLY
            string sqlQuery = "select COUNT(*) as total_query_rows FROM dba." + ls_settingsjson_table;
            List<object> minimalRequiredModelParams = new List<object> { };


            sqlQuery += " where id > :afteridArgument";
            minimalRequiredModelParams.Add(li_after_id);

            if (ls_fec_registro_min.Length > 0)
            {
                sqlQuery += " and fec_registro >= :fecregistroArgument";
                minimalRequiredModelParams.Add(ls_fec_registro_min);
            }

            //if (ls_zona.Length > 0)
            //{
            //    sqlQuery += " and zona= :zonaArgument";
            //    minimalRequiredModelParams.Add(ls_zona);
            //}

            // Is required | Convert objectList to objectArray
            object[] paramsArrayObject = minimalRequiredModelParams.ToArray();

            try
            {
                // Totalizado por modelo - pero sin poder agregar mas parametros
                //li_total_rows = _dataContext.SqlModelMapper.Count<ClienteSnapByFiltered>(li_after_id, ls_fec_registro_min);               

                // Totalizado por sqlManually
                li_total_rows = await _dataContext.SqlExecutor.ScalarAsync<int>(sqlQuery, paramsArrayObject);
            }
            catch (Exception ex)
            {
                ls_err_msg = $"No fue posible realizar la consulta a: " + ls_settingsjson_table;
                objFnRes.Ksuccess = false;
                objFnRes.Kmessage = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_err_msg + ".  Mensaje: " + ex.Message;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Ksuccess = true;
            objFnRes.Kdata.Add(li_total_rows);

            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }


        public async Task<string> QueryTiposNegocioAllAzync()
        {
            ObjFnResBdQueryTiposNegocioAllAzync objFnRes = new()
            {
                Ksuccess = false,
                Kmessage = "",
                Kdata = new List<TiposNegocioSnapModel>()
            };

            string ls_settingsjson_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_tipos_negocio_table");
            string ls_aux_error_message = "";
            List<TiposNegocioSnapModel> la_registros_ase = new List<TiposNegocioSnapModel>();

            // GET ROWS using sqlmodelmapper sybase ase            
            try
            {
                la_registros_ase = (await _dataContext.SqlModelMapper.LoadAsync<TiposNegocioSnapModel>()).ToList(); // Load no retornará null - Sino un tipo Inumerable                
            }
            // RESPONSE DB ERROR
            catch (Exception ex)
            {
                ls_aux_error_message = $"No fue posible realizar la consulta a: " + ls_settingsjson_table;
                objFnRes.Kmessage = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_aux_error_message + ".  Mensaje: " + ex.Message;
                objFnRes.Ksuccess = false;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Ksuccess = true;
            objFnRes.Kdata = la_registros_ase;

            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }
        // ===== TIPOS NEGOCIO END


        // DEPARTAMENTOS
        public async Task<string> QueryDepartamentosAllAzync()
        {
            FnResBdQueryDepartamentosAllAzync objFnRes = new()
            {
                Ksuccess = false,
                Kmessage = "",
                Kdata = new List<DepartamentosSnapModel>()
            };

            string ls_settingsjson_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_departamentos_table");
            string ls_aux_error_message = "";
            List<DepartamentosSnapModel> la_registros_ase = new List<DepartamentosSnapModel>();

            // GET ROWS using sqlmodelmapper sybase ase            
            try
            {
                la_registros_ase = (await _dataContext.SqlModelMapper.LoadAsync<DepartamentosSnapModel>()).ToList(); // Load no retornará null - Sino un tipo Inumerable                
            }
            // RESPONSE DB ERROR
            catch (Exception ex)
            {
                ls_aux_error_message = $"No fue posible realizar la consulta a: " + ls_settingsjson_table;
                objFnRes.Kmessage = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_aux_error_message + ".  Mensaje: " + ex.Message;
                objFnRes.Ksuccess = false;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Ksuccess = true;
            objFnRes.Kdata = la_registros_ase;

            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }


        public async Task<string> QueryDepartamentosFilteredAzync(Dictionary<string, object> la_params)
        {
            // INICIALIZAMOS VALORES DEFECTO DE RESPUESTA
            FnResQueryDepartamentosFilteredAzync objFnRes = new()
            {
                Ksuccess = false,
                Kmessage = "",
                Kdata = new List<DepartamentosFilteredSnapModel>()
            };

            // INICIALIZAMOS VARIABLES DE ERROR
            string ls_settingsjson_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_departamentos_table");
            string ls_err_msg = "";


            // ARRAY MERGE
            Dictionary<string, object> la_params_default = new Dictionary<string, object>
            {
                { "li_after_id", -1 },
                { "ls_fec_registro_min", "" }, // 1800-12-31
                { "li_limit", 50 }
            };
            Dictionary<string, object> ldictionary_mergedResult = _customHelper.MergeArrays(la_params_default, la_params); // merge the params


            // GET MERGED VALUES
            int li_after_id = _customHelper.getMergedPropertyInt("li_after_id", ldictionary_mergedResult) ?? -1;
            string ls_fec_registro_min = _customHelper.getMergedPropertyString("ls_fec_registro_min", ldictionary_mergedResult) ?? "1900-12-31";
            int li_limit = _customHelper.getMergedPropertyInt("li_limit", ldictionary_mergedResult) ?? 50;


            // ===== QUERY USING SNAPOBJECTS SQLMODELMAPPER AND EXTRA BUILDER                                    
            var Builder = _dataContext.SqlModelMapper.GetQueryBuilder<DepartamentosFilteredSnapModel>(); // Order by "id" is defined in currentModel
            List<object> minimalRequiredModelParams = new List<object> { };


            Builder.Where("id", SqlBinaryOperator.GreaterThan, SqlBuilder.Parameter<int>("idArgument"));
            minimalRequiredModelParams.Add(li_after_id);

            if (ls_fec_registro_min.Length > 0)
            {
                Builder.AndWhere("fec_registro", SqlBinaryOperator.GreaterThanOrEquals, SqlBuilder.Parameter<string>("fecregistroArgument"));
                minimalRequiredModelParams.Add(ls_fec_registro_min);
            }


            // Casteamos los parametros a formato array pues es requerido
            object[] paramsArrayObject = minimalRequiredModelParams.ToArray();
            List<DepartamentosFilteredSnapModel> la_registros_ase = new List<DepartamentosFilteredSnapModel>();


            try
            {
                la_registros_ase = (await Builder.LoadByPageAsync(0, li_limit, paramsArrayObject)).ToList();
            }
            catch (Exception ex)
            {
                ls_err_msg = $"No fue posible realizar la consulta a: " + ls_settingsjson_table;

                objFnRes.Ksuccess = false;
                objFnRes.Kmessage = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_err_msg + ".  Mensaje: " + ex.Message;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Ksuccess = true;
            objFnRes.Kmessage = "Query realizada con exito.";
            objFnRes.Kdata = la_registros_ase;
            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }


        public async Task<string> QueryDepartamentosFilteredTotalRowsAzync(Dictionary<string, object> la_params)
        {
            // REUTILIZAMOS AAAModelDefaultFnResFilteredTotalRowsAzync
            FnResQueryAAAModelDefaultFnResFTRowsAzync objFnRes = new()
            {
                Ksuccess = false,
                Kmessage = "",
                Kdata = new List<int>()
            };

            // define an array of parameters expected to be received. [ like an asociative array but in csharp is a dictionary ]
            Dictionary<string, object> la_params_default = new Dictionary<string, object>
            {
                { "li_after_id", -1 },
                { "ls_fec_registro_min", "" }, // 1800-12-31
                { "li_limit", 50 }
            };
            Dictionary<string, object> ldictionary_mergedResult = _customHelper.MergeArrays(la_params_default, la_params); // merge the params


            int li_after_id = _customHelper.getMergedPropertyInt("li_after_id", ldictionary_mergedResult) ?? -1;
            string ls_fec_registro_min = _customHelper.getMergedPropertyString("ls_fec_registro_min", ldictionary_mergedResult) ?? "1900-12-31";
            int li_limit = _customHelper.getMergedPropertyInt("li_limit", ldictionary_mergedResult) ?? 50;
            //Console.WriteLine(JsonSerializer.Serialize(ldictionary_mergedResult));


            string ls_settingsjson_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_departamentos_table");
            string ls_err_msg = "";
            int li_total_rows = 0;


            // QUERY USING SQLEXECUTOR SCALAR SQLMANUALLY
            string sqlQuery = "select COUNT(*) as total_query_rows FROM dba." + ls_settingsjson_table;
            List<object> minimalRequiredModelParams = new List<object> { };


            sqlQuery += " where id > :afteridArgument";
            minimalRequiredModelParams.Add(li_after_id);

            if (ls_fec_registro_min.Length > 0)
            {
                sqlQuery += " and fec_registro >= :fecregistroArgument";
                minimalRequiredModelParams.Add(ls_fec_registro_min);
            }

            //if (ls_zona.Length > 0)
            //{
            //    sqlQuery += " and zona= :zonaArgument";
            //    minimalRequiredModelParams.Add(ls_zona);
            //}

            // Is required | Convert objectList to objectArray
            object[] paramsArrayObject = minimalRequiredModelParams.ToArray();

            try
            {
                // Totalizado por modelo - pero sin poder agregar mas parametros
                //li_total_rows = _dataContext.SqlModelMapper.Count<ClienteSnapByFiltered>(li_after_id, ls_fec_registro_min);               

                // Totalizado por sqlManually
                li_total_rows = await _dataContext.SqlExecutor.ScalarAsync<int>(sqlQuery, paramsArrayObject);
            }
            catch (Exception ex)
            {
                ls_err_msg = $"No fue posible realizar la consulta a: " + ls_settingsjson_table;
                objFnRes.Ksuccess = false;
                objFnRes.Kmessage = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_err_msg + ".  Mensaje: " + ex.Message;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Ksuccess = true;
            objFnRes.Kdata.Add(li_total_rows);

            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }
        // ===== DEPARTAMENTOS END


        // ===== MUNICIPIOS
        public async Task<string> QueryMunicipiosAllAzync()
        {
            FnResBdQueryMunicipiosAllAzync objFnRes = new()
            {
                Ksuccess = false,
                Kmessage = "",
                Kdata = new List<MunicipiosSnapModel>()
            };

            string ls_settingsjson_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_municipios_table");
            string ls_aux_error_message = "";
            List<MunicipiosSnapModel> la_registros_ase = new List<MunicipiosSnapModel>();

            // GET ROWS using sqlmodelmapper sybase ase            
            try
            {
                la_registros_ase = (await _dataContext.SqlModelMapper.LoadAsync<MunicipiosSnapModel>()).ToList(); // Load no retornará null - Sino un tipo Inumerable                
            }
            // RESPONSE DB ERROR
            catch (Exception ex)
            {
                ls_aux_error_message = $"No fue posible realizar la consulta a: " + ls_settingsjson_table;
                objFnRes.Kmessage = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_aux_error_message + ".  Mensaje: " + ex.Message;
                objFnRes.Ksuccess = false;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Ksuccess = true;
            objFnRes.Kdata = la_registros_ase;

            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }


        public async Task<string> QueryMunicipiosFilteredAzync(Dictionary<string, object> la_params)
        {
            // INICIALIZAMOS VALORES DEFECTO DE RESPUESTA
            FnResQueryMunicipiosFilteredAzync objFnRes = new()
            {
                Ksuccess = false,
                Kmessage = "",
                Kdata = new List<MunicipiosFilteredSnapModel>(),
                Kfiltered_params = new Dictionary<string, object>() // array asociative empty
            };

            // INICIALIZAMOS VARIABLES DE ERROR
            string ls_settingsjson_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_municipios_table");
            string ls_err_msg = "";


            // ARRAY MERGE
            Dictionary<string, object> la_params_default = new Dictionary<string, object>
            {
                { "li_after_id", -1 },
                { "ls_fec_registro_min", "" }, // 1800-12-31
                { "li_limit", 50 }
            };
            Dictionary<string, object> ldictionary_mergedResult = _customHelper.MergeArrays(la_params_default, la_params); // merge the params


            // GET MERGED VALUES
            int li_after_id = _customHelper.getMergedPropertyInt("li_after_id", ldictionary_mergedResult) ?? -1;
            string ls_fec_registro_min = _customHelper.getMergedPropertyString("ls_fec_registro_min", ldictionary_mergedResult) ?? "1900-12-31";
            int li_limit = _customHelper.getMergedPropertyInt("li_limit", ldictionary_mergedResult) ?? 50;


            // ===== QUERY USING SNAPOBJECTS SQLMODELMAPPER AND EXTRA BUILDER                                    
            var Builder = _dataContext.SqlModelMapper.GetQueryBuilder<MunicipiosFilteredSnapModel>(); // Order by "id" is defined in currentModel
            List<object> minimalRequiredModelParams = new List<object> { };


            Builder.Where("id", SqlBinaryOperator.GreaterThan, SqlBuilder.Parameter<int>("idArgument"));
            minimalRequiredModelParams.Add(li_after_id);
            objFnRes.Kfiltered_params.Add("li_after_id", li_after_id); // For testing pourpose mode - by devs ;)

            if (ls_fec_registro_min.Length > 0)
            {
                Builder.AndWhere("fec_registro", SqlBinaryOperator.GreaterThanOrEquals, SqlBuilder.Parameter<string>("fecregistroArgument"));
                minimalRequiredModelParams.Add(ls_fec_registro_min);
                objFnRes.Kfiltered_params.Add("ls_fec_registro_min", ls_fec_registro_min);// For testing pourpose mode - by devs ;)
            }


            // Casteamos los parametros a formato array pues es requerido
            object[] paramsArrayObject = minimalRequiredModelParams.ToArray();
            List<MunicipiosFilteredSnapModel> la_registros_ase = new List<MunicipiosFilteredSnapModel>();


            try
            {
                la_registros_ase = (await Builder.LoadByPageAsync(0, li_limit, paramsArrayObject)).ToList();
                objFnRes.Kfiltered_params.Add("li_limit", li_limit - 1); // li_limit -1 because Last record will be removed
            }
            catch (Exception ex)
            {
                ls_err_msg = $"No fue posible realizar la consulta a: " + ls_settingsjson_table;

                objFnRes.Ksuccess = false;
                objFnRes.Kmessage = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_err_msg + ".  Mensaje: " + ex.Message;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Ksuccess = true;
            objFnRes.Kmessage = "Query realizada con exito.";

            objFnRes.Kdata = la_registros_ase;
            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }


        public async Task<string> QueryMunicipiosFilteredTotalRowsAzync(Dictionary<string, object> la_params)
        {
            // REUTILIZAMOS AAAModelDefaultFnResFilteredTotalRowsAzync
            FnResQueryAAAModelDefaultFnResFTRowsAzync objFnRes = new()
            {
                Ksuccess = false,
                Kmessage = "",
                Kdata = new List<int>()
            };

            // define an array of parameters expected to be received. [ like an asociative array but in csharp is a dictionary ]
            Dictionary<string, object> la_params_default = new Dictionary<string, object>
            {
                { "li_after_id", -1 },
                { "ls_fec_registro_min", "" }, // 1800-12-31
                { "li_limit", 50 }
            };
            Dictionary<string, object> ldictionary_mergedResult = _customHelper.MergeArrays(la_params_default, la_params); // merge the params


            int li_after_id = _customHelper.getMergedPropertyInt("li_after_id", ldictionary_mergedResult) ?? -1;
            string ls_fec_registro_min = _customHelper.getMergedPropertyString("ls_fec_registro_min", ldictionary_mergedResult) ?? "1900-12-31";
            int li_limit = _customHelper.getMergedPropertyInt("li_limit", ldictionary_mergedResult) ?? 50;
            //Console.WriteLine(JsonSerializer.Serialize(ldictionary_mergedResult));


            string ls_settingsjson_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_municipios_table");
            string ls_err_msg = "";
            int li_total_rows = 0;


            // QUERY USING SQLEXECUTOR SCALAR SQLMANUALLY
            string sqlQuery = "select COUNT(*) as total_query_rows FROM dba." + ls_settingsjson_table;
            List<object> minimalRequiredModelParams = new List<object> { };


            sqlQuery += " where id > :afteridArgument";
            minimalRequiredModelParams.Add(li_after_id);

            if (ls_fec_registro_min.Length > 0)
            {
                sqlQuery += " and fec_registro >= :fecregistroArgument";
                minimalRequiredModelParams.Add(ls_fec_registro_min);
            }

            //if (ls_zona.Length > 0)
            //{
            //    sqlQuery += " and zona= :zonaArgument";
            //    minimalRequiredModelParams.Add(ls_zona);
            //}

            // Is required | Convert objectList to objectArray
            object[] paramsArrayObject = minimalRequiredModelParams.ToArray();

            try
            {
                // Totalizado por modelo - pero sin poder agregar mas parametros
                //li_total_rows = _dataContext.SqlModelMapper.Count<ClienteSnapByFiltered>(li_after_id, ls_fec_registro_min);               

                // Totalizado por sqlManually
                li_total_rows = await _dataContext.SqlExecutor.ScalarAsync<int>(sqlQuery, paramsArrayObject);
            }
            catch (Exception ex)
            {
                ls_err_msg = $"No fue posible realizar la consulta a: " + ls_settingsjson_table;
                objFnRes.Ksuccess = false;
                objFnRes.Kmessage = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_err_msg + ".  Mensaje: " + ex.Message;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Ksuccess = true;
            objFnRes.Kdata.Add(li_total_rows);

            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }
        // ===== MUNICIPIOS END


        // ===== USUARIOS MOVIL
        public async Task<string> QueryCmUsuariosMovilAllAzync()
        {
            var objFnRes = new FnResBdQueryCmUsuariosMovilAllAzync()
            {
                Ksuccess = false,
                Kmessage = "",
                Kdata = new List<UsuariosMovilSnapModel>()
            };

            string ls_settingsjson_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_usuarios_movil_table");
            string ls_aux_error_message = "";
            var la_registros_ase = new List<UsuariosMovilSnapModel>();

            // GET ROWS using sqlmodelmapper sybase ase
            try
            {
                la_registros_ase = (await _dataContext.SqlModelMapper.LoadAsync<UsuariosMovilSnapModel>()).ToList(); // Load no retornará null - Sino un tipo Inumerable                
            }
            // RESPONSE DB ERROR
            catch (Exception ex)
            {
                ls_aux_error_message = $"No fue posible realizar la consulta a: " + ls_settingsjson_table;
                objFnRes.Kmessage = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_aux_error_message + ".  Mensaje: " + ex.Message;
                objFnRes.Ksuccess = false;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Ksuccess = true;
            objFnRes.Kdata = la_registros_ase;

            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }


        public async Task<string> QueryCmUsuariosMovilFilteredAzync(Dictionary<string, object> la_params)
        {
            var objFnRes = new FnResQueryCmUsuariosMovilFilteredAzync()
            {
                Ksuccess = false,
                Kmessage = "",
                Kdata = new List<UsuariosMovilFilteredSnapModel>(),
                Kfiltered_params = new Dictionary<string, object>() // array asociative empty
            };

            // INICIALIZAMOS VARIABLES DE ERROR
            string ls_settingsjson_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_usuarios_movil_table");
            string ls_err_msg = "";


            // ARRAY MERGE
            Dictionary<string, object> la_params_default = new Dictionary<string, object>
            {
                { "li_after_id", -1 },
                { "ls_fec_registro_min", "" }, // 1800-12-31
                { "li_limit", 50 }
            };
            Dictionary<string, object> ldictionary_mergedResult = _customHelper.MergeArrays(la_params_default, la_params); // merge the params


            // GET MERGED VALUES
            int li_after_id = _customHelper.getMergedPropertyInt("li_after_id", ldictionary_mergedResult) ?? -1;
            string ls_fec_registro_min = _customHelper.getMergedPropertyString("ls_fec_registro_min", ldictionary_mergedResult) ?? "1900-12-31";
            int li_limit = _customHelper.getMergedPropertyInt("li_limit", ldictionary_mergedResult) ?? 50;


            // ===== QUERY USING SNAPOBJECTS SQLMODELMAPPER AND EXTRA BUILDER                                    
            var Builder = _dataContext.SqlModelMapper.GetQueryBuilder<UsuariosMovilFilteredSnapModel>(); // Order by "id" is defined in currentModel
            List<object> minimalRequiredModelParams = new List<object> { };


            Builder.Where("id", SqlBinaryOperator.GreaterThan, SqlBuilder.Parameter<int>("idArgument"));
            minimalRequiredModelParams.Add(li_after_id);
            objFnRes.Kfiltered_params.Add("li_after_id", li_after_id); // For testing pourpose mode - by devs ;)

            if (ls_fec_registro_min.Length > 0)
            {
                Builder.AndWhere("fec_registro", SqlBinaryOperator.GreaterThanOrEquals, SqlBuilder.Parameter<string>("fecregistroArgument"));
                minimalRequiredModelParams.Add(ls_fec_registro_min);
                objFnRes.Kfiltered_params.Add("ls_fec_registro_min", ls_fec_registro_min);// For testing pourpose mode - by devs ;)
            }


            // Casteamos los parametros a formato array pues es requerido
            object[] paramsArrayObject = minimalRequiredModelParams.ToArray();
            var la_registros_ase = new List<UsuariosMovilFilteredSnapModel>();


            try
            {
                la_registros_ase = (await Builder.LoadByPageAsync(0, li_limit, paramsArrayObject)).ToList();
                objFnRes.Kfiltered_params.Add("li_limit", li_limit - 1); // li_limit -1 because Last record will be removed
            }
            catch (Exception ex)
            {
                ls_err_msg = $"No fue posible realizar la consulta a: " + ls_settingsjson_table;

                objFnRes.Ksuccess = false;
                objFnRes.Kmessage = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_err_msg + ".  Mensaje: " + ex.Message;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Ksuccess = true;
            objFnRes.Kmessage = "Query realizada con exito.";

            objFnRes.Kdata = la_registros_ase;
            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }


        public async Task<string> QueryCmUsuariosMovilFilteredTotalRowsAzync(Dictionary<string, object> la_params)
        {
            // REUTILIZAMOS AAAModelDefaultFnResFilteredTotalRowsAzync
            FnResQueryAAAModelDefaultFnResFTRowsAzync objFnRes = new()
            {
                Ksuccess = false,
                Kmessage = "",
                Kdata = new List<int>()
            };

            // define an array of parameters expected to be received. [ like an asociative array but in csharp is a dictionary ]
            Dictionary<string, object> la_params_default = new Dictionary<string, object>
            {
                { "li_after_id", -1 },
                { "ls_fec_registro_min", "" }, // 1800-12-31
                { "li_limit", 50 }
            };
            Dictionary<string, object> ldictionary_mergedResult = _customHelper.MergeArrays(la_params_default, la_params); // merge the params


            int li_after_id = _customHelper.getMergedPropertyInt("li_after_id", ldictionary_mergedResult) ?? -1;
            string ls_fec_registro_min = _customHelper.getMergedPropertyString("ls_fec_registro_min", ldictionary_mergedResult) ?? "1900-12-31";
            int li_limit = _customHelper.getMergedPropertyInt("li_limit", ldictionary_mergedResult) ?? 50;
            //Console.WriteLine(JsonSerializer.Serialize(ldictionary_mergedResult));


            string ls_settingsjson_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_usuarios_movil_table");
            string ls_err_msg = "";
            int li_total_rows = 0;


            // QUERY USING SQLEXECUTOR SCALAR SQLMANUALLY
            string sqlQuery = "select COUNT(*) as total_query_rows FROM dba." + ls_settingsjson_table;
            List<object> minimalRequiredModelParams = new List<object> { };


            sqlQuery += " where id > :afteridArgument";
            minimalRequiredModelParams.Add(li_after_id);

            if (ls_fec_registro_min.Length > 0)
            {
                sqlQuery += " and fec_registro >= :fecregistroArgument";
                minimalRequiredModelParams.Add(ls_fec_registro_min);
            }

            //if (ls_zona.Length > 0)
            //{
            //    sqlQuery += " and zona= :zonaArgument";
            //    minimalRequiredModelParams.Add(ls_zona);
            //}

            // Is required | Convert objectList to objectArray
            object[] paramsArrayObject = minimalRequiredModelParams.ToArray();

            try
            {
                li_total_rows = await _dataContext.SqlExecutor.ScalarAsync<int>(sqlQuery, paramsArrayObject);
            }
            catch (Exception ex)
            {
                ls_err_msg = $"No fue posible realizar la consulta a: " + ls_settingsjson_table;
                objFnRes.Ksuccess = false;
                objFnRes.Kmessage = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_err_msg + ".  Mensaje: " + ex.Message;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Ksuccess = true;
            objFnRes.Kdata.Add(li_total_rows);

            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }
        // ===== USUARIOS MOVIL END

        // ===== LINEAS PRODUCTOS
        public async Task<string> QueryCmLineasProductosAllAzync()
        {
            var objFnRes = new FnResBdQueryCmLineasProductosAllAzync()
            {
                Ksuccess = false,
                Kmessage = "",
                Kdata = new List<LineasProductosSnapModel>()
            };

            string ls_settingsjson_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_lineas_productos_table");
            string ls_aux_error_message = "";
            var la_registros_ase = new List<LineasProductosSnapModel>();

            // GET ROWS using sqlmodelmapper sybase ase
            try
            {
                la_registros_ase = (await _dataContext.SqlModelMapper.LoadAsync<LineasProductosSnapModel>()).ToList(); // Load no retornará null - Sino un tipo Inumerable                
            }
            // RESPONSE DB ERROR
            catch (Exception ex)
            {
                ls_aux_error_message = $"No fue posible realizar la consulta a: " + ls_settingsjson_table;
                objFnRes.Kmessage = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_aux_error_message + ".  Mensaje: " + ex.Message;
                objFnRes.Ksuccess = false;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Ksuccess = true;
            objFnRes.Kdata = la_registros_ase;

            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }


        public async Task<string> QueryCmLineasProductosFilteredAzync(Dictionary<string, object> la_params)
        {
            var objFnRes = new FnResQueryCmLineasProductosFilteredAzync()
            {
                Ksuccess = false,
                Kmessage = "",
                Kdata = new List<LineasProductosFilteredSnapModel>(),
                Kfiltered_params = new Dictionary<string, object>() // array asociative empty
            };

            // INICIALIZAMOS VARIABLES DE ERROR
            string ls_settingsjson_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_lineas_productos_table");
            string ls_err_msg = "";


            // ARRAY MERGE
            Dictionary<string, object> la_params_default = new Dictionary<string, object>
            {
                { "li_after_id", -1 },
                { "ls_fec_registro_min", "" }, // 1800-12-31
                { "li_limit", 50 }
            };
            Dictionary<string, object> ldictionary_mergedResult = _customHelper.MergeArrays(la_params_default, la_params); // merge the params


            // GET MERGED VALUES
            int li_after_id = _customHelper.getMergedPropertyInt("li_after_id", ldictionary_mergedResult) ?? -1;
            string ls_fec_registro_min = _customHelper.getMergedPropertyString("ls_fec_registro_min", ldictionary_mergedResult) ?? "1900-12-31";
            int li_limit = _customHelper.getMergedPropertyInt("li_limit", ldictionary_mergedResult) ?? 50;


            // ===== QUERY USING SNAPOBJECTS SQLMODELMAPPER AND EXTRA BUILDER                                    
            var Builder = _dataContext.SqlModelMapper.GetQueryBuilder<LineasProductosFilteredSnapModel>(); // Order by "id" is defined in currentModel
            List<object> minimalRequiredModelParams = new List<object> { };


            Builder.Where("id", SqlBinaryOperator.GreaterThan, SqlBuilder.Parameter<int>("idArgument"));
            minimalRequiredModelParams.Add(li_after_id);
            objFnRes.Kfiltered_params.Add("li_after_id", li_after_id); // For testing pourpose mode - by devs ;)

            if (ls_fec_registro_min.Length > 0)
            {
                Builder.AndWhere("fec_registro", SqlBinaryOperator.GreaterThanOrEquals, SqlBuilder.Parameter<string>("fecregistroArgument"));
                minimalRequiredModelParams.Add(ls_fec_registro_min);
                objFnRes.Kfiltered_params.Add("ls_fec_registro_min", ls_fec_registro_min);// For testing pourpose mode - by devs ;)
            }


            // Casteamos los parametros a formato array pues es requerido
            object[] paramsArrayObject = minimalRequiredModelParams.ToArray();
            var la_registros_ase = new List<LineasProductosFilteredSnapModel>();


            try
            {
                la_registros_ase = (await Builder.LoadByPageAsync(0, li_limit, paramsArrayObject)).ToList();
                objFnRes.Kfiltered_params.Add("li_limit", li_limit - 1); // li_limit -1 because Last record will be removed
            }
            catch (Exception ex)
            {
                ls_err_msg = $"No fue posible realizar la consulta a: " + ls_settingsjson_table;

                objFnRes.Ksuccess = false;
                objFnRes.Kmessage = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_err_msg + ".  Mensaje: " + ex.Message;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Ksuccess = true;
            objFnRes.Kmessage = "Query realizada con exito.";

            objFnRes.Kdata = la_registros_ase;
            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }


        public async Task<string> QueryCmLineasProductosFilteredTotalRowsAzync(Dictionary<string, object> la_params)
        {
            // REUTILIZAMOS AAAModelDefaultFnResFilteredTotalRowsAzync
            FnResQueryAAAModelDefaultFnResFTRowsAzync objFnRes = new()
            {
                Ksuccess = false,
                Kmessage = "",
                Kdata = new List<int>()
            };

            // define an array of parameters expected to be received. [ like an asociative array but in csharp is a dictionary ]
            Dictionary<string, object> la_params_default = new Dictionary<string, object>
            {
                { "li_after_id", -1 },
                { "ls_fec_registro_min", "" }, // 1800-12-31
                { "li_limit", 50 }
            };
            Dictionary<string, object> ldictionary_mergedResult = _customHelper.MergeArrays(la_params_default, la_params); // merge the params


            int li_after_id = _customHelper.getMergedPropertyInt("li_after_id", ldictionary_mergedResult) ?? -1;
            string ls_fec_registro_min = _customHelper.getMergedPropertyString("ls_fec_registro_min", ldictionary_mergedResult) ?? "1900-12-31";
            int li_limit = _customHelper.getMergedPropertyInt("li_limit", ldictionary_mergedResult) ?? 50;
            //Console.WriteLine(JsonSerializer.Serialize(ldictionary_mergedResult));


            string ls_settingsjson_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_lineas_productos_table");
            string ls_err_msg = "";
            int li_total_rows = 0;


            // QUERY USING SQLEXECUTOR SCALAR SQLMANUALLY
            string sqlQuery = "select COUNT(*) as total_query_rows FROM dba." + ls_settingsjson_table;
            List<object> minimalRequiredModelParams = new List<object> { };


            sqlQuery += " where id > :afteridArgument";
            minimalRequiredModelParams.Add(li_after_id);

            if (ls_fec_registro_min.Length > 0)
            {
                sqlQuery += " and fec_registro >= :fecregistroArgument";
                minimalRequiredModelParams.Add(ls_fec_registro_min);
            }

            //if (ls_zona.Length > 0)
            //{
            //    sqlQuery += " and zona= :zonaArgument";
            //    minimalRequiredModelParams.Add(ls_zona);
            //}

            // Is required | Convert objectList to objectArray
            object[] paramsArrayObject = minimalRequiredModelParams.ToArray();

            try
            {
                li_total_rows = await _dataContext.SqlExecutor.ScalarAsync<int>(sqlQuery, paramsArrayObject);
            }
            catch (Exception ex)
            {
                ls_err_msg = $"No fue posible realizar la consulta a: " + ls_settingsjson_table;
                objFnRes.Ksuccess = false;
                objFnRes.Kmessage = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_err_msg + ".  Mensaje: " + ex.Message;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Ksuccess = true;
            objFnRes.Kdata.Add(li_total_rows);

            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }
        // ===== LINEAS PRODUCTOS END

        // ===== CMPRODUCTOS
        public async Task<string> CmProductosQueryAllAzync()
        {
            var objFnRes = new CmProductosFnResBdQueryAllAzync()
            {
                Ksuccess = false,
                Kmessage = "",
                Kdata = new List<CmProductosSnapModel>()
            };

            string ls_settingsjson_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_productos_table");
            string ls_aux_error_message = "";
            var la_registros_ase = new List<CmProductosSnapModel>();

            // GET ROWS using sqlmodelmapper sybase ase
            try
            {
                la_registros_ase = (await _dataContext.SqlModelMapper.LoadAsync<CmProductosSnapModel>()).ToList(); // Load no retornará null - Sino un tipo Inumerable                
            }
            // RESPONSE DB ERROR
            catch (Exception ex)
            {
                ls_aux_error_message = $"No fue posible realizar la consulta a: " + ls_settingsjson_table;
                objFnRes.Kmessage = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_aux_error_message + ".  Mensaje: " + ex.Message;
                objFnRes.Ksuccess = false;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Ksuccess = true;
            objFnRes.Kdata = la_registros_ase;

            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }


        public async Task<string> CmProductosQueryFilteredAzync(Dictionary<string, object> la_params)
        {
            var objFnRes = new CmProductosFnResQueryFilteredAzync()
            {
                Ksuccess = false,
                Kmessage = "",
                Kdata = new List<CmProductosFilteredSnapModel>(),
                Kfiltered_params = new Dictionary<string, object>() // array asociative empty
            };

            // INICIALIZAMOS VARIABLES DE ERROR
            string ls_settingsjson_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_productos_table");
            string ls_err_msg = "";


            // ARRAY MERGE
            Dictionary<string, object> la_params_default = new Dictionary<string, object>
            {
                { "li_after_id", -1 },
                { "ls_fec_registro_min", "" }, // 1800-12-31
                { "li_limit", 50 }
            };
            Dictionary<string, object> ldictionary_mergedResult = _customHelper.MergeArrays(la_params_default, la_params); // merge the params


            // GET MERGED VALUES
            int li_after_id = _customHelper.getMergedPropertyInt("li_after_id", ldictionary_mergedResult) ?? -1;
            string ls_fec_registro_min = _customHelper.getMergedPropertyString("ls_fec_registro_min", ldictionary_mergedResult) ?? "1900-12-31";
            int li_limit = _customHelper.getMergedPropertyInt("li_limit", ldictionary_mergedResult) ?? 50;


            // ===== QUERY USING SNAPOBJECTS SQLMODELMAPPER AND EXTRA BUILDER                                    
            var Builder = _dataContext.SqlModelMapper.GetQueryBuilder<CmProductosFilteredSnapModel>(); // Order by "id" is defined in currentModel
            List<object> minimalRequiredModelParams = new List<object> { };


            Builder.Where("id", SqlBinaryOperator.GreaterThan, SqlBuilder.Parameter<int>("idArgument"));
            minimalRequiredModelParams.Add(li_after_id);
            objFnRes.Kfiltered_params.Add("li_after_id", li_after_id); // For testing pourpose mode - by devs ;)

            if (ls_fec_registro_min.Length > 0)
            {
                Builder.AndWhere("fec_registro", SqlBinaryOperator.GreaterThanOrEquals, SqlBuilder.Parameter<string>("fecregistroArgument"));
                minimalRequiredModelParams.Add(ls_fec_registro_min);
                objFnRes.Kfiltered_params.Add("ls_fec_registro_min", ls_fec_registro_min);// For testing pourpose mode - by devs ;)
            }


            // Casteamos los parametros a formato array pues es requerido
            object[] paramsArrayObject = minimalRequiredModelParams.ToArray();
            var la_registros_ase = new List<CmProductosFilteredSnapModel>();


            try
            {
                la_registros_ase = (await Builder.LoadByPageAsync(0, li_limit, paramsArrayObject)).ToList();
                objFnRes.Kfiltered_params.Add("li_limit", li_limit - 1); // li_limit -1 because Last record will be removed
            }
            catch (Exception ex)
            {
                ls_err_msg = $"No fue posible realizar la consulta a: " + ls_settingsjson_table;

                objFnRes.Ksuccess = false;
                objFnRes.Kmessage = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_err_msg + ".  Mensaje: " + ex.Message;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Ksuccess = true;
            objFnRes.Kmessage = "Query realizada con exito.";

            objFnRes.Kdata = la_registros_ase;
            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }


        public async Task<string> CmProductosQueryFilteredTotalRowsAzync(Dictionary<string, object> la_params)
        {
            // REUTILIZAMOS AAAModelDefaultFnResFilteredTotalRowsAzync
            FnResQueryAAAModelDefaultFnResFTRowsAzync objFnRes = new()
            {
                Ksuccess = false,
                Kmessage = "",
                Kdata = new List<int>()
            };

            // define an array of parameters expected to be received. [ like an asociative array but in csharp is a dictionary ]
            Dictionary<string, object> la_params_default = new Dictionary<string, object>
            {
                { "li_after_id", -1 },
                { "ls_fec_registro_min", "" }, // 1800-12-31
                { "li_limit", 50 }
            };
            Dictionary<string, object> ldictionary_mergedResult = _customHelper.MergeArrays(la_params_default, la_params); // merge the params


            int li_after_id = _customHelper.getMergedPropertyInt("li_after_id", ldictionary_mergedResult) ?? -1;
            string ls_fec_registro_min = _customHelper.getMergedPropertyString("ls_fec_registro_min", ldictionary_mergedResult) ?? "1900-12-31";
            int li_limit = _customHelper.getMergedPropertyInt("li_limit", ldictionary_mergedResult) ?? 50;
            //Console.WriteLine(JsonSerializer.Serialize(ldictionary_mergedResult));


            string ls_settingsjson_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_productos_table");
            string ls_err_msg = "";
            int li_total_rows = 0;


            // QUERY USING SQLEXECUTOR SCALAR SQLMANUALLY
            string sqlQuery = "select COUNT(*) as total_query_rows FROM dba." + ls_settingsjson_table;
            List<object> minimalRequiredModelParams = new List<object> { };


            sqlQuery += " where id > :afteridArgument";
            minimalRequiredModelParams.Add(li_after_id);

            if (ls_fec_registro_min.Length > 0)
            {
                sqlQuery += " and fec_registro >= :fecregistroArgument";
                minimalRequiredModelParams.Add(ls_fec_registro_min);
            }

            //if (ls_zona.Length > 0)
            //{
            //    sqlQuery += " and zona= :zonaArgument";
            //    minimalRequiredModelParams.Add(ls_zona);
            //}

            // Is required | Convert objectList to objectArray
            object[] paramsArrayObject = minimalRequiredModelParams.ToArray();

            try
            {
                li_total_rows = await _dataContext.SqlExecutor.ScalarAsync<int>(sqlQuery, paramsArrayObject);
            }
            catch (Exception ex)
            {
                ls_err_msg = $"No fue posible realizar la consulta a: " + ls_settingsjson_table;
                objFnRes.Ksuccess = false;
                objFnRes.Kmessage = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_err_msg + ".  Mensaje: " + ex.Message;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Ksuccess = true;
            objFnRes.Kdata.Add(li_total_rows);

            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }
        // ===== CMPRODUCTOS END


        // ===== CMMOTIVOSNC
        public async Task<string> CmMotivosNcQueryAllAzync()
        {
            var objFnRes = new CmMotivosNcFnResBdQueryAllAzync()
            {
                Ksuccess = false,
                Kmessage = "",
                Kdata = new List<CmMotivosNcSnapModel>()
            };

            string ls_settingsjson_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_motivos_nc_table");
            string ls_aux_error_message = "";
            var la_registros_ase = new List<CmMotivosNcSnapModel>();

            // GET ROWS using sqlmodelmapper sybase ase
            try
            {
                la_registros_ase = (await _dataContext.SqlModelMapper.LoadAsync<CmMotivosNcSnapModel>()).ToList(); // Load no retornará null - Sino un tipo Inumerable                
            }
            // RESPONSE DB ERROR
            catch (Exception ex)
            {
                ls_aux_error_message = $"No fue posible realizar la consulta a: " + ls_settingsjson_table;
                objFnRes.Kmessage = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_aux_error_message + ".  Mensaje: " + ex.Message;
                objFnRes.Ksuccess = false;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Ksuccess = true;
            objFnRes.Kdata = la_registros_ase;

            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }


        public async Task<string> CmMotivosNcQueryFilteredAzync(Dictionary<string, object> la_params)
        {
            var objFnRes = new CmMotivosNcFnResQueryFilteredAzync()
            {
                Ksuccess = false,
                Kmessage = "",
                Kdata = new List<CmMotivosNcFilteredSnapModel>(),
                Kfiltered_params = new Dictionary<string, object>() // array asociative empty
            };

            // INICIALIZAMOS VARIABLES DE ERROR
            string ls_settingsjson_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_motivos_nc_table");
            string ls_err_msg = "";


            // ARRAY MERGE
            Dictionary<string, object> la_params_default = new Dictionary<string, object>
            {
                { "li_after_id", -1 },
                { "ls_fec_registro_min", "" }, // 1800-12-31
                { "li_limit", 50 }
            };
            Dictionary<string, object> ldictionary_mergedResult = _customHelper.MergeArrays(la_params_default, la_params); // merge the params


            // GET MERGED VALUES
            int li_after_id = _customHelper.getMergedPropertyInt("li_after_id", ldictionary_mergedResult) ?? -1;
            string ls_fec_registro_min = _customHelper.getMergedPropertyString("ls_fec_registro_min", ldictionary_mergedResult) ?? "1900-12-31";
            int li_limit = _customHelper.getMergedPropertyInt("li_limit", ldictionary_mergedResult) ?? 50;


            // ===== QUERY USING SNAPOBJECTS SQLMODELMAPPER AND EXTRA BUILDER                                    
            var Builder = _dataContext.SqlModelMapper.GetQueryBuilder<CmMotivosNcFilteredSnapModel>(); // Order by "id" is defined in currentModel
            List<object> minimalRequiredModelParams = new List<object> { };


            Builder.Where("id", SqlBinaryOperator.GreaterThan, SqlBuilder.Parameter<int>("idArgument"));
            minimalRequiredModelParams.Add(li_after_id);
            objFnRes.Kfiltered_params.Add("li_after_id", li_after_id); // For testing pourpose mode - by devs ;)

            if (ls_fec_registro_min.Length > 0)
            {
                Builder.AndWhere("fec_registro", SqlBinaryOperator.GreaterThanOrEquals, SqlBuilder.Parameter<string>("fecregistroArgument"));
                minimalRequiredModelParams.Add(ls_fec_registro_min);
                objFnRes.Kfiltered_params.Add("ls_fec_registro_min", ls_fec_registro_min);// For testing pourpose mode - by devs ;)
            }


            // Casteamos los parametros a formato array pues es requerido
            object[] paramsArrayObject = minimalRequiredModelParams.ToArray();
            var la_registros_ase = new List<CmMotivosNcFilteredSnapModel>();


            try
            {
                la_registros_ase = (await Builder.LoadByPageAsync(0, li_limit, paramsArrayObject)).ToList();
                objFnRes.Kfiltered_params.Add("li_limit", li_limit - 1); // li_limit -1 because Last record will be removed
            }
            catch (Exception ex)
            {
                ls_err_msg = $"No fue posible realizar la consulta a: " + ls_settingsjson_table;

                objFnRes.Ksuccess = false;
                objFnRes.Kmessage = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_err_msg + ".  Mensaje: " + ex.Message;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Ksuccess = true;
            objFnRes.Kmessage = "Query realizada con exito.";

            objFnRes.Kdata = la_registros_ase;
            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }


        public async Task<string> CmMotivosNcQueryFilteredTotalRowsAzync(Dictionary<string, object> la_params)
        {
            // REUTILIZAMOS AAAModelDefaultFnResFilteredTotalRowsAzync
            FnResQueryAAAModelDefaultFnResFTRowsAzync objFnRes = new()
            {
                Ksuccess = false,
                Kmessage = "",
                Kdata = new List<int>()
            };

            // define an array of parameters expected to be received. [ like an asociative array but in csharp is a dictionary ]
            Dictionary<string, object> la_params_default = new Dictionary<string, object>
            {
                { "li_after_id", -1 },
                { "ls_fec_registro_min", "" }, // 1800-12-31
                { "li_limit", 50 }
            };
            Dictionary<string, object> ldictionary_mergedResult = _customHelper.MergeArrays(la_params_default, la_params); // merge the params


            int li_after_id = _customHelper.getMergedPropertyInt("li_after_id", ldictionary_mergedResult) ?? -1;
            string ls_fec_registro_min = _customHelper.getMergedPropertyString("ls_fec_registro_min", ldictionary_mergedResult) ?? "1900-12-31";
            int li_limit = _customHelper.getMergedPropertyInt("li_limit", ldictionary_mergedResult) ?? 50;
            //Console.WriteLine(JsonSerializer.Serialize(ldictionary_mergedResult));


            string ls_settingsjson_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_motivos_nc_table");
            string ls_err_msg = "";
            int li_total_rows = 0;


            // QUERY USING SQLEXECUTOR SCALAR SQLMANUALLY
            string sqlQuery = "select COUNT(*) as total_query_rows FROM dba." + ls_settingsjson_table;
            List<object> minimalRequiredModelParams = new List<object> { };


            sqlQuery += " where id > :afteridArgument";
            minimalRequiredModelParams.Add(li_after_id);

            if (ls_fec_registro_min.Length > 0)
            {
                sqlQuery += " and fec_registro >= :fecregistroArgument";
                minimalRequiredModelParams.Add(ls_fec_registro_min);
            }

            //if (ls_zona.Length > 0)
            //{
            //    sqlQuery += " and zona= :zonaArgument";
            //    minimalRequiredModelParams.Add(ls_zona);
            //}

            // Is required | Convert objectList to objectArray
            object[] paramsArrayObject = minimalRequiredModelParams.ToArray();

            try
            {
                li_total_rows = await _dataContext.SqlExecutor.ScalarAsync<int>(sqlQuery, paramsArrayObject);
            }
            catch (Exception ex)
            {
                ls_err_msg = $"No fue posible realizar la consulta a: " + ls_settingsjson_table;
                objFnRes.Ksuccess = false;
                objFnRes.Kmessage = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_err_msg + ".  Mensaje: " + ex.Message;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Ksuccess = true;
            objFnRes.Kdata.Add(li_total_rows);

            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }
        // ===== CMMOTIVOSNC END


        // ===== CMZONASBIO
        public async Task<string> CmZonasBioQueryAllAzync()
        {
            var objFnRes = new CmZonasBioFnResBdQueryAllAzync()
            {
                Ksuccess = false,
                Kmessage = "",
                Kdata = new List<CmZonasBioSnapModel>()
            };

            string ls_settingsjson_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_zonas_bio_table");
            string ls_aux_error_message = "";
            var la_registros_ase = new List<CmZonasBioSnapModel>();

            // GET ROWS using sqlmodelmapper sybase ase
            try
            {
                la_registros_ase = (await _dataContext.SqlModelMapper.LoadAsync<CmZonasBioSnapModel>()).ToList(); // Load no retornará null - Sino un tipo Inumerable                
            }
            // RESPONSE DB ERROR
            catch (Exception ex)
            {
                ls_aux_error_message = $"No fue posible realizar la consulta a: " + ls_settingsjson_table;
                objFnRes.Kmessage = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_aux_error_message + ".  Mensaje: " + ex.Message;
                objFnRes.Ksuccess = false;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Ksuccess = true;
            objFnRes.Kdata = la_registros_ase;

            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }


        public async Task<string> CmZonasBioQueryFilteredAzync(Dictionary<string, object> la_params)
        {
            var objFnRes = new CmZonasBioFnResQueryFilteredAzync()
            {
                Ksuccess = false,
                Kmessage = "",
                Kdata = new List<CmZonasBioFilteredSnapModel>(),
                Kfiltered_params = new Dictionary<string, object>() // array asociative empty
            };

            // INICIALIZAMOS VARIABLES DE ERROR
            string ls_settingsjson_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_zonas_bio_table");
            string ls_err_msg = "";


            // ARRAY MERGE
            Dictionary<string, object> la_params_default = new Dictionary<string, object>
            {
                { "li_after_id", -1 },
                { "ls_fec_registro_min", "" }, // 1800-12-31
                { "li_limit", 50 }
            };
            Dictionary<string, object> ldictionary_mergedResult = _customHelper.MergeArrays(la_params_default, la_params); // merge the params


            // GET MERGED VALUES
            int li_after_id = _customHelper.getMergedPropertyInt("li_after_id", ldictionary_mergedResult) ?? -1;
            string ls_fec_registro_min = _customHelper.getMergedPropertyString("ls_fec_registro_min", ldictionary_mergedResult) ?? "1900-12-31";
            int li_limit = _customHelper.getMergedPropertyInt("li_limit", ldictionary_mergedResult) ?? 50;


            // ===== QUERY USING SNAPOBJECTS SQLMODELMAPPER AND EXTRA BUILDER                                    
            var Builder = _dataContext.SqlModelMapper.GetQueryBuilder<CmZonasBioFilteredSnapModel>(); // Order by "id" is defined in currentModel
            List<object> minimalRequiredModelParams = new List<object> { };


            Builder.Where("id", SqlBinaryOperator.GreaterThan, SqlBuilder.Parameter<int>("idArgument"));
            minimalRequiredModelParams.Add(li_after_id);
            objFnRes.Kfiltered_params.Add("li_after_id", li_after_id); // For testing pourpose mode - by devs ;)

            if (ls_fec_registro_min.Length > 0)
            {
                Builder.AndWhere("fec_registro", SqlBinaryOperator.GreaterThanOrEquals, SqlBuilder.Parameter<string>("fecregistroArgument"));
                minimalRequiredModelParams.Add(ls_fec_registro_min);
                objFnRes.Kfiltered_params.Add("ls_fec_registro_min", ls_fec_registro_min);// For testing pourpose mode - by devs ;)
            }


            // Casteamos los parametros a formato array pues es requerido
            object[] paramsArrayObject = minimalRequiredModelParams.ToArray();
            var la_registros_ase = new List<CmZonasBioFilteredSnapModel>();


            try
            {
                la_registros_ase = (await Builder.LoadByPageAsync(0, li_limit, paramsArrayObject)).ToList();
                objFnRes.Kfiltered_params.Add("li_limit", li_limit - 1); // li_limit -1 because Last record will be removed
            }
            catch (Exception ex)
            {
                ls_err_msg = $"No fue posible realizar la consulta a: " + ls_settingsjson_table;

                objFnRes.Ksuccess = false;
                objFnRes.Kmessage = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_err_msg + ".  Mensaje: " + ex.Message;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Ksuccess = true;
            objFnRes.Kmessage = "Query realizada con exito.";

            objFnRes.Kdata = la_registros_ase;
            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }


        public async Task<string> CmZonasBioQueryFilteredTotalRowsAzync(Dictionary<string, object> la_params)
        {
            // REUTILIZAMOS AAAModelDefaultFnResFilteredTotalRowsAzync
            FnResQueryAAAModelDefaultFnResFTRowsAzync objFnRes = new()
            {
                Ksuccess = false,
                Kmessage = "",
                Kdata = new List<int>()
            };

            // define an array of parameters expected to be received. [ like an asociative array but in csharp is a dictionary ]
            Dictionary<string, object> la_params_default = new Dictionary<string, object>
            {
                { "li_after_id", -1 },
                { "ls_fec_registro_min", "" }, // 1800-12-31
                { "li_limit", 50 }
            };
            Dictionary<string, object> ldictionary_mergedResult = _customHelper.MergeArrays(la_params_default, la_params); // merge the params


            int li_after_id = _customHelper.getMergedPropertyInt("li_after_id", ldictionary_mergedResult) ?? -1;
            string ls_fec_registro_min = _customHelper.getMergedPropertyString("ls_fec_registro_min", ldictionary_mergedResult) ?? "1900-12-31";
            int li_limit = _customHelper.getMergedPropertyInt("li_limit", ldictionary_mergedResult) ?? 50;
            //Console.WriteLine(JsonSerializer.Serialize(ldictionary_mergedResult));


            string ls_settingsjson_table = _configuration.GetValue<string>("CustomApp:Ls_bionegocios_zonas_bio_table");
            string ls_err_msg = "";
            int li_total_rows = 0;


            // QUERY USING SQLEXECUTOR SCALAR SQLMANUALLY
            string sqlQuery = "select COUNT(*) as total_query_rows FROM dba." + ls_settingsjson_table;
            List<object> minimalRequiredModelParams = new List<object> { };


            sqlQuery += " where id > :afteridArgument";
            minimalRequiredModelParams.Add(li_after_id);

            if (ls_fec_registro_min.Length > 0)
            {
                sqlQuery += " and fec_registro >= :fecregistroArgument";
                minimalRequiredModelParams.Add(ls_fec_registro_min);
            }

            //if (ls_zona.Length > 0)
            //{
            //    sqlQuery += " and zona= :zonaArgument";
            //    minimalRequiredModelParams.Add(ls_zona);
            //}

            // Is required | Convert objectList to objectArray
            object[] paramsArrayObject = minimalRequiredModelParams.ToArray();

            try
            {
                li_total_rows = await _dataContext.SqlExecutor.ScalarAsync<int>(sqlQuery, paramsArrayObject);
            }
            catch (Exception ex)
            {
                ls_err_msg = $"No fue posible realizar la consulta a: " + ls_settingsjson_table;
                objFnRes.Ksuccess = false;
                objFnRes.Kmessage = "||" + _currentServiceName + "|| Error line number (" + _customHelper.LineNumber() + "): " + ls_err_msg + ".  Mensaje: " + ex.Message;
                return JsonSerializer.Serialize(objFnRes, _currentJsonOptions);
            }

            // RESPONSE FOUND
            objFnRes.Ksuccess = true;
            objFnRes.Kdata.Add(li_total_rows);

            return JsonSerializer.Serialize(objFnRes, _currentJsonOptions); // no olvidar pasar parametro _currentJsonOptions
        }
        // ===== CMZONASBIO END


        public DateTime? HoraServidor()
        {

            String ldt_fecha_registro_bitacora = "22/05/2023";
            DateTime? dt_date = Convert.ToDateTime(ldt_fecha_registro_bitacora);

            // [INFO0103] Stored procedure declaration: sp_procedure.
            _dataContext.SqlExecutor.ExecuteProcedure("dbo.sp_fecha_sistema", out DbResultSet resultSet);

            //se ejecutó  el siguiente  script para  permitir modo de transacción (cualquiera)
            // si no se realiza esto, se  pueden generar fallos a la hora e ejecutar el procedimiento
            // esto debido a la activación de los begin transaction . la ejecución del procedimiento empezó a fallar
            // exec sp_procxmode 'sp_fecha_sistema', anymode

            //Ejecutamos con el nombre del Alias
            // [INFO0110] Fetch sp_procedure.
            resultSet.Next();
            // dt_date = resultSet.GetValue<DateTime>(0).ToString();
            //Recuperamos el resultado

            // Console.WriteLine(resultSet.SqlCode.ToString());
            //Console.WriteLine(resultSet.ErrorText.ToString());


            if (resultSet.SqlCode == -1)
            {
                dt_date = null;
            }

            while (resultSet.SqlCode == 0)
            {
                // [INFO0110] Fetch sp_procedure.
                // dt_date = resultSet.GetValue<DateTime>(0).ToString();
                dt_date = resultSet.GetValue<DateTime>(0);

                //Console.WriteLine($"SqlCode sp_fecha_sistema: {resultSet.SqlCode.ToString()} And error text sql: {resultSet.ErrorText.ToString()}");
                //Console.WriteLine($"SqlCode sp_fecha_sistema valor: {dt_date}");

                resultSet.Next();
            }
            // [INFO0111] Close sp_procedure.
            resultSet.Close();

            // [INFO0000] Return.
            return dt_date;
        }



        // End class
    }
}


