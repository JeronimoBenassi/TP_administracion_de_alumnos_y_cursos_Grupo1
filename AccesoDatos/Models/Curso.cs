namespace AccesoDatos.Models
{
    public class Curso
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Duracion { get; set; }
        public int CupoMaximo { get; set; }
        public bool Activo { get; set; } = true;
        public ICollection<Inscripcion> Inscripciones { get; set; }


    }
}
