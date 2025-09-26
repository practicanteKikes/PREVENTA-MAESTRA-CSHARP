using MainProject.Services.CustomSqlserver.Models.EFModel;
using Microsoft.EntityFrameworkCore;

namespace MainProject
{
    public partial class MSSQLINBANCOSDbContext : DbContext
    {
        // INTERNAL PROPERTIES
        private IConfiguration _configuration; // Leeremos las tablas o vistas usadas
        
        public virtual DbSet<VwOccidenteTercerosAutAth> VwOccidenteTercerosAthAuts { get; set; }        
        public virtual DbSet<DatabaseInfoEF> DatabaseInfoEFs { get; set; }
        public virtual DbSet<BancoConvenioEF> BancosConveniosEfs { get; set; }


        // CONSTRUCTOR
        public MSSQLINBANCOSDbContext(
            IConfiguration argconfiguration,
            DbContextOptions<MSSQLINBANCOSDbContext> options

        ): base(options)
        {
            _configuration = argconfiguration;            

            VwOccidenteTercerosAthAuts = Set<VwOccidenteTercerosAutAth>();
            DatabaseInfoEFs = Set<DatabaseInfoEF>();
            BancosConveniosEfs = Set<BancoConvenioEF>();            
        }                

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string ls_settingsjson_athterceros_view = _configuration.GetValue<string>("CustomApp:Ls_occidente_ath_terceros_view"); // "vw_occidente_terceros_aut_ath"
            string ls_settingsjson_athconvenios_table = _configuration.GetValue<string>("CustomApp:Ls_occidente_ath_convenios_table"); // "BANCOS_CONVENIOS"

            //// SET TABLE NAME            
            modelBuilder.Entity<VwOccidenteTercerosAutAth>().ToView(ls_settingsjson_athterceros_view); // Tambien valido que el nit exista
            modelBuilder.Entity<DatabaseInfoEF>().HasNoKey(); // Raw query
            modelBuilder.Entity<BancoConvenioEF>().ToTable(ls_settingsjson_athconvenios_table); // Tambien valido que el convenio exista
        }




        // end class
    }
}
