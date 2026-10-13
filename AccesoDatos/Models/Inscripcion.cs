namespace AccesoDatos.Models
{
    public class Inscripcion
    {
        public int Id { get; set; }
        public int AlumnoId { get; set; }
        public Alumno Alumno { get; set; }
        public int CursoId { get; set; }
        public Curso Curso { get; set; }
        public string Estado { get; set; }
        public DateTime FechaInscripcion { get; set; }

    }
}
