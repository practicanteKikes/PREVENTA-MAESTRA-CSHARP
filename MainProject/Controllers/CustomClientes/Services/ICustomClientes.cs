namespace MainProject.Controllers.CustomClientes.Services
{
    public interface ICustomClientes
    {
        Task<string> MainGetClientesAllAsync(string ls_json);
        Task<string> MainGetClientesFilteredAsync(string ls_json);        
    }
}
