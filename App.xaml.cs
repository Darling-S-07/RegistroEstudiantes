using Microsoft.Extensions.DependencyInjection;

namespace RegistroEstudiantes
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Barra de título azul con texto blanco, como en el diseño de referencia.
            var navegacion = new NavigationPage(new Views.RegistroEstudiantesPage())
            {
                BarBackgroundColor = (Color)Resources["AzulTitulo"],
                BarTextColor = Colors.White
            };

            return new Window(navegacion);
        }
    }
}