namespace AccesoDatos.Models
{
    public class Alumno
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int DNI { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public int Telefono { get; set; }
        public bool Activo { get; set; } = true;
        public string Email { get; set; }
        public ICollection<Inscripcion> Inscripciones { get; set; }

    }
}
