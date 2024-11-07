using Microsoft.EntityFrameworkCore;
using minimal_api.dominio.entidades;

namespace minimal_api.infraestutura.DB
{
    public class DbContexto : DbContext
    {
        private readonly IConfiguration _configuracaoAppSettings;
        public DbContexto(IConfiguration configuracaoAppSettings)
        {
            _configuracaoAppSettings = configuracaoAppSettings;
        }
        public DbSet<Administrador> Administradores {get; set;} = default!;
        public DbSet<Veiculo> Veiculos {get; set;} = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrador>().HasData(
                new Administrador {
                    Id = 1,
                    Email = "Administrador@teste.com",
                    Senha = "123456",   
                    Perfil = "Adm"                
                }
            );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var stringConexao = _configuracaoAppSettings.GetConnectionString("Default")?.ToString();
            if(!string.IsNullOrEmpty(stringConexao))
                optionsBuilder.UseSqlServer(stringConexao);
        }
    }
}