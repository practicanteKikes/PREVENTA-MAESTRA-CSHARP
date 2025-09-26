namespace MainProject.Services.CustomSybase.Models.LoadInformationSchemaByTableName.Json
{
    // Le puse el mismo nombre de la tabla que mysql guarda internamente
    // Pero realmente es sybase ase y la tabla es sysobjects
    // ej para el desarrollador: SELECT type, name FROM sysobjects WHERE type = 'U' AND name = 'ca_temp_consignaciones_ws'
    public class InformationSchemaJsonModel
    {
        public string? Type { get; set; } = null;
        public string? Name { get; set; } = null;
    }
}
