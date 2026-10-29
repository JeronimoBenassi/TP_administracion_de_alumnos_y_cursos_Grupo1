namespace AccesoDatos.Models
{
    public class Alumno
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public long DNI { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public long Telefono { get; set; }
        public bool Activo { get; set; } = true;
        public string Email { get; set; }
        public ICollection<Inscripcion> Inscripciones { get; set; }

    }
}
