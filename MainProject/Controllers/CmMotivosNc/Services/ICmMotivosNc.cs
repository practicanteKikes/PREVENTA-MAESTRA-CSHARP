namespace MainProject.Controllers.CmMotivosNc.Services
{
    public interface ICmMotivosNc
    {
        Task<string> CmMotivosNcMainGetAllAsync(string ls_ctrl_json);
        Task<string> CmMotivosNcMainGetFilteredAsync(string ls_ctrl_json);
    }
}
