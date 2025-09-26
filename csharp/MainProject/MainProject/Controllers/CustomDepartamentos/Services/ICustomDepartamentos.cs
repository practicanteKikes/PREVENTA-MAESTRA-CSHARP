namespace MainProject.Controllers.CustomDepartamentos.Services
{
    public interface ICustomDepartamentos
    {
        Task<string> MainGetDepartamentosAllAsync(string ls_ctrl_json);
        Task<string> MainGetDepartamentosFilteredAsync(string ls_ctrl_json);
    }
}
