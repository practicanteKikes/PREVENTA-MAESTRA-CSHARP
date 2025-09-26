namespace MainProject.Controllers.CmLineasProductos.Services
{
    public interface ICmLineasProductos
    {
        Task<string> MainGetCmLineasProductosAllAsync(string ls_ctrl_json);
        Task<string> MainGetCmLineasProductosFilteredAsync(string ls_ctrl_json);
    }
}
