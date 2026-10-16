
using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using AccesoDatos.Models;
using AccesoDatos.Repositories;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private readonly GenericRepository<Alumno> repositorio = new();

        private readonly TextBox txtNombre = new();
        private readonly TextBox txtApellido = new();
        private readonly TextBox txtDNI = new();
        private readonly TextBox txtTelefono = new();
        private readonly TextBox txtEmail = new();
        private readonly DateTimePicker dtpNacimiento = new();
        private readonly DataGridView dgvAlumnos = new();

        private int? alumnoIdSeleccionado = null;

        public Form1()
        {
            InitializeComponent();
            CrearInterfaz();
            CargarAlumnos();
        }

        private void CrearInterfaz()
        {
            Text = "Administración de Alumnos";
            Width = 950;
            Height = 650;
            StartPosition = FormStartPosition.CenterScreen;

            Controls.Add(new Label
            {
                Text = "ADMINISTRACIÓN DE ALUMNOS",
                Left = 25,
                Top = 20,
                Width = 600,
                Height = 35,
                Font = new Font("Segoe UI", 16, FontStyle.Bold)
            });

            AgregarCampo("Nombre", txtNombre, 25, 80);
            AgregarCampo("Apellido", txtApellido, 250, 80);
            AgregarCampo("DNI", txtDNI, 475, 80);
            AgregarCampo("Teléfono", txtTelefono, 25, 150);
            AgregarCampo("Email", txtEmail, 250, 150);

            Controls.Add(new Label
            {
                Text = "Fecha de nacimiento",
                Left = 475,
                Top = 150,
                Width = 200
            });

            dtpNacimiento.SetBounds(475, 175, 200, 25);
            Controls.Add(dtpNacimiento);

            CrearBoton("Guardar", 25, 230, GuardarAlumno);
            CrearBoton("Modificar", 140, 230, ModificarAlumno);
            CrearBoton("Eliminar", 255, 230, EliminarAlumno);
            CrearBoton("Limpiar", 370, 230, LimpiarCampos);

            dgvAlumnos.SetBounds(25, 290, 875, 290);
            dgvAlumnos.ReadOnly = true;
            dgvAlumnos.AllowUserToAddRows = false;
            dgvAlumnos.MultiSelect = false;
            dgvAlumnos.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvAlumnos.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvAlumnos.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    SeleccionarAlumno();
            };

            Controls.Add(dgvAlumnos);
        }

        private void AgregarCampo(
            string etiqueta, TextBox caja, int x, int y)
        {
            Controls.Add(new Label
            {
                Text = etiqueta,
                Left = x,
                Top = y,
                Width = 200
            });

            caja.SetBounds(x, y + 25, 200, 25);
            Controls.Add(caja);
        }

        private void CrearBoton(
            string texto, int x, int y, Action accion)
        {
            Button boton = new Button
            {
                Text = texto,
                Left = x,
                Top = y,
                Width = 100,
                Height = 35
            };

            boton.Click += (s, e) => accion();
            Controls.Add(boton);
        }

        private void CargarAlumnos()
        {
            try
            {
                dgvAlumnos.DataSource = null;

                dgvAlumnos.DataSource = repositorio.ObtenerTodos()
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
                MessageBox.Show(
                    "Error al cargar alumnos:\n" + ex.Message);
            }
        }

        private bool ValidarCampos(
            out int dni, out int telefono)
        {
            dni = 0;
            telefono = 0;

            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("Ingresá nombre y apellido.");
                return false;
            }

            if (!int.TryParse(txtDNI.Text, out dni))
            {
                MessageBox.Show("El DNI debe ser numérico.");
                return false;
            }

            if (!int.TryParse(txtTelefono.Text, out telefono))
            {
                MessageBox.Show("El teléfono debe ser numérico.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Ingresá el email.");
                return false;
            }

            return true;
        }

        private void GuardarAlumno()
        {
            if (!ValidarCampos(out int dni, out int telefono))
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

                repositorio.Agregar(alumno);

                MessageBox.Show("Alumno guardado correctamente.");
                CargarAlumnos();
                LimpiarCampos();
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
                MessageBox.Show("Seleccioná un alumno de la tabla.");
                return;
            }

            if (!ValidarCampos(out int dni, out int telefono))
                return;

            try
            {
                Alumno alumno =
                    repositorio.ObtenerPorId(alumnoIdSeleccionado.Value);

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

                repositorio.Modificar(alumno);

                MessageBox.Show("Alumno modificado correctamente.");
                CargarAlumnos();
                LimpiarCampos();
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
                MessageBox.Show("Seleccioná un alumno de la tabla.");
                return;
            }

            if (MessageBox.Show(
                "¿Eliminar al alumno seleccionado?",
                "Confirmar",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                repositorio.Eliminar(alumnoIdSeleccionado.Value);

                MessageBox.Show("Alumno eliminado.");
                CargarAlumnos();
                LimpiarCampos();
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

            alumnoIdSeleccionado = Convert.ToInt32(
                fila.Cells["Id"].Value);

            txtNombre.Text =
                fila.Cells["Nombre"].Value?.ToString() ?? "";
            txtApellido.Text =
                fila.Cells["Apellido"].Value?.ToString() ?? "";
            txtDNI.Text =
                fila.Cells["DNI"].Value?.ToString() ?? "";
            txtTelefono.Text =
                fila.Cells["Telefono"].Value?.ToString() ?? "";
            txtEmail.Text =
                fila.Cells["Email"].Value?.ToString() ?? "";

            if (DateTime.TryParse(
                fila.Cells["FechaNacimiento"].Value?.ToString(),
                out DateTime fecha))
                dtpNacimiento.Value = fecha;
        }

        private void LimpiarCampos()
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
    }
}