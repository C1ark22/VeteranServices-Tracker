using Microsoft.Extensions.DependencyInjection;

namespace SFSU_VeteranServices_Tracker
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

    }
}