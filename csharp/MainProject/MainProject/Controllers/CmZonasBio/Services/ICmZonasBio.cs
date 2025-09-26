namespace MainProject.Controllers.CmZonasBio.Services
{
    public interface ICmZonasBio
    {
        Task<string> CmZonasBioMainGetAllAsync(string ls_ctrl_json);
        Task<string> CmZonasBioMainGetFilteredAsync(string ls_ctrl_json);

      
    }
}
