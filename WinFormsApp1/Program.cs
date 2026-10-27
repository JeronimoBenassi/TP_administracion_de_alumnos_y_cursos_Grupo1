using AccesoDatos.Models;
using AccesoDatos.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace WinFormsApp1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            var services = new ServiceCollection();

            // 1. Registramos todos los repositorios que usa el FormAlumnos
            services.AddTransient<IGenericRepository<Alumno>, GenericRepository<Alumno>>();
            services.AddTransient<IGenericRepository<Curso>, GenericRepository<Curso>>();
            services.AddTransient<InscripcionRepository>();

            // 2. Registramos el formulario
            services.AddTransient<FormAlumnos>();

            var provider = services.BuildServiceProvider();

            // 3. Ejecutamos la app levantando el form con inyección de dependencias
            Application.Run(provider.GetRequiredService<FormAlumnos>());
        }
    }
}