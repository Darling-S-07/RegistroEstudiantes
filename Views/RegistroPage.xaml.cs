using System.Collections.ObjectModel;
using System.Net.Mail;
using RegistroEstudiantes.Data;
using RegistroEstudiantes.Models;
using SQLite;

namespace RegistroEstudiantes.Views;

public partial class RegistroPage : ContentPage
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

    public RegistroPage()
    {
        InitializeComponent();

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "registro_estudiantes.db3");
        _database = new SQLiteDatabase(dbPath);

        CarreraPicker.ItemsSource = Carreras;
        FechaNacimientoPicker.MaximumDate = DateTime.Today;
        BindableLayout.SetItemsSource(ListaEstudiantes, _estudiantes);

        LimpiarFormulario();
    }

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
        // 1. Validar todos los campos
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

        // 2. Mostrar resumen de los datos antes de guardar
        bool confirmar = await DisplayAlertAsync(
            "Resumen del estudiante",
            ConstruirResumen(estudiante),
            "Guardar",
            "Cancelar");

        if (!confirmar)
            return;

        // 3. Guardar en SQLite (se deshabilita el botón para evitar duplicados por doble toque)
        GuardarButton.IsEnabled = false;
        try
        {
            if (await _database.ExisteNumeroCuentaAsync(estudiante.NumeroCuenta))
            {
                await MostrarCuentaDuplicadaAsync(estudiante.NumeroCuenta);
                return;
            }

            await _database.GuardarEstudianteAsync(estudiante);

            await CargarEstudiantesAsync();
            LimpiarFormulario();

            await DisplayAlertAsync("Éxito", "El estudiante se registró correctamente.", "OK");
        }
        catch (SQLiteException ex) when (ex.Result == SQLite3.Result.Constraint)
        {
            await MostrarCuentaDuplicadaAsync(estudiante.NumeroCuenta);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Ocurrió un error al guardar el estudiante: {ex.Message}", "OK");
        }
        finally
        {
            GuardarButton.IsEnabled = true;
        }
    }

    private void LimpiarButton_Clicked(object? sender, EventArgs e)
    {
        LimpiarFormulario();
    }

    private async Task MostrarCuentaDuplicadaAsync(string numeroCuenta)
    {
        await DisplayAlertAsync(
            "Cuenta duplicada",
            $"Ya existe un estudiante con el número de cuenta {numeroCuenta}.",
            "OK");

        NumeroCuentaEntry.Focus();
    }

    private static string ConstruirResumen(Estudiante estudiante)
    {
        return
            $"Número de cuenta: {estudiante.NumeroCuenta}\n" +
            $"Nombre: {estudiante.NombreCompleto}\n" +
            $"Correo: {estudiante.Correo}\n" +
            $"Teléfono: {estudiante.Telefono}\n" +
            $"Carrera: {estudiante.Carrera}\n" +
            $"Fecha de nacimiento: {estudiante.FechaNacimiento:dd/MM/yyyy}\n" +
            $"Género: {estudiante.Genero}\n" +
            $"Acepta términos: {(estudiante.AceptaTerminos ? "Sí" : "No")}";
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
        else if (!NumeroCuentaEsValido(numeroCuenta))
            Falla("El número de cuenta debe contener solo dígitos (sin letras ni símbolos).", NumeroCuentaEntry);

        // Nombre completo
        string nombreCompleto = NombreCompletoEntry.Text?.Trim() ?? string.Empty;
        if (nombreCompleto.Length == 0)
            Falla("El nombre completo es obligatorio.", NombreCompletoEntry);
        else if (!NombreCompletoEsValido(nombreCompleto))
            Falla("El nombre completo solo debe contener letras y espacios (mínimo dos palabras).", NombreCompletoEntry);

        // Correo
        string correo = CorreoEntry.Text?.Trim() ?? string.Empty;
        if (correo.Length == 0)
            Falla("El correo electrónico es obligatorio.", CorreoEntry);
        else if (!CorreoEsValido(correo))
            Falla("El correo electrónico no tiene un formato válido (ejemplo@correo.com).", CorreoEntry);

        // Teléfono
        string telefono = TelefonoEntry.Text?.Trim() ?? string.Empty;
        if (telefono.Length == 0)
            Falla("El teléfono es obligatorio.", TelefonoEntry);
        else if (!TelefonoEsValido(telefono))
            Falla("El teléfono debe contener solo números (y opcionalmente un guion), con al menos 8 dígitos.", TelefonoEntry);

        // Carrera
        string carrera = CarreraPicker.SelectedItem as string ?? string.Empty;
        if (carrera.Length == 0)
            Falla("Selecciona una carrera.", CarreraPicker);

        // Fecha de nacimiento
        DateTime? fechaNacimiento = FechaNacimientoPicker.Date;
        if (fechaNacimiento is null || fechaNacimiento.Value.Date >= DateTime.Today)
            Falla("Selecciona una fecha de nacimiento válida.", FechaNacimientoPicker);
        else if (DateTime.Today.AddYears(-120) > fechaNacimiento.Value.Date)
            Falla("La fecha de nacimiento no es válida.", FechaNacimientoPicker);

        // Género
        string genero = string.Empty;
        if (GeneroMasculinoRadio.IsChecked)
            genero = "Masculino";
        else if (GeneroFemeninoRadio.IsChecked)
            genero = "Femenino";
        else if (GeneroOtroRadio.IsChecked)
            genero = "Otro";

        if (genero.Length == 0)
            Falla("Selecciona un género.", GeneroMasculinoRadio);

        // Términos y condiciones
        if (!TerminosCheckBox.IsChecked)
            Falla("Debes aceptar los términos y condiciones.", TerminosCheckBox);

        errores = listaErrores;
        primerCampoInvalido = primerInvalido;

        if (listaErrores.Count > 0)
            return null;

        return new Estudiante
        {
            NumeroCuenta = numeroCuenta,
            NombreCompleto = nombreCompleto,
            Correo = correo,
            Telefono = telefono,
            Carrera = carrera,
            FechaNacimiento = fechaNacimiento!.Value.Date,
            Genero = genero,
            AceptaTerminos = TerminosCheckBox.IsChecked
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

    // Acepta dígitos y guiones, con al menos 8 dígitos (formato hondureño típico: 9999-9999).
    private static bool TelefonoEsValido(string telefono)
    {
        int digitos = telefono.Count(char.IsDigit);
        bool soloDigitosYGuiones = telefono.All(c => char.IsDigit(c) || c == '-');
        return soloDigitosYGuiones && digitos >= 8;
    }

    // Solo dígitos, sin letras ni símbolos (ej. 20231001).
    private static bool NumeroCuentaEsValido(string numeroCuenta)
    {
        return numeroCuenta.All(char.IsDigit);
    }

    // Solo letras (incluye tildes y ñ) y espacios, con al menos dos palabras (nombre y apellido).
    private static bool NombreCompletoEsValido(string nombreCompleto)
    {
        bool soloLetrasYEspacios = nombreCompleto.All(c => char.IsLetter(c) || c == ' ');
        int palabras = nombreCompleto.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        return soloLetrasYEspacios && palabras >= 2;
    }

    private void LimpiarFormulario()
    {
        NumeroCuentaEntry.Text = string.Empty;
        NombreCompletoEntry.Text = string.Empty;
        CorreoEntry.Text = string.Empty;
        TelefonoEntry.Text = string.Empty;
        CarreraPicker.SelectedIndex = -1;
        FechaNacimientoPicker.Date = DateTime.Today.AddYears(-18);
        GeneroMasculinoRadio.IsChecked = false;
        GeneroFemeninoRadio.IsChecked = false;
        GeneroOtroRadio.IsChecked = false;
        TerminosCheckBox.IsChecked = false;
    }
}