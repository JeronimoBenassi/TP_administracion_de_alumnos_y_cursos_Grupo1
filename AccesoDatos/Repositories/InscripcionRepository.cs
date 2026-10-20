using AccesoDatos.Data;
using AccesoDatos.Models;
using Microsoft.EntityFrameworkCore;

namespace AccesoDatos.Repositories;

public class InscripcionRepository : GenericRepository<Inscripcion>
{
    public List<Inscripcion> ObtenerAlumnosActivosPorCurso(int cursoId)
    {
        return _context.Set<Inscripcion>()
            .AsNoTracking()
            .Include(i => i.Alumno)
            .Where(i =>
                i.CursoId == cursoId &&
                i.Estado == "Activa")
            .ToList();
    }
    public List<Inscripcion> ObtenerInscripcionesPorAlumno(int alumnoId)
    {
        return _context.Set<Inscripcion>()
            .AsNoTracking()
            .Include(i => i.Curso)
            .Where(i => i.AlumnoId == alumnoId)
            .ToList();
    }
    public Dictionary<int, int> ObtenerCantidadAlumnosActivosPorCurso()
    {
        return _context.Set<Inscripcion>()
            .AsNoTracking()
            .Where(i => i.Estado == "Activa")
            .GroupBy(i => i.CursoId)
            .Select(grupo => new
            {
                CursoId = grupo.Key,
                Cantidad = grupo.Count()
            })
            .ToDictionary(
                resultado => resultado.CursoId,
                resultado => resultado.Cantidad);
    }
}