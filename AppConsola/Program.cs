using AccesoDatos.Models;
using AccesoDatos.Repositories;

// 1. Instanciamos el repositorio.
IGenericRepository<Alumno> alumnoRepository = new GenericRepository<Alumno>();
IGenericRepository<Curso> cursoRepository = new GenericRepository<Curso>();
IGenericRepository<Inscripcion> inscripcionRepository = new GenericRepository<Inscripcion>();

bool continuar = true;

while (continuar)
{
    Console.WriteLine("=================================================");
    Console.WriteLine("\tGestión de Usuarios");
    Console.WriteLine("=================================================");
    Console.WriteLine();
    Console.WriteLine("1. Agregar alumno (Alta)");
    Console.WriteLine("2. Modificar alumno (Modificación)");
    Console.WriteLine("3. Eliminar alumno (Baja)");
    Console.WriteLine("4. Ver todos los alumnos");
    Console.WriteLine("5. Agregar Curso (Alta)");
    Console.WriteLine("6. Modificar Curso (Modificación)");
    Console.WriteLine("7. Eliminar Curso (Baja)");
    Console.WriteLine("8. Ver todos los cursos");
    Console.WriteLine("9. Salir");
    Console.WriteLine();

    Console.Write("Seleccione una opción: ");
    string opcion = Console.ReadLine();
    Console.Clear();

    switch (opcion)
    {
        case "1":
            AltaAlumno();
            break;

        case "2":
            ModificarAlumno();
            break;

        case "3":
            BajaAlumno();
            break;

        case "4":
            VisualizarAlumnos();
            break;
        case "5":
            AltaCurso();
            break;
        case "6":
            ModificarCurso();
            break;
        case "7":
            BajaCurso();
            break;
        case "8":
            VisualizarCursos();
            break;

        case "9":
            Console.WriteLine("¡Cerrando el sistema de usuarios!");
            continuar = false;
            break;

        default:
            Console.WriteLine("Opción no válida. Intente nuevamente.");
            PresioneParaContinuar();
            break;
    }
}

void AltaAlumno()
{
    Console.Write("Ingrese el nombre del alumno: ");
    string Nombre = Console.ReadLine();

    Console.Write("Ingrese el apellido del alumno: ");
    string Apellido = Console.ReadLine();

    Console.Write("Ingrese el DNI del alumno: ");
    int DNI = int.Parse(Console.ReadLine());

    Console.Write("Ingrese el telefono del alumno: ");
    int Telefono = int.Parse(Console.ReadLine());

    Console.Write("Ingrese el email del alumno: ");
    string Email = Console.ReadLine();

    Console.Write("Ingrese la fecha de nacimiento del alumno: ");
    DateTime FechaNacimiento = DateTime.Parse(Console.ReadLine());

    var nuevoAlumno = new Alumno
    {
        Nombre = Nombre,
        Apellido = Apellido,
        DNI = DNI,
        Telefono = Telefono,
        Email = Email,
        FechaNacimiento = FechaNacimiento
    };

    alumnoRepository.Agregar(nuevoAlumno);
    Console.WriteLine("Alumno agregado exitosamente.");
    PresioneParaContinuar();
}

void ModificarAlumno()
{
    MostrarListaAlumnos(alumnoRepository);
    Console.Write("Ingrese el ID del alumno a modificar: ");

    if (int.TryParse(Console.ReadLine(), out int id))
    {
        var alumnoACambiar = alumnoRepository.ObtenerPorId(id);

        if (alumnoACambiar != null)
        {
            Console.Write($"Ingrese el nuevo nombre para '{alumnoACambiar.Nombre}': ");
            alumnoACambiar.Nombre = Console.ReadLine();

            Console.Write($"Ingrese el nuevo apellido para '{alumnoACambiar.Apellido}': ");
            alumnoACambiar.Apellido = Console.ReadLine();

            Console.Write($"Ingrese el nuevo DNI para '{alumnoACambiar.DNI}': ");
            alumnoACambiar.DNI = int.Parse(Console.ReadLine());

            Console.Write($"Ingrese el nuevo telefono para '{alumnoACambiar.Telefono}': ");
            alumnoACambiar.Telefono = int.Parse(Console.ReadLine());


            Console.Write($"Ingrese el nuevo email para '{alumnoACambiar.Email}': ");
            alumnoACambiar.Email = Console.ReadLine();

            Console.Write($"Ingrese la nueva fecha de nacimiento para '{alumnoACambiar.FechaNacimiento}': ");
            alumnoACambiar.FechaNacimiento = DateTime.Parse(Console.ReadLine());

            alumnoRepository.Modificar(alumnoACambiar);
            Console.WriteLine("Alumno actualizado correctamente.");
        }
        else
        {
            Console.WriteLine("No se encontró ningún alumno con ese ID.");
        }
    }
    else
    {
        Console.WriteLine("ID inválido.");
    }
    PresioneParaContinuar();
}

void BajaAlumno()
{
    MostrarListaAlumnos(alumnoRepository);
    Console.Write("Ingrese el ID del alumno a dar de baja: ");

    if (int.TryParse(Console.ReadLine(), out int id))
    {
        var alumno = alumnoRepository.ObtenerPorId(id);

        if (alumno == null)
        {
            Console.WriteLine("El alumno no existe.");
        }
        else if (!alumno.Activo)
        {
            Console.WriteLine("El alumno ya está dado de baja.");
        }
        else
        {
            alumno.Activo = false;
            alumnoRepository.Modificar(alumno);

            Console.WriteLine("Alumno dado de baja.");
        }
    }
    else
    {
        Console.WriteLine("ID inválido.");
    }
    PresioneParaContinuar();
}

void VisualizarAlumnos()
{
    MostrarListaAlumnos(alumnoRepository);
    PresioneParaContinuar();
}

void MostrarListaAlumnos(IGenericRepository<Alumno> repository)
{
    Console.WriteLine("--- LISTADO ACTUAL EN BASE DE DATOS ---");
    var alumnos = repository.ObtenerTodos();

    if (!alumnos.Any())
    {
        Console.WriteLine("[La tabla está vacía]");
    }
    else
    {
        foreach (var a in alumnos)
        {
            string estado = a.Activo ? "Activo" : "Baja";
            Console.WriteLine($"ID: {a.Id} | Nombre: {a.Nombre} {a.Apellido} | DNI: {a.DNI} | Teléfono: {a.Telefono} | Email: {a.Email} | Fecha de Nacimiento: {a.FechaNacimiento:dd/MM/yyyy} | Estado: {estado}");
        }
    }
    Console.WriteLine("---------------------------------------");
    Console.WriteLine();
}

void AltaCurso()
{
    Console.Write("Ingrese el nombre del curso: ");
    string Nombre = Console.ReadLine();

    Console.Write("Ingrese la descripción del curso: ");
    string Descripcion = Console.ReadLine();

    Console.Write("Ingrese la duración del curso (en horas): ");
    int Duracion = int.Parse(Console.ReadLine());

    Console.Write("Ingrese el cupo máximo del curso: ");
    int CupoMaximo = int.Parse(Console.ReadLine());

    var nuevoCurso = new Curso
    {
        Nombre = Nombre,
        Descripcion = Descripcion,
        Duracion = Duracion,
        CupoMaximo = CupoMaximo
    };

    cursoRepository.Agregar(nuevoCurso);
    Console.WriteLine("Curso agregado exitosamente.");
    PresioneParaContinuar();
}

void ModificarCurso()
{
    MostrarListaCursos(cursoRepository);
    Console.Write("Ingrese el ID del curso a modificar: ");

    if (int.TryParse(Console.ReadLine(), out int id))
    {
        var cursoACambiar = cursoRepository.ObtenerPorId(id);

        if (cursoACambiar != null)
        {
            Console.Write($"Ingrese el nuevo nombre para '{cursoACambiar.Nombre}': ");
            cursoACambiar.Nombre = Console.ReadLine();

            Console.Write($"Ingrese la nueva descripción para '{cursoACambiar.Descripcion}': ");
            cursoACambiar.Descripcion = Console.ReadLine();

            Console.Write($"Ingrese la nueva duración para '{cursoACambiar.Duracion}': ");
            cursoACambiar.Duracion = int.Parse(Console.ReadLine());

            Console.Write($"Ingrese el nuevo cupo máximo para '{cursoACambiar.CupoMaximo}': ");
            cursoACambiar.CupoMaximo = int.Parse(Console.ReadLine());

            cursoRepository.Modificar(cursoACambiar);
            Console.WriteLine("Curso actualizado correctamente.");
        }
        else
        {
            Console.WriteLine("No se encontró ningún curso con ese ID.");
        }
    }
    else
    {
        Console.WriteLine("ID inválido.");
    }
    PresioneParaContinuar();
}

void BajaCurso()
{
    MostrarListaCursos(cursoRepository);
    Console.Write("Ingrese el ID del curso a dar de baja: ");

    if (int.TryParse(Console.ReadLine(), out int id))
    {
        var curso = cursoRepository.ObtenerPorId(id);

        if (curso == null)
        {
            Console.WriteLine("El curso no existe.");
        }
        else if (!curso.Activo)
        {
            Console.WriteLine("El curso ya está dado de baja.");
        }
        else
        {
            curso.Activo = false;
            cursoRepository.Modificar(curso);

            Console.WriteLine("Curso dado de baja.");
        }
    }
    else
    {
        Console.WriteLine("ID inválido.");
    }
    PresioneParaContinuar();
}

void VisualizarCursos()
{
    MostrarListaCursos(cursoRepository);
    PresioneParaContinuar();
}

void MostrarListaCursos(IGenericRepository<Curso> repository)
{
    Console.WriteLine("--- LISTADO DE CURSOS DISPONIBLES ---");
    var cursos = repository.ObtenerTodos().Where(c => c.Activo).ToList();

    if (!cursos.Any())
    {
        Console.WriteLine("[No hay cursos disponibles]");
    }
    else
    {
        foreach (var c in cursos)
        {
            Console.WriteLine($"ID: {c.Id} | Nombre: {c.Nombre} | Descripción: {c.Descripcion} | Duración: {c.Duracion} hs | Cupo máximo: {c.CupoMaximo}");
        }
    }
    Console.WriteLine("---------------------------------------");
    Console.WriteLine();
}

void PresioneParaContinuar()
{
    Console.WriteLine("\nPresione cualquier tecla para continuar...");
    Console.ReadKey();
    Console.Clear();
}
