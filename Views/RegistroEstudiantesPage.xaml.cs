using System.Collections.ObjectModel;
using System.Net.Mail;
using RegistroEstudiantes.Data;
using RegistroEstudiantes.Models;
using SQLite;

namespace RegistroEstudiantes.Views;

public partial class RegistroEstudiantesPage : ContentPage
{
    private static readonly string[] Carreras =
    {
        "Ingeniería en Sistemas",
        "Administración de Empresas",
        "Contaduría Pública",
        "Derecho",
        "Medicina",
        "Psicología",
        "Arquitectura",
        "Mercadotecnia"
    };

    private readonly SQLiteDatabase _database;
    private readonly ObservableCollection<Estudiante> _estudiantes = new();

    public RegistroEstudiantesPage()
    {
        InitializeComponent();

        // Archivo nuevo: el esquema anterior (IdEstudiante, Nombre, Apellido...) es incompatible.
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "registro_estudiantes.db3");
        _database = new SQLiteDatabase(dbPath);

        CarreraPicker.ItemsSource = Carreras;
        BindableLayout.SetItemsSource(ListaEstudiantes, _estudiantes);

        LimpiarFormulario();
    }

    // Consulta: al abrir la página se cargan los estudiantes ya guardados (persistencia).
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarEstudiantesAsync();
    }

    private async Task CargarEstudiantesAsync()
    {
        try
        {
            var lista = await _database.ObtenerEstudiantesAsync();

            _estudiantes.Clear();
            foreach (var estudiante in lista)
                _estudiantes.Add(estudiante);

            TituloListaLabel.Text = $"Estudiantes registrados ({_estudiantes.Count})";
            SinRegistrosLabel.IsVisible = _estudiantes.Count == 0;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"No se pudo cargar la lista de estudiantes: {ex.Message}", "OK");
        }
    }

    private async void GuardarButton_Clicked(object? sender, EventArgs e)
    {
        // 1. Validar campos obligatorios y formato del correo
        Estudiante? estudiante = ValidarFormulario(out List<string> errores, out VisualElement? campoInvalido);

        if (estudiante is null)
        {
            await DisplayAlertAsync(
                "Datos incompletos o inválidos",
                string.Join(Environment.NewLine, errores),
                "OK");

            campoInvalido?.Focus();
            return;
        }

        // 2. Guardar en SQLite (se deshabilita el botón para evitar duplicados por doble toque)
        GuardarButton.IsEnabled = false;
        try
        {
            // Validar cuenta única antes de insertar
            if (await _database.ExisteNumeroCuentaAsync(estudiante.NumeroCuenta))
            {
                await MostrarCuentaDuplicadaAsync(estudiante.NumeroCuenta);
                return;
            }

            await _database.GuardarEstudianteAsync(estudiante);

            await CargarEstudiantesAsync();
            LimpiarFormulario();

            await DisplayAlertAsync(
                "Éxito",
                $"El estudiante {estudiante.NombreCompleto} se registró correctamente.",
                "OK");
        }
        catch (SQLiteException ex) when (ex.Result == SQLite3.Result.Constraint)
        {
            // Respaldo: el índice único de la base de datos rechazó la cuenta repetida.
            await MostrarCuentaDuplicadaAsync(estudiante.NumeroCuenta);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Error",
                $"Ocurrió un error al guardar el estudiante: {ex.Message}",
                "OK");
        }
        finally
        {
            GuardarButton.IsEnabled = true;
        }
    }

    private async Task MostrarCuentaDuplicadaAsync(string numeroCuenta)
    {
        await DisplayAlertAsync(
            "Cuenta duplicada",
            $"Ya existe un estudiante con el número de cuenta {numeroCuenta}.",
            "OK");

        NumeroCuentaEntry.Focus();
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

        // Número de cuenta
        string numeroCuenta = NumeroCuentaEntry.Text?.Trim() ?? string.Empty;
        if (numeroCuenta.Length == 0)
            Falla("El número de cuenta es obligatorio.", NumeroCuentaEntry);

        // Nombre completo
        string nombreCompleto = NombreCompletoEntry.Text?.Trim() ?? string.Empty;
        if (nombreCompleto.Length == 0)
            Falla("El nombre completo es obligatorio.", NombreCompletoEntry);

        // Carrera
        string carrera = CarreraPicker.SelectedItem as string ?? string.Empty;
        if (carrera.Length == 0)
            Falla("Selecciona una carrera.", CarreraPicker);

        // Correo
        string correo = CorreoEntry.Text?.Trim() ?? string.Empty;
        if (correo.Length == 0)
            Falla("El correo electrónico es obligatorio.", CorreoEntry);
        else if (!CorreoEsValido(correo))
            Falla("El correo electrónico no tiene un formato válido (ejemplo@correo.com).", CorreoEntry);

        errores = listaErrores;
        primerCampoInvalido = primerInvalido;

        if (listaErrores.Count > 0)
            return null;

        return new Estudiante
        {
            NumeroCuenta = numeroCuenta,
            NombreCompleto = nombreCompleto,
            Carrera = carrera,
            Correo = correo
        };
    }

    // Formato válido: usuario@dominio.ext, sin espacios ni texto adicional.
    private static bool CorreoEsValido(string correo)
    {
        if (correo.Contains(' '))
            return false;

        try
        {
            var direccion = new MailAddress(correo);
            return direccion.Address == correo && direccion.Host.Contains('.');
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private void LimpiarFormulario()
    {
        NumeroCuentaEntry.Text = string.Empty;
        NombreCompletoEntry.Text = string.Empty;
        CarreraPicker.SelectedIndex = -1;
        CorreoEntry.Text = string.Empty;
    }
}