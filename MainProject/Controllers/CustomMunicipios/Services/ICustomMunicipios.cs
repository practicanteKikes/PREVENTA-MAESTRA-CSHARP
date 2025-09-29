namespace MainProject.Controllers.CustomMunicipios.Services
{
    public interface ICustomMunicipios
    {
        Task<string> MainGetMunicipiosAllAsync(string ls_ctrl_json);
        Task<string> MainGetMunicipiosFilteredAsync(string ls_ctrl_json);
    }
}
