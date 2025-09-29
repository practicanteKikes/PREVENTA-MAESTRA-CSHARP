namespace MainProject.Controllers.CustomTiposNegocio.Services
{
    public interface ICustomTiposNegocio
    {
        Task<string> MainGetTiposNegocioFilteredAsync(string ls_ctrl_json);
        Task<string> MainGetTiposNegocioAllAsync(string ls_ctrl_json);
    }
}
