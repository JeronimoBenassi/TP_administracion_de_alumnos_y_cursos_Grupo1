
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AccesoDatos.Models;
using AccesoDatos.Repositories;

namespace WinFormsApp1
{
    public class FormAlumnos : Form
    {
        // =====================================
        // REPOSITORIOS (Inyección de Dependencias)
        // =====================================
        private readonly IGenericRepository<Alumno> alumnoRepository;
        private readonly IGenericRepository<Curso> cursoRepository;
        private readonly InscripcionRepository inscripcionRepository;

        // =====================================
        // PANELES DE NAVEGACIÓN
        // =====================================
        private Panel panelAlumnos = new();
        private Panel panelCursos = new();
        private Panel panelInscripciones = new();
        private Panel panelReportes = new();

        // =====================================
        // CONTROLES - ALUMNOS
        // =====================================
        private readonly TextBox txtNombre = new();
        private readonly TextBox txtApellido = new();
        private readonly TextBox txtDNI = new();
        private readonly TextBox txtTelefono = new();
        private readonly TextBox txtEmail = new();
        private readonly DateTimePicker dtpNacimiento = new();
        private readonly DataGridView dgvAlumnos = new();
        private int? alumnoIdSeleccionado;

        // =====================================
        // CONTROLES - CURSOS
        // =====================================
        private readonly TextBox txtCursoNombre = new();
        private readonly TextBox txtDescripcion = new();
        private readonly TextBox txtDuracion = new();
        private readonly TextBox txtCupo = new();
        private readonly CheckBox chkActivo = new();
        private readonly DataGridView dgvCursos = new();
        private int? cursoIdSeleccionado;

        // =====================================
        // CONTROLES - INSCRIPCIONES
        // =====================================
        private readonly ComboBox cmbInscripcionAlumno = new();
        private readonly ComboBox cmbInscripcionCurso = new();

        // =====================================
        // CONTROLES - REPORTES
        // =====================================
        private readonly ComboBox cmbCursoReporte = new();
        private readonly ComboBox cmbAlumnoReporte = new();
        private readonly DataGridView dgvReporte = new();

        // =====================================
        // CONSTRUCTOR
        // =====================================
        public FormAlumnos(
            IGenericRepository<Alumno> alumnoRepo,
            IGenericRepository<Curso> cursoRepo,
            InscripcionRepository inscripcionRepo)
        {
            alumnoRepository = alumnoRepo;
            cursoRepository = cursoRepo;
            inscripcionRepository = inscripcionRepo;

            CrearInterfaz();
            CargarAlumnos();
            CargarCursos();
            CargarCombosInscripciones();
            CargarCombosReportes();
        }

        // =====================================
        // INTERFAZ Y NAVEGACIÓN
        // =====================================
        private void CrearInterfaz()
        {
            Text = "Sistema de administración";
            Width = 1050;
            Height = 760;
            MinimumSize = new Size(900, 650);
            StartPosition = FormStartPosition.CenterScreen;

            // Instanciar Paneles
            panelAlumnos = CrearPanel("ADMINISTRACIÓN DE ALUMNOS");
            panelCursos = CrearPanel("ADMINISTRACIÓN DE CURSOS");
            panelInscripciones = CrearPanel("GESTIÓN DE INSCRIPCIONES");
            panelReportes = CrearPanel("REPORTES");

            // Armar contenido de cada panel
            CrearInterfazAlumnos();
            CrearInterfazCursos();
            CrearInterfazInscripciones(panelInscripciones);
            CrearInterfazReportes();

            // Botones de Menú Superior
            CrearBoton(this, "Alumnos", 20, 15, 120, 35,
                () => MostrarPanel(panelAlumnos));

            CrearBoton(this, "Cursos", 150, 15, 120, 35,
                () => MostrarPanel(panelCursos));

            CrearBoton(this, "Inscripciones", 280, 15, 130, 35,
                () =>
                {
                    CargarCombosInscripciones();
                    MostrarPanel(panelInscripciones);
                });

            CrearBoton(this, "Reportes", 420, 15, 120, 35,
                () =>
                {
                    CargarCombosReportes();
                    MostrarPanel(panelReportes);
                });

            MostrarPanel(panelAlumnos);
        }

        private Panel CrearPanel(string titulo)
        {
            Panel panel = new Panel
            {
                Left = 15,
                Top = 65,
                Width = 990,
                Height = 640,
                Visible = false,
                AutoScroll = true
            };

            panel.Controls.Add(new Label
            {
                Text = titulo,
                Left = 10,
                Top = 10,
                Width = 750,
                Height = 35,
                Font = new Font("Segoe UI", 17, FontStyle.Bold)
            });

            Controls.Add(panel);
            return panel;
        }

        private void MostrarPanel(Panel panel)
        {
            panelAlumnos.Visible = false;
            panelCursos.Visible = false;
            panelInscripciones.Visible = false;
            panelReportes.Visible = false;

            panel.Visible = true;
            panel.BringToFront();

            foreach (Control control in Controls)
            {
                if (control is Button)
                    control.BringToFront();
            }
        }

        private void AgregarCampo(Panel panel, string etiqueta, TextBox caja, int x, int y)
        {
            panel.Controls.Add(new Label
            {
                Text = etiqueta,
                Left = x,
                Top = y,
                Width = 190
            });

            caja.SetBounds(x, y + 24, 200, 27);
            panel.Controls.Add(caja);
        }

        private Button CrearBoton(Control contenedor, string texto, int x, int y, int ancho, int alto, Action accion)
        {
            Button boton = new Button
            {
                Text = texto,
                Left = x,
                Top = y,
                Width = ancho,
                Height = alto
            };

            boton.Click += (s, e) => accion();
            contenedor.Controls.Add(boton);
            return boton;
        }

        private void ConfigurarGrilla(DataGridView grilla)
        {
            grilla.ReadOnly = true;
            grilla.AllowUserToAddRows = false;
            grilla.AllowUserToDeleteRows = false;
            grilla.MultiSelect = false;
            grilla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grilla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // =====================================
        // ALUMNOS
        // =====================================
        private void CrearInterfazAlumnos()
        {
            AgregarCampo(panelAlumnos, "Nombre", txtNombre, 10, 60);
            AgregarCampo(panelAlumnos, "Apellido", txtApellido, 240, 60);
            AgregarCampo(panelAlumnos, "DNI", txtDNI, 470, 60);
            AgregarCampo(panelAlumnos, "Teléfono", txtTelefono, 10, 130);
            AgregarCampo(panelAlumnos, "Email", txtEmail, 240, 130);

            panelAlumnos.Controls.Add(new Label
            {
                Text = "Fecha de nacimiento",
                Left = 470,
                Top = 130,
                Width = 200
            });

            dtpNacimiento.SetBounds(470, 154, 200, 27);
            panelAlumnos.Controls.Add(dtpNacimiento);

            CrearBoton(panelAlumnos, "Guardar", 10, 200, 100, 35, GuardarAlumno);
            CrearBoton(panelAlumnos, "Modificar", 120, 200, 100, 35, ModificarAlumno);
            CrearBoton(panelAlumnos, "Eliminar", 230, 200, 100, 35, EliminarAlumno);
            CrearBoton(panelAlumnos, "Limpiar", 340, 200, 100, 35, LimpiarAlumno);

            ConfigurarGrilla(dgvAlumnos);
            dgvAlumnos.SetBounds(10, 250, 940, 330);
            dgvAlumnos.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    SeleccionarAlumno();
            };

            panelAlumnos.Controls.Add(dgvAlumnos);
        }

        private void CargarAlumnos()
        {
            try
            {
                dgvAlumnos.DataSource = null;
                dgvAlumnos.DataSource = alumnoRepository.ObtenerTodos()
                    .Select(a => new
                    {
                        a.Id,
                        a.Nombre,
                        a.Apellido,
                        a.DNI,
                        a.FechaNacimiento,
                        a.Telefono,
                        a.Email,
                        a.Activo
                    }).ToList();

                if (dgvAlumnos.Columns["Id"] != null)
                    dgvAlumnos.Columns["Id"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar alumnos:\n" + ex.Message);
            }
        }

        private bool ValidarAlumno(out int dni, out int telefono)
        {
            dni = 0;
            telefono = 0;

            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Completá nombre, apellido y email.");
                return false;
            }

            if (!int.TryParse(txtDNI.Text, out dni) ||
                !int.TryParse(txtTelefono.Text, out telefono))
            {
                MessageBox.Show("DNI y teléfono deben ser numéricos.");
                return false;
            }

            return true;
        }

        private void GuardarAlumno()
        {
            if (!ValidarAlumno(out int dni, out int telefono))
                return;

            try
            {
                Alumno alumno = new Alumno
                {
                    Nombre = txtNombre.Text.Trim(),
                    Apellido = txtApellido.Text.Trim(),
                    DNI = dni,
                    Telefono = telefono,
                    Email = txtEmail.Text.Trim(),
                    FechaNacimiento = dtpNacimiento.Value.Date,
                    Activo = true
                };

                alumnoRepository.Agregar(alumno);
                CargarAlumnos();
                CargarCombosInscripciones();
                CargarCombosReportes();
                LimpiarAlumno();

                MessageBox.Show("Alumno guardado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar:\n" + ex.Message);
            }
        }

        private void ModificarAlumno()
        {
            if (alumnoIdSeleccionado == null)
            {
                MessageBox.Show("Seleccioná un alumno.");
                return;
            }

            if (!ValidarAlumno(out int dni, out int telefono))
                return;

            try
            {
                Alumno alumno = alumnoRepository.ObtenerPorId(alumnoIdSeleccionado.Value);

                if (alumno == null)
                {
                    MessageBox.Show("Alumno no encontrado.");
                    return;
                }

                alumno.Nombre = txtNombre.Text.Trim();
                alumno.Apellido = txtApellido.Text.Trim();
                alumno.DNI = dni;
                alumno.Telefono = telefono;
                alumno.Email = txtEmail.Text.Trim();
                alumno.FechaNacimiento = dtpNacimiento.Value.Date;

                alumnoRepository.Modificar(alumno);
                CargarAlumnos();
                CargarCombosInscripciones();
                CargarCombosReportes();
                LimpiarAlumno();

                MessageBox.Show("Alumno modificado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar:\n" + ex.Message);
            }
        }

        private void EliminarAlumno()
        {
            if (alumnoIdSeleccionado == null)
            {
                MessageBox.Show("Seleccioná un alumno.");
                return;
            }

            if (MessageBox.Show("¿Eliminar el alumno seleccionado?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                alumnoRepository.Eliminar(alumnoIdSeleccionado.Value);
                CargarAlumnos();
                CargarCombosInscripciones();
                CargarCombosReportes();
                LimpiarAlumno();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar:\n" + ex.Message);
            }
        }

        private void SeleccionarAlumno()
        {
            if (dgvAlumnos.CurrentRow == null)
                return;

            var fila = dgvAlumnos.CurrentRow;
            alumnoIdSeleccionado = Convert.ToInt32(fila.Cells["Id"].Value);

            txtNombre.Text = fila.Cells["Nombre"].Value?.ToString() ?? "";
            txtApellido.Text = fila.Cells["Apellido"].Value?.ToString() ?? "";
            txtDNI.Text = fila.Cells["DNI"].Value?.ToString() ?? "";
            txtTelefono.Text = fila.Cells["Telefono"].Value?.ToString() ?? "";
            txtEmail.Text = fila.Cells["Email"].Value?.ToString() ?? "";

            if (DateTime.TryParse(fila.Cells["FechaNacimiento"].Value?.ToString(), out DateTime fecha))
                dtpNacimiento.Value = fecha;
        }

        private void LimpiarAlumno()
        {
            txtNombre.Clear();
            txtApellido.Clear();
            txtDNI.Clear();
            txtTelefono.Clear();
            txtEmail.Clear();
            dtpNacimiento.Value = DateTime.Today;
            alumnoIdSeleccionado = null;
            dgvAlumnos.ClearSelection();
        }

        // =====================================
        // CURSOS
        // =====================================
        private void CrearInterfazCursos()
        {
            AgregarCampo(panelCursos, "Nombre", txtCursoNombre, 10, 60);
            AgregarCampo(panelCursos, "Descripción", txtDescripcion, 240, 60);
            AgregarCampo(panelCursos, "Duración (horas)", txtDuracion, 470, 60);
            AgregarCampo(panelCursos, "Cupo máximo", txtCupo, 10, 130);

            chkActivo.Text = "Curso activo";
            chkActivo.Checked = true;
            chkActivo.SetBounds(240, 154, 180, 27);
            panelCursos.Controls.Add(chkActivo);

            CrearBoton(panelCursos, "Guardar", 10, 200, 100, 35, GuardarCurso);
            CrearBoton(panelCursos, "Modificar", 120, 200, 100, 35, ModificarCurso);
            CrearBoton(panelCursos, "Eliminar", 230, 200, 100, 35, EliminarCurso);
            CrearBoton(panelCursos, "Limpiar", 340, 200, 100, 35, LimpiarCurso);

            ConfigurarGrilla(dgvCursos);
            dgvCursos.SetBounds(10, 250, 940, 330);
            dgvCursos.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    SeleccionarCurso();
            };

            panelCursos.Controls.Add(dgvCursos);
        }

        private void CargarCursos()
        {
            try
            {
                var cursos = cursoRepository.ObtenerTodos().ToList();

                dgvCursos.DataSource = null;
                dgvCursos.DataSource = cursos.Select(c => new
                {
                    c.Id,
                    c.Nombre,
                    c.Descripcion,
                    c.Duracion,
                    c.CupoMaximo,
                    c.Activo
                }).ToList();

                if (dgvCursos.Columns["Id"] != null)
                    dgvCursos.Columns["Id"].Visible = false;

                CargarCombosInscripciones();
                CargarCombosReportes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar cursos:\n" + ex.Message);
            }
        }

        private bool ValidarCurso(out int duracion, out int cupo)
        {
            duracion = 0;
            cupo = 0;

            if (string.IsNullOrWhiteSpace(txtCursoNombre.Text) ||
                string.IsNullOrWhiteSpace(txtDescripcion.Text))
            {
                MessageBox.Show("Completá el nombre y la descripción.");
                return false;
            }

            if (!int.TryParse(txtDuracion.Text, out duracion) || duracion <= 0)
            {
                MessageBox.Show("La duración debe ser mayor a cero.");
                return false;
            }

            if (!int.TryParse(txtCupo.Text, out cupo) || cupo <= 0)
            {
                MessageBox.Show("El cupo debe ser mayor a cero.");
                return false;
            }

            return true;
        }

        private void GuardarCurso()
        {
            if (!ValidarCurso(out int duracion, out int cupo))
                return;

            try
            {
                Curso curso = new Curso
                {
                    Nombre = txtCursoNombre.Text.Trim(),
                    Descripcion = txtDescripcion.Text.Trim(),
                    Duracion = duracion,
                    CupoMaximo = cupo,
                    Activo = chkActivo.Checked
                };

                cursoRepository.Agregar(curso);
                CargarCursos();
                LimpiarCurso();

                MessageBox.Show("Curso guardado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar curso:\n" + ex.Message);
            }
        }

        private void ModificarCurso()
        {
            if (cursoIdSeleccionado == null)
            {
                MessageBox.Show("Seleccioná un curso.");
                return;
            }

            if (!ValidarCurso(out int duracion, out int cupo))
                return;

            try
            {
                Curso curso = cursoRepository.ObtenerPorId(cursoIdSeleccionado.Value);

                if (curso == null)
                {
                    MessageBox.Show("Curso no encontrado.");
                    return;
                }

                curso.Nombre = txtCursoNombre.Text.Trim();
                curso.Descripcion = txtDescripcion.Text.Trim();
                curso.Duracion = duracion;
                curso.CupoMaximo = cupo;
                curso.Activo = chkActivo.Checked;

                cursoRepository.Modificar(curso);
                CargarCursos();
                LimpiarCurso();

                MessageBox.Show("Curso modificado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar curso:\n" + ex.Message);
            }
        }

        private void EliminarCurso()
        {
            if (cursoIdSeleccionado == null)
            {
                MessageBox.Show("Seleccioná un curso.");
                return;
            }

            if (MessageBox.Show("¿Eliminar el curso seleccionado?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                cursoRepository.Eliminar(cursoIdSeleccionado.Value);
                CargarCursos();
                LimpiarCurso();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo eliminar el curso. Puede tener inscripciones asociadas.\n" + ex.Message);
            }
        }

        private void SeleccionarCurso()
        {
            if (dgvCursos.CurrentRow == null)
                return;

            var fila = dgvCursos.CurrentRow;
            cursoIdSeleccionado = Convert.ToInt32(fila.Cells["Id"].Value);

            txtCursoNombre.Text = fila.Cells["Nombre"].Value?.ToString() ?? "";
            txtDescripcion.Text = fila.Cells["Descripcion"].Value?.ToString() ?? "";
            txtDuracion.Text = fila.Cells["Duracion"].Value?.ToString() ?? "";
            txtCupo.Text = fila.Cells["CupoMaximo"].Value?.ToString() ?? "";
            chkActivo.Checked = Convert.ToBoolean(fila.Cells["Activo"].Value);
        }

        private void LimpiarCurso()
        {
            txtCursoNombre.Clear();
            txtDescripcion.Clear();
            txtDuracion.Clear();
            txtCupo.Clear();
            chkActivo.Checked = true;
            cursoIdSeleccionado = null;
            dgvCursos.ClearSelection();
        }

        // =====================================
        // INSCRIPCIONES
        // =====================================
        private void CrearInterfazInscripciones(Panel panel)
        {
            panel.Controls.Add(new Label { Text = "Seleccionar Alumno:", Left = 10, Top = 60, Width = 150 });
            cmbInscripcionAlumno.SetBounds(10, 85, 300, 30);
            cmbInscripcionAlumno.DropDownStyle = ComboBoxStyle.DropDownList;
            panel.Controls.Add(cmbInscripcionAlumno);

            panel.Controls.Add(new Label { Text = "Seleccionar Curso Activo:", Left = 330, Top = 60, Width = 180 });
            cmbInscripcionCurso.SetBounds(330, 85, 300, 30);
            cmbInscripcionCurso.DropDownStyle = ComboBoxStyle.DropDownList;
            panel.Controls.Add(cmbInscripcionCurso);

            CrearBoton(panel, "Inscribir Alumno", 650, 83, 150, 32, EjecutarInscripcion);
        }

        private void CargarCombosInscripciones()
        {
            if (cmbInscripcionAlumno.IsDisposed || cmbInscripcionCurso.IsDisposed)
                return;

            try
            {
                var alumnos = alumnoRepository.ObtenerTodos()
                    .OrderBy(a => a.Apellido)
                    .ThenBy(a => a.Nombre)
                    .ToList();

                var cursos = cursoRepository.ObtenerTodos()
                    .Where(c => c.Activo)
                    .OrderBy(c => c.Nombre)
                    .ToList();

                cmbInscripcionAlumno.DataSource = null;
                cmbInscripcionAlumno.DisplayMember = "Apellido";
                cmbInscripcionAlumno.ValueMember = "Id";
                cmbInscripcionAlumno.DataSource = alumnos;

                cmbInscripcionCurso.DataSource = null;
                cmbInscripcionCurso.DisplayMember = "Nombre";
                cmbInscripcionCurso.ValueMember = "Id";
                cmbInscripcionCurso.DataSource = cursos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar combos de inscripción:\n" + ex.Message);
            }
        }

        private void EjecutarInscripcion()
        {
            if (cmbInscripcionAlumno.SelectedValue == null || cmbInscripcionCurso.SelectedValue == null)
            {
                MessageBox.Show("Seleccioná un alumno y un curso válidos.");
                return;
            }

            try
            {
                int alumnoId = Convert.ToInt32(cmbInscripcionAlumno.SelectedValue);
                int cursoId = Convert.ToInt32(cmbInscripcionCurso.SelectedValue);

                inscripcionRepository.InscribirAlumno(alumnoId, cursoId);

                MessageBox.Show("¡Inscripción realizada con éxito!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al realizar la inscripción:\n" + ex.Message, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // =====================================
        // REPORTES
        // =====================================
        private void CrearInterfazReportes()
        {
            panelReportes.Controls.Add(new Label { Text = "Curso:", Left = 10, Top = 60, Width = 100 });

            cmbCursoReporte.SetBounds(10, 85, 240, 30);
            cmbCursoReporte.DropDownStyle = ComboBoxStyle.DropDownList;
            panelReportes.Controls.Add(cmbCursoReporte);

            CrearBoton(panelReportes, "Ver inscriptos", 265, 85, 130, 30, ReporteAlumnosInscriptos);
            CrearBoton(panelReportes, "Cantidad por curso", 405, 85, 150, 30, ReporteCantidadPorCurso);
            CrearBoton(panelReportes, "Cupos disponibles", 565, 85, 150, 30, ReporteCuposDisponibles);

            panelReportes.Controls.Add(new Label { Text = "Alumno:", Left = 10, Top = 130, Width = 100 });

            cmbAlumnoReporte.SetBounds(10, 155, 240, 30);
            cmbAlumnoReporte.DropDownStyle = ComboBoxStyle.DropDownList;
            panelReportes.Controls.Add(cmbAlumnoReporte);

            CrearBoton(panelReportes, "Ver cursos del alumno", 265, 155, 190, 30, ReporteCursosDeAlumno);

            ConfigurarGrilla(dgvReporte);
            dgvReporte.SetBounds(10, 205, 940, 375);
            panelReportes.Controls.Add(dgvReporte);
        }

        private void CargarCombosReportes()
        {
            if (cmbCursoReporte.IsDisposed || cmbAlumnoReporte.IsDisposed)
                return;

            try
            {
                int? cursoAnterior = cmbCursoReporte.SelectedValue is int ci ? ci : null;
                int? alumnoAnterior = cmbAlumnoReporte.SelectedValue is int ai ? ai : null;

                var cursos = cursoRepository.ObtenerTodos()
                    .Where(c => c.Activo)
                    .OrderBy(c => c.Nombre)
                    .ToList();

                var alumnos = alumnoRepository.ObtenerTodos()
                    .OrderBy(a => a.Apellido)
                    .ThenBy(a => a.Nombre)
                    .ToList();

                cmbCursoReporte.DataSource = null;
                cmbCursoReporte.DisplayMember = "Nombre";
                cmbCursoReporte.ValueMember = "Id";
                cmbCursoReporte.DataSource = cursos;

                cmbAlumnoReporte.DataSource = null;
                cmbAlumnoReporte.DisplayMember = "Apellido";
                cmbAlumnoReporte.ValueMember = "Id";
                cmbAlumnoReporte.DataSource = alumnos;

                if (cursoAnterior.HasValue && cursos.Any(c => c.Id == cursoAnterior.Value))
                    cmbCursoReporte.SelectedValue = cursoAnterior.Value;

                if (alumnoAnterior.HasValue && alumnos.Any(a => a.Id == alumnoAnterior.Value))
                    cmbAlumnoReporte.SelectedValue = alumnoAnterior.Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar opciones de reportes:\n" + ex.Message);
            }
        }

        private void ReporteAlumnosInscriptos()
        {
            if (cmbCursoReporte.SelectedValue == null)
            {
                MessageBox.Show("No hay cursos activos para consultar.");
                return;
            }

            try
            {
                int cursoId = Convert.ToInt32(cmbCursoReporte.SelectedValue);
                var inscripciones = inscripcionRepository.ObtenerAlumnosActivosPorCurso(cursoId);

                dgvReporte.DataSource = null;
                dgvReporte.DataSource = inscripciones.Select(i => new
                {
                    i.Alumno.Id,
                    i.Alumno.Nombre,
                    i.Alumno.Apellido,
                    i.Alumno.DNI,
                    i.Alumno.Email,
                    FechaInscripcion = i.FechaInscripcion.ToString("dd/MM/yyyy")
                }).ToList();

                if (dgvReporte.Columns["Id"] != null)
                    dgvReporte.Columns["Id"].Visible = false;

                if (inscripciones.Count == 0)
                    MessageBox.Show("El curso no tiene alumnos inscriptos activos.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar inscriptos:\n" + ex.Message);
            }
        }

        private void ReporteCursosDeAlumno()
        {
            if (cmbAlumnoReporte.SelectedValue == null)
            {
                MessageBox.Show("No hay alumnos cargados.");
                return;
            }

            try
            {
                int alumnoId = Convert.ToInt32(cmbAlumnoReporte.SelectedValue);
                var inscripciones = inscripcionRepository.ObtenerInscripcionesPorAlumno(alumnoId);

                dgvReporte.DataSource = null;
                dgvReporte.DataSource = inscripciones.Select(i => new
                {
                    Curso = i.Curso.Nombre,
                    DuracionHoras = i.Curso.Duracion,
                    FechaInscripcion = i.FechaInscripcion.ToString("dd/MM/yyyy"),
                    i.Estado
                }).ToList();

                if (inscripciones.Count == 0)
                    MessageBox.Show("El alumno no tiene inscripciones.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar cursos:\n" + ex.Message);
            }
        }

        private void ReporteCantidadPorCurso()
        {
            try
            {
                var cursos = cursoRepository.ObtenerTodos().OrderBy(c => c.Id).ToList();
                var cantidades = inscripcionRepository.ObtenerCantidadAlumnosActivosPorCurso();

                dgvReporte.DataSource = null;
                dgvReporte.DataSource = cursos.Select(c =>
                {
                    cantidades.TryGetValue(c.Id, out int cantidad);
                    return new
                    {
                        c.Id,
                        c.Nombre,
                        c.CupoMaximo,
                        Inscriptos = cantidad,
                        Estado = c.Activo ? "Activo" : "Baja"
                    };
                }).ToList();

                if (dgvReporte.Columns["Id"] != null)
                    dgvReporte.Columns["Id"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar reporte:\n" + ex.Message);
            }
        }

        private void ReporteCuposDisponibles()
        {
            try
            {
                var cursos = cursoRepository.ObtenerTodos().Where(c => c.Activo).OrderBy(c => c.Nombre).ToList();
                var cantidades = inscripcionRepository.ObtenerCantidadAlumnosActivosPorCurso();

                dgvReporte.DataSource = null;
                dgvReporte.DataSource = cursos.Select(c =>
                {
                    cantidades.TryGetValue(c.Id, out int cantidad);
                    return new
                    {
                        c.Id,
                        c.Nombre,
                        c.CupoMaximo,
                        Inscriptos = cantidad,
                        CuposDisponibles = Math.Max(0, c.CupoMaximo - cantidad)
                    };
                })
                .Where(c => c.CuposDisponibles > 0)
                .ToList();

                if (dgvReporte.Columns["Id"] != null)
                    dgvReporte.Columns["Id"].Visible = false;

                if (dgvReporte.Rows.Count == 0)
                    MessageBox.Show("No hay cursos activos con cupos disponibles.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar reporte:\n" + ex.Message);
            }
        }
    }
}