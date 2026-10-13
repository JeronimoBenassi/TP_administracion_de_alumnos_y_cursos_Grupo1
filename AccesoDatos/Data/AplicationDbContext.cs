using Microsoft.EntityFrameworkCore;
using AccesoDatos.Models;

namespace AccesoDatos.Data
{
    public class AplicationDbContext : DbContext
    {
        public DbSet<Alumno> Alumno { get; set; }
        public DbSet<Curso> Curso { get; set; }
        public DbSet<Inscripcion> Inscripcion { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=C:\\databases\\exampleDB.db");
        }
    }
}
