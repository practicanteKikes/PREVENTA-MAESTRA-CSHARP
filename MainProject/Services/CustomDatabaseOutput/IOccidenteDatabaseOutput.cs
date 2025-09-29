using MainProject.Services.CustomDatabaseOutput.Models.FnRes;

namespace MainProject.Services.CustomDatabaseOutput
{
    public interface IOccidenteDatabaseOutput
    {
        Task<ObjFnResFindFirstOrDefaultByNit> FindFirstOrDefaultByNit(string ls_nit);

        Task<ObjFnResLoadDbInfoOutput> LoadDbInfoAsync();

        Task<ObjFnResBancosConveniosByConvenio> BancosConveniosByConvenioAsync(string ls_convenio);
    }
}
