using AccesoDatos.Data;
using AccesoDatos.Models;
using Microsoft.EntityFrameworkCore;

namespace AccesoDatos.Repositories;

public class InscripcionRepository : GenericRepository<Inscripcion>
{
    public List<Inscripcion> ObtenerAlumnosActivosPorCurso(int cursoId)
    {
        return _context.Inscripcion
            .Include(i => i.Alumno)  
            .Include(i => i.Curso)
            .Where(i => i.CursoId == cursoId && i.Estado == "Activo")
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

    public void InscribirAlumno(int alumnoId, int cursoId)
    {
        // 1. Validar si ya existe una inscripción activa para evitar duplicados
        var yaInscripto = _context.Inscripcion
            .Any(i => i.AlumnoId == alumnoId && i.CursoId == cursoId && i.Estado == "Activo");

        if (yaInscripto)
        {
            throw new Exception("El alumno ya se encuentra inscripto en este curso.");
        }

        // 2. Opcional: Validar cupos disponibles antes de inscribir
        var curso = _context.Curso.Find(cursoId);
        var inscriptosActuales = _context.Inscripcion
            .Count(i => i.CursoId == cursoId && i.Estado == "Activo");

        if (inscriptosActuales >= curso.CupoMaximo)
        {
            throw new Exception("El curso ya no cuenta con cupos disponibles.");
        }

        // 3. Crear y guardar la inscripción
        var nuevaInscripcion = new Inscripcion
        {
            AlumnoId = alumnoId,
            CursoId = cursoId,
            FechaInscripcion = DateTime.Now,
            Estado = "Activo"
        };

        _context.Inscripcion.Add(nuevaInscripcion);
        _context.SaveChanges();
    }
}