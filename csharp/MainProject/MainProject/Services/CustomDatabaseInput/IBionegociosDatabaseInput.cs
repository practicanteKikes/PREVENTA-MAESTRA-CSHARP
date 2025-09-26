using MainProject.Services.CustomSybase.Models.LoadDbInfo.FnRes;
using MainProject.Services.CustomSybase.Models.LoadInformationSchemaByTableName.FnRes;

namespace MainProject.Services.CustomDatabaseInput
{
    public interface IBionegociosDatabaseInput
    {
        int InsertDepositNotification<T>(T newRecord);
        //ObjFnResLoadUserByEmailPassword LoadUserByEmailPassword(string email, string password);

        ObjFnResLoadDbInfoInput LoadDbInfo();

        int InsertDepositNotifyLog<T>(T newRecord);

        ObjFnResLoadInformationSchemaByTableName LoadInformationSchemaByTableName(string ls_table_name);

        ODBCBionegociosDataContext GetConnection();


        // Remember Task for asyn await
        Task<string> QueryClientesAllAzync();
        Task<string> QueryClientesFilteredAzync( Dictionary<string, object> la_params );
        Task<string> QueryClientesFilteredTotalRowsAzync(Dictionary<string, object> la_params);        
        Task<string> QueryUsersFilteredAzync(Dictionary<string, object> la_params);


        Task<string> QueryTiposNegocioFilteredAzync(Dictionary<string, object> la_params);
        Task<string> QueryTiposNegocioFilteredTotalRowsAzync(Dictionary<string, object> la_params);
        Task<string> QueryTiposNegocioAllAzync();


        Task<string> QueryDepartamentosAllAzync();
        Task<string> QueryDepartamentosFilteredAzync(Dictionary<string, object> la_params);
        Task<string> QueryDepartamentosFilteredTotalRowsAzync(Dictionary<string, object> la_params);


        Task<string> QueryMunicipiosAllAzync();
        Task<string> QueryMunicipiosFilteredAzync(Dictionary<string, object> la_params);
        Task<string> QueryMunicipiosFilteredTotalRowsAzync(Dictionary<string, object> la_params);
        
        Task<string> QueryCmUsuariosMovilAllAzync();
        Task<string> QueryCmUsuariosMovilFilteredAzync(Dictionary<string, object> la_params);
        Task<string> QueryCmUsuariosMovilFilteredTotalRowsAzync(Dictionary<string, object> la_params);

        Task<string> QueryCmLineasProductosAllAzync();
        Task<string> QueryCmLineasProductosFilteredAzync(Dictionary<string, object> la_params);
        Task<string> QueryCmLineasProductosFilteredTotalRowsAzync(Dictionary<string, object> la_params);

        
        // last must use sintax
        Task<string> CmProductosQueryAllAzync();
        Task<string> CmProductosQueryFilteredAzync(Dictionary<string, object> la_params);
        Task<string> CmProductosQueryFilteredTotalRowsAzync(Dictionary<string, object> la_params);

        Task<string> CmMotivosNcQueryAllAzync();
        Task<string> CmMotivosNcQueryFilteredAzync(Dictionary<string, object> la_params);
        Task<string> CmMotivosNcQueryFilteredTotalRowsAzync(Dictionary<string, object> la_params);


        Task<string> CmZonasBioQueryAllAzync();
        Task<string> CmZonasBioQueryFilteredAzync(Dictionary<string, object> la_params);
        Task<string> CmZonasBioQueryFilteredTotalRowsAzync(Dictionary<string, object> la_params);


        public DateTime? HoraServidor();

    }
}
