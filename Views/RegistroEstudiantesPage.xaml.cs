using System.Globalization;
using RegistroEstudiantes.Data;
using RegistroEstudiantes.Models;

namespace RegistroEstudiantes.Views;

public partial class RegistroEstudiantesPage : ContentPage
{
    private readonly SQLiteDatabase _database;

    public RegistroEstudiantesPage()
    {
        InitializeComponent();

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "estudiantes.db3");
        _database = new SQLiteDatabase(dbPath);

        LimpiarFormulario();
    }

    private async void GuardarButton_Clicked(object? sender, EventArgs e)
    {
        // 1. Validar los datos ingresados
        Estudiante? estudiante = ValidarFormulario(out List<string> errores, out VisualElement? campoInvalido);

        if (estudiante is null)
        {
            await DisplayAlert(
                "Datos incompletos o inválidos",
                string.Join(Environment.NewLine, errores),
                "OK");

            campoInvalido?.Focus();
            return;
        }

        // 2. Guardar en SQLite (deshabilitamos el botón para evitar duplicados por doble toque)
        GuardarButton.IsEnabled = false;
        try
        {
            await _database.GuardarEstudianteAsync(estudiante);

            await DisplayAlert(
                "Éxito",
                $"El estudiante se registró correctamente (ID: {estudiante.IdEstudiante}).",
                "OK");

            LimpiarFormulario();
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Error",
                $"Ocurrió un error al guardar el estudiante: {ex.Message}",
                "OK");
        }
        finally
        {
            GuardarButton.IsEnabled = true;
        }
    }

    /// Valida todos los campos. Devuelve el estudiante si todo es correcto;
    /// de lo contrario devuelve null, la lista de errores y el primer campo con error.
    private Estudiante? ValidarFormulario(out List<string> errores, out VisualElement? primerCampoInvalido)
    {
        var listaErrores = new List<string>();
        VisualElement? primerInvalido = null;

        void Falla(string mensaje, VisualElement campo)
        {
            listaErrores.Add($"• {mensaje}");
            primerInvalido ??= campo;
        }

        // Nombre
        string nombre = NombreEntry.Text?.Trim() ?? string.Empty;
        if (nombre.Length == 0)
            Falla("El nombre del estudiante es obligatorio.", NombreEntry);

        // Apellido
        string apellido = ApellidoEntry.Text?.Trim() ?? string.Empty;
        if (apellido.Length == 0)
            Falla("El apellido del estudiante es obligatorio.", ApellidoEntry);

        // Correo
        string correo = CorreoEntry.Text?.Trim() ?? string.Empty;
        if (correo.Length == 0)
            Falla("El correo electrónico es obligatorio.", CorreoEntry);
        else if (!correo.Contains('@') || !correo.Contains('.'))
            Falla("El correo electrónico no es válido.", CorreoEntry);

        // Teléfono (opcional, pero si se llena validamos longitud o formato opcional)
        string telefono = TelefonoEntry.Text?.Trim() ?? string.Empty;

        // Carrera (opcional)
        string carrera = CarreraEntry.Text?.Trim() ?? string.Empty;

        // Fecha de registro
        DateTime fecha = DateTime.Today;
        if (FechaRegistroPicker.Date is not DateTime fechaSeleccionada)
            Falla("Selecciona la fecha de registro.", FechaRegistroPicker);
        else if (fechaSeleccionada.Date > DateTime.Today)
            Falla("La fecha de registro no puede ser futura.", FechaRegistroPicker);
        else
            fecha = fechaSeleccionada.Date;

        errores = listaErrores;
        primerCampoInvalido = primerInvalido;

        if (listaErrores.Count > 0)
            return null;

        return new Estudiante
        {
            Nombre = nombre,
            Apellido = apellido,
            Correo = correo,
            Telefono = telefono,
            Carrera = carrera,
            FechaRegistro = fecha
        };
    }

    private void LimpiarFormulario()
    {
        IdEntry.Text = string.Empty;
        NombreEntry.Text = string.Empty;
        ApellidoEntry.Text = string.Empty;
        CorreoEntry.Text = string.Empty;
        TelefonoEntry.Text = string.Empty;
        CarreraEntry.Text = string.Empty;
        FechaRegistroPicker.Date = DateTime.Today;
    }
}