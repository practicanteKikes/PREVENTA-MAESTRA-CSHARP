namespace MainProject.Controllers.CmUsuariosMovil.Services
{
    public interface ICmUsuariosMovil
    {
        Task<string> MainGetCmUsuariosMovilAllAsync(string ls_ctrl_json);
        Task<string> MainGetCmUsuariosMovilFilteredAsync(string ls_ctrl_json);
    }
}
